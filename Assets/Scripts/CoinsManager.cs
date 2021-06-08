using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CoinsManager : MonoBehaviour
{
    public static int Coins;
    private Text _coinsCount;

    void Start()
    {
        _coinsCount = GetComponent<Text>();
        Coins = PlayerPrefs.GetInt("Coins", Coins);
    }

    // Update is called once per frame
    void Update()
    {
        _coinsCount.text = Coins.ToString();
    }
}
