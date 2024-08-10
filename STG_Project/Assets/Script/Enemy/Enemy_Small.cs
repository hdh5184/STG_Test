using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy_Small : Enemy_pre
{
    protected override void Awake()
    { base.Awake(); setScore = 100; }

    protected override void OnEnable()
    { base.OnEnable(); Health = 3; }

    protected override void Update()
    {
        if (StageManager.isGamePlay)
            base.Update();
    }
}
