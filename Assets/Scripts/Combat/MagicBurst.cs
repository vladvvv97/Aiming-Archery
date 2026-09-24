using UnityEngine;

// Blue magic blast sized to the arrow's AOE radius. Spawned when a magic arrow sticks.
public class MagicBurst : MonoBehaviour
{
    private static Sprite _ringSprite;

    public static void Play(Vector2 point, float radius)
    {
        if (radius <= 0f) return;

        var root = new GameObject("MagicBurst");
        root.transform.position = point;

        var ring = new GameObject("Ring");
        ring.transform.SetParent(root.transform, false);
        var renderer = ring.AddComponent<SpriteRenderer>();
        renderer.sprite = RingSprite();
        renderer.color = new Color(0.45f, 0.9f, 1f, 0.55f);
        renderer.sortingOrder = 20;
        float diameter = radius * 2f;
        ring.transform.localScale = new Vector3(diameter, diameter, 1f);

        var particles = root.AddComponent<ParticleSystem>();
        var main = particles.main;
        main.duration = 0.45f;
        main.loop = false;
        main.playOnAwake = false;
        main.startLifetime = 0.4f;
        main.startSpeed = Mathf.Max(1.5f, radius * 2.2f);
        main.startSize = Mathf.Clamp(radius * 0.18f, 0.15f, 0.55f);
        main.startColor = new Color(0.55f, 0.95f, 1f, 1f);
        main.maxParticles = 64;
        main.simulationSpace = ParticleSystemSimulationSpace.World;

        var emission = particles.emission;
        emission.rateOverTime = 0f;
        emission.SetBursts(new[] { new ParticleSystem.Burst(0f, 36) });

        var shape = particles.shape;
        shape.shapeType = ParticleSystemShapeType.Circle;
        shape.radius = radius;
        shape.radiusThickness = 0.15f;

        var colorOverLife = particles.colorOverLifetime;
        colorOverLife.enabled = true;
        var gradient = new Gradient();
        gradient.SetKeys(
            new[]
            {
                new GradientColorKey(new Color(0.7f, 0.95f, 1f), 0f),
                new GradientColorKey(new Color(0.2f, 0.45f, 1f), 1f)
            },
            new[]
            {
                new GradientAlphaKey(1f, 0f),
                new GradientAlphaKey(0f, 1f)
            });
        colorOverLife.color = gradient;

        particles.Play();

        if (AudioManager.INSTANCE != null)
            AudioManager.INSTANCE.PlaySound(AudioManager.eSoundsNames.explosion);

        Object.Destroy(root, 0.9f);
    }

    private static Sprite RingSprite()
    {
        if (_ringSprite != null) return _ringSprite;

        const int size = 64;
        var texture = new Texture2D(size, size, TextureFormat.RGBA32, false);
        float mid = (size - 1) * 0.5f;
        float outer = mid;
        float inner = mid * 0.72f;
        for (int y = 0; y < size; y++)
        {
            for (int x = 0; x < size; x++)
            {
                float d = Vector2.Distance(new Vector2(x, y), new Vector2(mid, mid));
                float edge = Mathf.Clamp01((outer - d) / 2f) * Mathf.Clamp01((d - inner) / 3f);
                texture.SetPixel(x, y, new Color(1f, 1f, 1f, edge));
            }
        }
        texture.Apply();
        _ringSprite = Sprite.Create(texture, new Rect(0, 0, size, size), new Vector2(0.5f, 0.5f), size);
        return _ringSprite;
    }
}
