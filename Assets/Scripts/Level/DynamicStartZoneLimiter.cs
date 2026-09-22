using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DynamicStartZoneLimiter : MonoBehaviour
{
    private void OnTriggerStay2D(Collider2D collision)
    {
        Player player = collision.GetComponent<Player>();
        if (player)
        {
            if (player.transform.position.x >= this.transform.position.x )
            {
                player.transform.position = new Vector2(this.transform.position.x, player.transform.position.y) ;
            }
        }
    }
}
