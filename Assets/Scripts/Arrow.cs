using System.Collections;
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
    protected virtual void Start()
    {
        _trail = GetComponentInChildren<TrailRenderer>();     
        _rb2d = GetComponent<Rigidbody2D>();
        _initialArrowDmg = arrowDmg;
        _coll = gameObject.GetComponent<BoxCollider2D>();
    }


    protected virtual void Update()
    {
        if (_rb2d.simulated == true && _isHit == false)
        {
            float angle = Mathf.Atan2(_rb2d.velocity.y, _rb2d.velocity.x) * Mathf.Rad2Deg; // Angle between Velocity.x and Resulting Velocity
            transform.rotation = Quaternion.AngleAxis(angle - 90, Vector3.forward); // Rotate around Axis on a Angle value (-90) - ArrowRotationOffset  
            arrowDmg = _initialArrowDmg;
        }   
        else if (_isHit)
        {           
            arrowDmg = 0;
            _trail.gameObject.transform.SetParent(null);
        }

    }
    protected virtual void OnCollisionEnter2D(Collision2D collision)
    {
        
        _rb2d.velocity = Vector2.zero;
        _rb2d.isKinematic = true;
        DestroyThisArrow(3);

        switch (collision.gameObject.tag)
        {
            case "Enemy":
                AudioManager.INSTANCE.PlaySound(AudioManager.eSoundsNames.arrow_hit_enemy);
                break;

            case "Ground":
                AudioManager.INSTANCE.PlaySound(AudioManager.eSoundsNames.arrow_hit_ground);
                break;

            case "Stone":
                AudioManager.INSTANCE.PlaySound(AudioManager.eSoundsNames.arrow_hit_stone);
                break;

            case "Wood":
                AudioManager.INSTANCE.PlaySound(AudioManager.eSoundsNames.arrow_hit_wood);
                break;

            default: return;
        }

            Enemy enemy = collision.gameObject.GetComponent<Enemy>();
        
        if (enemy)
        {
             _coll.enabled = false;
            gameObject.transform.SetParent(enemy.transform);
        }
        if (collision.collider.GetComponent<EnemyHead>())
        {
            enemy.Damaged(Enemy.eBoydPart.head, arrowDmg, this.transform, _isHit);
            _coll.enabled = false;
            //Debug.Log("Hit Enemy's Head");
        }
        if (collision.collider.GetComponent<EnemyChest>())
        {
            enemy.Damaged(Enemy.eBoydPart.chest, arrowDmg, this.transform, _isHit);
            _coll.enabled = false;
            // Debug.Log("Hit Enemy's Chest");
        }
        if (collision.collider.GetComponent<EnemyLegs>())
        {
            enemy.Damaged(Enemy.eBoydPart.legs, arrowDmg, this.transform, _isHit);
            _coll.enabled = false;
            // Debug.Log("Hit Enemy's Legs");
        }
        if (collision.collider.GetComponent<EnemyShield>())
        {
            enemy.Block(Enemy.eBoydPart.shield, this.transform);
            _coll.enabled = false;
            // Debug.Log("Hit Enemy's Legs");
        }
        _isHit = true;
    }

    protected virtual void DestroyThisArrow(float t)
    {
        Destroy(this.gameObject, t);
    }
}
