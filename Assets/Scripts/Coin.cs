using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Coin : MonoBehaviour
{
    [SerializeField] private int value = 10;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Player player = collision.gameObject.GetComponent<Player>();
        if (player)
        {
            AudioManager.INSTANCE.PlaySound(AudioManager.eSoundsNames.coin);
            CoinsManager.Coins += value;
            PlayerPrefs.SetInt("Coins", CoinsManager.Coins);
            Destroy(this.gameObject);
            
        }

    }

}
