using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrainingOrc : Enemy
{
    [SerializeField] private int value = 10;
    protected override void Awake()
    {
        base.Awake();
    }
    protected override void Start()
    {
        base.Start();
    }

    protected override void Update()
    {
        base.Update();
    }

    protected override void Die()
    {
        base.Die();
        CoinsManager.Coins += value;
        PlayerPrefs.SetInt("Coins", CoinsManager.Coins);
        TrainingManager.S.TrainingCoinsCount++;
    }
}
