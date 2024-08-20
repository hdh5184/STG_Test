using UnityEngine;
using static Init_Enemy;
using static Logic_Enemy;

/* <Enemy 보조 무기> */
public class Enemy_SubWeapon : Enemy
{
    // 보조 무기 소유자
    public GameObject subWeaponOwner;
    // 공격 위치
    public GameObject[] obj_ShootPos;

    protected override void Awake()
    {
        base.Awake();
        Init(this, 150, 30, obj_ShootPos);
    }

    protected override void OnEnable()
    {
        base.OnEnable();
    }

    protected override void Update()
    {
        if (StageManager.isGamePlay)
        base.Update();

        Act_SubWeapon();
    }

    protected override void Dead()
    {
        subWeaponOwner.GetComponent<Enemy>().Health -= 50;
        Explosion(this, "ExplodeB", "EShotL");
        base.Dead();
    }





    /*************** 개별 매서드 모음 ***************/

    /// <summary> 보조 무기 행동 </summary>
    protected void Act_SubWeapon()
    {
        degree = SetDegree(transform.position);
        transform.rotation = Quaternion.Euler(0, 0, degree);
    }
}
