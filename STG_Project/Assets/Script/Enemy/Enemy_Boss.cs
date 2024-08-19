using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static StageManager;
using static Spawn_Boss;
using static Init_Enemy;
using static Logic_Enemy;

/* <Enemy 보스> */
public class Enemy_Boss : Enemy
{
    // 공격 위치 모음
    public GameObject[] obj_ShootPos;

    // 보스 공격 데이터 모음
    public Queue<BossData> queue_Attack;
    public BossData finalAttack;

    protected override void Awake()
    {
        base.Awake();
        Init(this, 80000, 1200, obj_ShootPos);

        queue_Attack = new Queue<BossData>();
        finalAttack = new BossData();
        firstWaitTime = 1.5f;
        isBoss = true;
    }

    protected override void OnEnable()
    {
        base.OnEnable();

        if (queue_Attack.Count != 0)
        SetBossData(this, queue_Attack.Dequeue());
        Change_Attack(attackType);
        Init_AttackData(this, ref attackData);
        StartCoroutine(FinalAttack());

        fieldTimeLimit = 10000;
    }

    protected override void Update()
    {
        if (StageManager.isGamePlay)
        base.Update();
    }

    protected override void CompareWait()
    {
        if (shootCount < shootLimit) return;

        enemyState = EnemyState.Wait;
        shootCount = 0;
        attackTime = 0;

        SetBossData(this, queue_Attack.Dequeue());
        Init_Pattern(this, getPatternType);
        Init_AttackData(this, ref attackData);
        Change_Attack(attackType);

        switch (getPatternType)
        {
            case "way":
            case "cir":
            case "spr":
                isLock = true; break;

            default:
                isLock = false; break;
        }

        set_Act = ComparePlay;
    }

    protected override void Dead()
    {
        enemyState = EnemyState.Dead;
        score += setScore;
        EnemyList.Remove(gameObject);
        StageManager.instance.GameClear();

        StartCoroutine(Dead_Boss(0.16f));
    }





    /*************** 개별 매서드 모음 ***************/

    /// <summary> 보스 기체 파괴 </summary>
    IEnumerator Dead_Boss(float explodeTime, bool isExplode = false)
    {
        InvokeRepeating("RandomExplosion", 0f, explodeTime);
        yield return new WaitForSeconds(1.5f);

        CancelInvoke("RandomExplosion");

        if (!isExplode) StartCoroutine(Dead_Boss(0.12f, true));
        else
        {
            GameObject explosion = Explosion(this, "ExplodeBoss", "ExplodeBoss");
            explosion.transform.localScale = new Vector3(10, 10, 1);
            gameObject.SetActive(false);
        }
    }

    /// <summary> 무작위 위치 폭발 연출 </summary>
    void RandomExplosion()
    {
        GameObject obj = Explosion(this, "ExplodeB", "EShotL", true);
        DropItem(obj.transform.position);
    }

    /// <summary> 보스 최후 공격 데이터 적용 </summary>
    IEnumerator FinalAttack()
    {
        yield return new WaitForSeconds(60f);

        StopAllCoroutines();
        queue_Attack.Clear();
        SetBossData(this, finalAttack);

        Init_Pattern(this, getPatternType);
        Change_Attack(attackType);

        shootCount = 0;
        attackTime = 0;

        Init_AttackData(this, ref attackData);
        enemyState = EnemyState.Play;
        set_Act = CompareWait;
    }
}
