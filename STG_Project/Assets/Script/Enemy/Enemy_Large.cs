using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy_Large : Enemy_pre
{
    protected override void Awake()
    { base.Awake(); setScore = 5000; }

    protected override void OnEnable()
    { base.OnEnable(); Health = 120; }

    protected override void Update()
    {
        if (StageManager.isGamePlay)
            base.Update();
    }
}
