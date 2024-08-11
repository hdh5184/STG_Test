using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy_Boss : Enemy
{
    public GameObject[] obj_ShootPos;
    int setBossPos = 0;
    bool isBossFinal = false;



    public Queue<BossData> queue_Attack;
    public Queue<BossData> queue_AttackFinal;

    protected override void Awake()
    {
        base.Awake();
        Init();
        setScore = 80000;
    }

    void Init()
    {
        shootPos = new Vector2[obj_ShootPos.Length];

        for (int i = 0; i < shootPos.Length; i++)
        shootPos[i] = Init_ShootPos(obj_ShootPos[i]);

        queue_Attack = new Queue<BossData>();
        queue_AttackFinal = new Queue<BossData>();
    }

    protected override void OnEnable()
    {
        base.OnEnable();
        Health = 1200;
        queue_Attack.Clear();
        queue_AttackFinal.Clear();
    }

    protected override void Update()
    {
        if (StageManager.isGamePlay)
            base.Update();
    }

}
