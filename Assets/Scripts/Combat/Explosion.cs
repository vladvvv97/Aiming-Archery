using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Explosion : MonoBehaviour
{
    public float ExplosionRadius;
    public float ExplosionForce;
    public float ExplosionDamage;
    public GameObject ExplosionEffect;
    private bool isAlreadyDamaged = false;


    private void Start()
    {
       // Invoke(nameof(Explode),2f);
    }

    public void Explode()
    {
        Collider2D[] overlappedColliders = Physics2D.OverlapCircleAll(transform.position, ExplosionRadius);
        for (int i = 0; i < overlappedColliders.Length; i++)
        {
            Rigidbody2D rb2D = overlappedColliders[i].attachedRigidbody;
            Enemy enemy = overlappedColliders[i].gameObject.GetComponentInParent<Enemy>();
            Player player = overlappedColliders[i].gameObject.GetComponentInParent<Player>();
            if (rb2D)
            {
                Vector2 direction = (overlappedColliders[i].transform.position - transform.position).normalized * 
                (1 - (overlappedColliders[i].transform.position - transform.position).magnitude / ExplosionRadius) * ExplosionForce;
                rb2D.AddForce(direction, ForceMode2D.Force);
                ExplosionEffect.transform.localScale = new Vector2(ExplosionRadius / 3, ExplosionRadius / 3);
                Instantiate(ExplosionEffect,transform);
                Destroy(gameObject,1f);

               if (rb2D.bodyType == RigidbodyType2D.Dynamic && !enemy && !player)
               {
                    Destroy(overlappedColliders[i].gameObject, 5f);
               }

            }
            
            //GameObject go = overlappedColliders[i].GetComponent<GameObject>();
            if (enemy && isAlreadyDamaged == false)
            {
                isAlreadyDamaged = true;
                enemy.Damaged(Enemy.eBoydPart.chest, ExplosionDamage);
            }

        }
        AudioManager.INSTANCE.PlaySound(AudioManager.eSoundsNames.explosion);
    }

   
    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(transform.position, ExplosionRadius);
    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.GetComponent<Arrow>())
        {
            collision.gameObject.transform.SetParent(this.transform);
            Invoke(nameof(Explode), 2f);
        }
    }
}

