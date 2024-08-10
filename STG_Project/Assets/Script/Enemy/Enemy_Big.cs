using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy_Big : Enemy_pre
{
    public GameObject[] subWeaponObj;

    protected override void Awake()
    { base.Awake(); setScore = 20000; }

    protected override void OnEnable()
    { base.OnEnable(); Health = 350; }

    protected override void Update()
    {
        if (StageManager.isGamePlay)
        base.Update();
    }
}
