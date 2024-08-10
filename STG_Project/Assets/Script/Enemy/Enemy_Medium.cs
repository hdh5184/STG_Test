using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy_Medium : Enemy_pre
{
    protected override void Awake()
    { base.Awake(); setScore = 500; }

    protected override void OnEnable()
    { base.OnEnable(); Health = 30; }

    protected override void Update()
    {
        if (StageManager.isGamePlay)
            base.Update();
    }
}
