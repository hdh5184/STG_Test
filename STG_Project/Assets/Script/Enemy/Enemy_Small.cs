using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy_Small : Enemy
{
    protected override void Awake()
    { base.Awake(); setScore = 100; }

    protected override void OnEnable()
    {
        base.OnEnable();
        Init();
        Health = 3;
    }

    void Init()
    {
        shootPos = new Vector2[1];
        shootPos[0] = Init_ShootPos();
    }

    protected override void Update()
    {
        if (StageManager.isGamePlay)
            base.Update();
    }

    protected override void Dead()
    {
        Explosion("ExplodeB", "EShotL");
        base.Dead();
    }
}
