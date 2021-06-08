using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wood_Arrow : Arrow
{
    [Header("Set in Inspector: Wood Arrow")]
    [SerializeField] public int woodArrowDmg;
    protected override void Start()
    {
        base.Start();
    }


    protected override void Update()
    {
        base.Update();
    }
}
