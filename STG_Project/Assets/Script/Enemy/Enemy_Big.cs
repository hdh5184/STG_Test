using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy_Big : Enemy
{
    public GameObject[] subWeaponObj;

    protected override void Awake()
    {
        base.Awake();
        setScore = 20000;
    }

    protected override void OnEnable()
    {
        base.OnEnable();
        Init();
        Health = 350;
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

}
