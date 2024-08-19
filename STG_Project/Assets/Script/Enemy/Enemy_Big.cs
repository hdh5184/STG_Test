using System.Collections;
using UnityEngine;
using static Init_Enemy;
using static StageManager;
using static Logic_Enemy;

/* <Enemy 대형> */
public class Enemy_Big : Enemy
{
    // 보조 무기 모음
    public GameObject[] subWeaponObj;

    protected override void Awake()
    {
        base.Awake();
        Init(this, 20000, 350);
    }

    protected override void OnEnable()
    {
        base.OnEnable();
    }

    protected override void Update()
    {
        if (isGamePlay)
        base.Update();
    }

    protected override void Dead()
    {
        enemyState = EnemyState.Dead;
        score += setScore;
        EnemyList.Remove(gameObject);
        getDropItemName = "silver";

        StartCoroutine(Dead_Big());
    }





    /*************** 개별 매서드 모음 ***************/

    /// <summary> 대형 기체(군함) 파괴 </summary>
    IEnumerator Dead_Big()
    {
        InvokeRepeating("RandomExplosion", 0f, 0.12f);
        yield return new WaitForSeconds(1.2f);

        CancelInvoke("RandomExplosion");
        GameObject explosion = Explosion(this, "ExplodeBoss", "ExplodeBoss");
        explosion.transform.localScale = new Vector3(10, 10, 1);
        gameObject.SetActive(false);
    }

    /// <summary> 무작위 위치 폭발 연출 </summary>
    void RandomExplosion()
    {
        GameObject obj = Explosion(this, "ExplodeB", "EShotL", true);
        DropItem(obj.transform.position);
    }
}
