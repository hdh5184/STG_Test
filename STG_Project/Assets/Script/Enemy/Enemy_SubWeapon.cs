using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy_SubWeapon : Enemy
{
    public GameObject subWeaponOwner;
    public GameObject[] Obj_ShootPos;

    protected override void Awake()
    { base.Awake(); setScore = 150; }

    protected override void OnEnable()
    {
        base.OnEnable();
        Init();
        Health = 30;
    }

    void Init()
    {
        shootPos = new Vector2[Obj_ShootPos.Length];

        for (int i = 0; i < shootPos.Length; i++)
        shootPos[i] = Init_ShootPos(Obj_ShootPos[i]);
    }

    protected override void Update()
    {
        if (StageManager.isGamePlay)
        base.Update();

        Act_SubWeapon();
    }

    protected void Act_SubWeapon()
    {
        degree = Logic_Enemy.SetDegree(transform.position);
        transform.rotation = Quaternion.Euler(0, 0, degree);
    }

    protected override void Dead()
    {
        subWeaponOwner.GetComponent<Enemy>().Health -= 50;
        base.Dead();
    }
}
