using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy_Boss : Enemy_pre
{
    public GameObject[] BigShootPos;
    public GameObject BossShootPos1, BossShootPos2, BossShootPos3, BossShootPos4, BossShootPos5;
    int setBossPos = 0;
    bool shoot1, shoot2, shoot3, shoot4, shoot5;
    float degree1, degree2, degree3, degree4, degree5;
    bool isBossFinal = false;


    public Queue<BossData> bossLogics = new Queue<BossData>();
    public Queue<BossData> bossLogicsFinal = new Queue<BossData>();

    protected override void Awake()
    { base.Awake(); setScore = 80000; }

    protected override void OnEnable()
    { base.OnEnable(); Health = 1200; }

    protected override void Update()
    {
        if (StageManager.isGamePlay)
            base.Update();
    }
}
