using UnityEngine;
using static StageManager;
using static Logic_Bullet;

/* <Bullet 플레이어 C타입 최고 레벨 전용> */
public class Bullet_PlayerC : Bullet
{
    // 추적용 적기 오브젝트
    GameObject targetEnemy;

    private void Awake()
    {
        moveData.shootVec = Vector2.up;
        moveData.moveVec = Vector2.up * 8f;
        moveData.isEnemyBullet = false;
        power = 5;

        set_Mov = Move_Hom;
    }

    protected override void OnEnable()
    {
        base.OnEnable();
        moveData.shootVec = Vector2.up;
    }

    protected override void Update()
    {
        if (!isGamePlay) return;
        base.Update();
    }

    protected override void Move()
    {
        Target_Compare();
        Target_Search();

        if (targetEnemy == null)
        {
            Move_Str(ref moveData); return;
        }

        moveData.targetPos = targetEnemy.transform.position;
        base.Move();
    }





    /*************** 개별 매서드 모음 ***************/

    /// <summary> Target 존재 유무 검사 </summary>
    void Target_Compare()
    {
        // Target 미존재 시 미실행
        if (targetEnemy == null) return;

        // Target 비활성화 시 Target 초기화
        if (targetEnemy.gameObject.activeSelf == false) targetEnemy = null;
    }

    /// <summary> Target 검색 </summary>
    void Target_Search()
    {
        // Target 존재 시 미실행
        if (targetEnemy != null) return;

        // 탄과 가장 가까운 Enemy를 타겟으로 지정
        float minDis = 100;

        foreach (var enemy in EnemyList)
        {
            float dis = Vector2.Distance
                (enemy.gameObject.transform.position, transform.position);

            if (dis <= minDis)
            {
                minDis = dis;
                targetEnemy = enemy;
            }
        }
    }
}