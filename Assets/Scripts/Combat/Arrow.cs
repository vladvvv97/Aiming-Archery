using System.Collections.Generic;
using UnityEngine;

public class Arrow : MonoBehaviour
{
    [Header("Set in Inspector: Arrow")]

    [SerializeField] public float arrowLifeTime = 10f;
    [SerializeField] public float arrowDmg = 10f;
    private Rigidbody2D _rb2d;
    private bool _isHit;
    private float _initialArrowDmg;
    private BoxCollider2D _coll;
    private TrailRenderer _trail;
    private ArrowTypeConfig _config;
    private int _pierceChargesLeft;
    private Vector2 _lastVelocity;
    private ParticleSystem _flame;

    public bool IsPiercing => _config != null && _config.id == ArrowTypeId.Piercing;

    public void ApplyConfig(ArrowTypeConfig config)
    {
        _config = config;
        if (config == null) return;

        arrowDmg = config.baseDamage;
        _initialArrowDmg = config.baseDamage;
        _pierceChargesLeft = config.pierceCharges;

        if (_rb2d == null) _rb2d = GetComponent<Rigidbody2D>();
        if (_rb2d != null)
            _rb2d.gravityScale = config.gravityScale;

        ApplyTrail(config);
        if (config.id == ArrowTypeId.Magic)
            EnsureMagicFlame();
    }

    protected virtual void Start()
    {
        _trail = GetComponentInChildren<TrailRenderer>();
        _rb2d = GetComponent<Rigidbody2D>();
        _coll = gameObject.GetComponent<BoxCollider2D>();
        if (_config != null)
            ApplyConfig(_config);
        else
            _initialArrowDmg = arrowDmg;
        LevelController.Instance?.RegisterArrow(this);
    }

    protected virtual void OnDestroy()
    {
        LevelController.Instance?.UnregisterArrow(this);
    }

