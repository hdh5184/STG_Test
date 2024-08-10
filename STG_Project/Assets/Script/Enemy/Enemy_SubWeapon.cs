using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy_SubWeapon : Enemy_pre
{
    public GameObject subWeaponOwner;

    protected override void Awake()
    { base.Awake(); setScore = 150; }

    protected override void OnEnable()
    { base.OnEnable(); Health = 30; }

    protected override void Update()
    {
        if (StageManager.isGamePlay)
            base.Update();
    }
}
