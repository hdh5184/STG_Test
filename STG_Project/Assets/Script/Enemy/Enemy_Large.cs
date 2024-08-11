using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy_Large : Enemy
{
    protected override void Awake()
    { base.Awake(); setScore = 5000; }

    protected override void OnEnable()
    {
        base.OnEnable();
        Init();
        Health = 120;
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
        Explosion("ExplodeC", "Explode");
        base.Dead();
    }
}