    protected virtual void Update()
    {
        if (_rb2d != null && _rb2d.simulated && _isHit == false)
        {
            if (_rb2d.linearVelocity.sqrMagnitude > 0.01f)
                _lastVelocity = _rb2d.linearVelocity;
            float angle = Mathf.Atan2(_rb2d.linearVelocity.y, _rb2d.linearVelocity.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.AngleAxis(angle - 90, Vector3.forward);
            arrowDmg = _initialArrowDmg;
        }
        else if (_isHit)
        {
            arrowDmg = 0;
            if (_trail != null)
                _trail.gameObject.transform.SetParent(null);
            if (_flame != null)
                _flame.Stop(true, ParticleSystemStopBehavior.StopEmitting);
        }
    }

    protected virtual void OnCollisionEnter2D(Collision2D collision)
    {
        if (_isHit || collision.collider == null) return;

        Vector2 point = collision.contactCount > 0 ? collision.GetContact(0).point : (Vector2)transform.position;
        Vector2 incoming = _rb2d != null ? _rb2d.linearVelocity : Vector2.zero;
        ResolveImpact(collision.collider, point, incoming);
    }

    // Hit pipeline from SPEC.md 3.3. Public so Play Mode checks can drive it without a full flight.
    public void ResolveImpact(Collider2D collider, Vector2 point, Vector2 incomingVelocity)
    {
        if (_isHit || collider == null) return;
        if (_coll == null) _coll = GetComponent<BoxCollider2D>();
        if (_rb2d == null) _rb2d = GetComponent<Rigidbody2D>();

        if (TryPierceThrough(collider, point, incomingVelocity))
            return;

        _isHit = true;
        Stick();
        PlayHitSound(collider);

        Enemy enemy = collider.GetComponentInParent<Enemy>();
        EnemyShield shield = collider.GetComponent<EnemyShield>();
        bool damagedBody = false;
        bool shieldBlockedBody = false;

        if (shield != null && shield.Durability > 0 && enemy != null)
        {
            shield.ReduceDurability(1);
            shieldBlockedBody = true;
            enemy.Block(Enemy.eBoydPart.shield, transform);
            if (_coll != null) _coll.enabled = false;
        }
        else if (enemy != null)
        {
            damagedBody = ApplyBodyZone(enemy, collider, true);
        }

        if (_config != null && _config.aoeRadius > 0f)
        {
            ApplyMagicAoe(point, damagedBody ? enemy : null);
            MagicBurst.Play(point, _config.aoeRadius);
        }

        if (enemy != null && !shieldBlockedBody && _coll != null)
            _coll.enabled = false;
    }

    // Piercing spends one charge on the first solid hit: damage it, keep flying, stick on the next hit.
    private bool TryPierceThrough(Collider2D collider, Vector2 point, Vector2 incomingVelocity)
    {
        if (!IsPiercing || _pierceChargesLeft <= 0) return false;
        _pierceChargesLeft--;

        Enemy enemy = collider.GetComponentInParent<Enemy>();
        EnemyShield shield = collider.GetComponent<EnemyShield>();

        if (shield != null && shield.Durability > 0)
        {
            shield.ReduceDurability(1);
            IgnoreCollider(collider);
        }
        else if (enemy != null)
        {
            ApplyBodyZone(enemy, collider, false);
            IgnoreColliders(enemy.GetComponentsInChildren<Collider2D>());
        }
        else
        {
            Transform root = collider.attachedRigidbody != null
                ? collider.attachedRigidbody.transform
                : collider.transform;
            IgnoreColliders(root.GetComponentsInChildren<Collider2D>());
        }

        PlayHitSound(collider);
        Vector2 keep = _lastVelocity.sqrMagnitude > 0.01f ? _lastVelocity : incomingVelocity;
        if (_rb2d != null && keep.sqrMagnitude > 0.01f)
            _rb2d.linearVelocity = keep;
        return true;
    }

    private void IgnoreCollider(Collider2D collider)
    {
        if (_coll == null || collider == null) return;
        Physics2D.IgnoreCollision(_coll, collider, true);
    }

    private void IgnoreColliders(Collider2D[] cols)
    {
        if (cols == null) return;
        for (int i = 0; i < cols.Length; i++)
            IgnoreCollider(cols[i]);
    }

    private bool ApplyBodyZone(Enemy enemy, Collider2D collider, bool stickArrow)
    {
        if (enemy == null || collider == null) return false;
        Transform stick = stickArrow ? transform : null;

        if (collider.GetComponent<EnemyHead>())
        {
            enemy.Damaged(Enemy.eBoydPart.head, arrowDmg, stick, false);
            return true;
        }
        if (collider.GetComponent<EnemyChest>())
        {
            enemy.Damaged(Enemy.eBoydPart.chest, arrowDmg, stick, false);
            return true;
        }
        if (collider.GetComponent<EnemyLegs>())
        {
            enemy.Damaged(Enemy.eBoydPart.legs, arrowDmg, stick, false);
            return true;
        }

        return false;
    }

    private void ApplyMagicAoe(Vector2 point, Enemy exclude)
    {
        Collider2D[] hits = Physics2D.OverlapCircleAll(point, _config.aoeRadius);
        var seen = new HashSet<Enemy>();
        if (exclude != null) seen.Add(exclude);

        for (int i = 0; i < hits.Length; i++)
        {
            Collider2D hit = hits[i];
            if (hit == null || hit.GetComponent<EnemyShield>() != null) continue;
            bool body = hit.GetComponent<EnemyHead>() || hit.GetComponent<EnemyChest>() || hit.GetComponent<EnemyLegs>();
            if (!body) continue;

            Enemy enemy = hit.GetComponentInParent<Enemy>();
            if (enemy == null || seen.Contains(enemy)) continue;
            seen.Add(enemy);
            enemy.Damaged(Enemy.eBoydPart.chest, arrowDmg, null, false);
        }
    }

    private void Stick()
    {
        if (_rb2d != null)
        {
            _rb2d.linearVelocity = Vector2.zero;
            _rb2d.angularVelocity = 0f;
            _rb2d.bodyType = RigidbodyType2D.Kinematic;
        }
        DestroyThisArrow(3f);
    }

    private void PlayHitSound(Collider2D collider)
    {
        if (AudioManager.INSTANCE == null || collider == null) return;

        if (IsGround(collider))
        {
            AudioManager.INSTANCE.PlaySound(AudioManager.eSoundsNames.arrow_hit_ground);
            return;
        }

        if (collider.GetComponentInParent<Enemy>() != null)
        {
            AudioManager.INSTANCE.PlaySound(AudioManager.eSoundsNames.arrow_hit_enemy);
            return;
        }

        switch (SurfaceTag(collider))
        {
            case "Stone":
                AudioManager.INSTANCE.PlaySound(AudioManager.eSoundsNames.arrow_hit_stone);
                break;
            case "Wood":
                AudioManager.INSTANCE.PlaySound(AudioManager.eSoundsNames.arrow_hit_wood);
                break;
        }
    }

    private static bool IsGround(Collider2D collider)
    {
        if (SurfaceTag(collider) == "Ground") return true;
        int groundLayer = LayerMask.NameToLayer("Ground");
        if (groundLayer < 0) return false;
        if (collider.gameObject.layer == groundLayer) return true;
        return collider.attachedRigidbody != null && collider.attachedRigidbody.gameObject.layer == groundLayer;
    }

    private static string SurfaceTag(Collider2D collider)
    {
        if (collider.CompareTag("Ground") || collider.CompareTag("Stone") || collider.CompareTag("Wood"))
            return collider.tag;
        if (collider.attachedRigidbody != null)
        {
            string bodyTag = collider.attachedRigidbody.gameObject.tag;
            if (bodyTag == "Ground" || bodyTag == "Stone" || bodyTag == "Wood")
                return bodyTag;
        }

        Transform parent = collider.transform.parent;
        while (parent != null)
        {
            if (parent.CompareTag("Ground") || parent.CompareTag("Stone") || parent.CompareTag("Wood"))
                return parent.tag;
            parent = parent.parent;
        }

        return collider.gameObject.tag;
    }

    private void ApplyTrail(ArrowTypeConfig config)
    {
        if (_trail == null) _trail = GetComponentInChildren<TrailRenderer>();
        if (_trail == null) return;

        Color color = config.trailColor;
        float width = 0.18f;
        float time = 0.35f;
        if (config.id == ArrowTypeId.Piercing)
        {
            width = 0.05f;
            time = 0.08f;
        }
        else if (config.id == ArrowTypeId.Magic)
        {
            width = 0.42f;
            time = 0.55f;
        }

        _trail.time = time;
        _trail.widthMultiplier = width;
        var gradient = new Gradient();
        gradient.SetKeys(
            new[] { new GradientColorKey(color, 0f), new GradientColorKey(color, 1f) },
            new[] { new GradientAlphaKey(0.05f, 0f), new GradientAlphaKey(0.9f, 0.35f), new GradientAlphaKey(0f, 1f) });
        _trail.colorGradient = gradient;
    }

    private void EnsureMagicFlame()
    {
        if (_flame != null) return;

        var flameObject = new GameObject("MagicFlame");
        flameObject.transform.SetParent(transform, false);
        flameObject.transform.localPosition = new Vector3(0f, 0.9f, 0f);
        _flame = flameObject.AddComponent<ParticleSystem>();

        var main = _flame.main;
        main.loop = true;
        main.playOnAwake = true;
        main.startLifetime = 0.28f;
        main.startSpeed = 0.6f;
        main.startSize = 0.28f;
        main.startColor = new ParticleSystem.MinMaxGradient(
            new Color(0.35f, 0.75f, 1f, 1f),
            new Color(0.75f, 1f, 1f, 1f));
        main.simulationSpace = ParticleSystemSimulationSpace.Local;
        main.maxParticles = 40;

        var emission = _flame.emission;
        emission.rateOverTime = 28f;

        var shape = _flame.shape;
        shape.shapeType = ParticleSystemShapeType.Cone;
        shape.angle = 18f;
        shape.radius = 0.05f;
        shape.rotation = new Vector3(-90f, 0f, 0f);

        var colorOverLife = _flame.colorOverLifetime;
        colorOverLife.enabled = true;
        var gradient = new Gradient();
        gradient.SetKeys(
            new[]
            {
                new GradientColorKey(new Color(0.85f, 1f, 1f), 0f),
                new GradientColorKey(new Color(0.15f, 0.35f, 1f), 1f)
            },
            new[] { new GradientAlphaKey(1f, 0f), new GradientAlphaKey(0f, 1f) });
        colorOverLife.color = gradient;

        _flame.Play();
    }

    protected virtual void DestroyThisArrow(float t)
    {
        Destroy(gameObject, t);
    }
}
