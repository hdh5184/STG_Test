using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static LobbyManager;
using static StageManager;

public class PlayerBullet : MonoBehaviour
{
    // 1. 타입 및 레벨
    public PBulletType PBType;
    public PBulletLv PBLevel;

    // 2. 참조 데이터
    public GameObject targetEnemy = null;

    // 3. 속성
    Vector3 ShootVec = Vector2.up;
    Vector2 MoveVec;
    float degreeZ;
    public int power = 0;

    // 4. 시간
    float fieldTime = 0f;

    // e. 속성 모음
    public enum PBulletLv { Lv1, Lv2, Lv3, LvMAX_A, LvMAX_B, LvMAX_C }
    public enum PBulletType { Bullet, Accel, Homing }

    // @. 대리자
    public delegate void Set_Move();
    Set_Move Move;





    /*************** 게임 루프 ***************/

    /// <summary> PlayerBullet 생성 </summary>
    private void OnEnable() => Init();

    /// <summary> PlayerBullet 로직 </summary>
    void Update()
    {
        if (!Compare_isPlay()) return;

        Timing();
        Move();
        Compare_Out();
    }





    /*************** PlayerBullet 초기화 매커니즘 ***************/

    /// <summary> PlayerBullet 초기화 모음 </summary>
    void Init()
    {
        AttributeInit();
        TypeInit();
        LevelInit();

        transform.rotation = Quaternion.Euler(0, 0, 0);
    }

    /// <summary> PlayerBullet 속성 초기화 </summary>
    void AttributeInit()
    {
        // 1. Vector
        ShootVec = Vector2.up;

        // 2. Time
        fieldTime = 0.5f;

        // 3. etc.
        degreeZ = 0f;
        targetEnemy = null;
    }

    /// <summary> PlayerBullet 타입 초기화 </summary>
    void TypeInit()
    {
        switch (PBLevel)
        {
            case PBulletLv.Lv1: power = 3; break;
            case PBulletLv.Lv2: power = 4; break;
            case PBulletLv.Lv3: power = 5; break;
            case PBulletLv.LvMAX_A: power = 8; break;
            case PBulletLv.LvMAX_B: power = 4; break;
            case PBulletLv.LvMAX_C: power = 5; break;
        }
    }

    /// <summary> PlayerBullet 레벨 초기화 </summary>
    void LevelInit()
    {
        switch (PBType)
        {
            case PBulletType.Bullet:
                MoveVec = Vector2.up * 12f;
                Move = Move_Str;
                break;
            case PBulletType.Accel:
                MoveVec = Vector2.up * 15f;
                Move = Move_Acc;
                break;
            case PBulletType.Homing:
                MoveVec = Vector2.up * 8f;
                Move = Move_Hom;
                break;
        }
    }

    

    /*************** PlayerBullet 동작 로직 ***************/

    /// <summary> 게임 진행 유무 검사 </summary>
    bool Compare_isPlay()
    {
        // 스테이지 일시정지 및 종료 시 PlayerBullet 로직 미실행
        if (StageManager.stageState == StageState.End) return false;
        if (StageManager.stageState == StageState.Pause) return false;
        if (LobbyManager.menuSelected == MenuSelected.Main)
        {
            gameObject.SetActive(false); return false;
        }

        return true;
    }

    /// <summary> 시간 측정 </summary>
    void Timing() => fieldTime += Time.deltaTime;

    /// <summary> PlayerBullet 필드 이탈 검사 </summary>
    void Compare_Out()
    {
        if (transform.position.y > GameManager.instance.transform.position.y + 8f)
        gameObject.SetActive(false);
    }





    /*************** EnemyBullet 타입 별 이동 모음 ***************/

    /// <summary> 직진 이동 </summary>
    void Move_Str() => transform.Translate(MoveVec * Time.deltaTime);
    /// <summary> 가속 이동 </summary>
    void Move_Acc() => transform.Translate(MoveVec * Time.deltaTime * fieldTime);
    /// <summary> Target 위치 추적 이동 </summary>
    void Move_Hom()
    {
        Compare_Target();
        SelectTarget();
        Homing();

        transform.Translate(MoveVec * Time.deltaTime);
    }





    /*************** EnemyBullet 이동 관련 모음 ***************/

    /// <summary> Target 검사 </summary>
    void Compare_Target()
    {
        // Target 미존재 시 미실행
        if (targetEnemy == null) return;

        // Target 비활성화 시 Target 초기화
        if (targetEnemy.gameObject.activeSelf == false) targetEnemy = null;
    }

    /// <summary> Target 설정 </summary>
    void SelectTarget()
    {
        // Target 존재 시 미실행
        if (targetEnemy != null) return;

        // PlayerBullet와 가장 가까운 Enemy를 타겟으로 지정
        float minDis = 100;

        foreach (var enemy in StageManager.EnemyList)
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

    /// <summary> Target 위치 추적에 따른 PlayerBullet 회전 </summary>
    void Homing()
    {
        // Target 미존재 시 미실행
        if (targetEnemy == null) return;

        Vector3 v1, v2, v3;

        v1 = (targetEnemy.transform.position - transform.position).normalized;

        float radian = Mathf.PI / 180 * 2;
        v2 = new Vector2(
            ShootVec.x * Mathf.Cos(radian) - ShootVec.y * Mathf.Sin(radian),
            ShootVec.x * Mathf.Sin(radian) + ShootVec.y * Mathf.Cos(radian));

        if (Vector2.Dot(ShootVec, v1) >= Vector2.Dot(ShootVec, v2))
        {
            degreeZ = Mathf.Atan2(v1.y, v1.x) / Mathf.PI * 180 - 90;
            ShootVec = v1;
        }
        else
        {
            v3 = new Vector2(
                ShootVec.x * Mathf.Cos(radian) + ShootVec.y * Mathf.Sin(radian),
                -ShootVec.x * Mathf.Sin(radian) + ShootVec.y * Mathf.Cos(radian));

            Vector3 pv = targetEnemy.transform.position - transform.position;

            if (Vector2.Dot(pv, v2) >= Vector2.Dot(pv, v3))
            {
                degreeZ = Mathf.Atan2(v2.y, v2.x) / Mathf.PI * 180 - 90;
                ShootVec = v2;
            }
            else
            {
                degreeZ = Mathf.Atan2(v3.y, v3.x) / Mathf.PI * 180 - 90;
                ShootVec = v3;
            }
        }

        transform.rotation = Quaternion.Euler(0, 0, degreeZ);
    }





    /*************** PlayerBullet 충돌 처리 로직 ***************/

    /// <summary> 충돌 처리 </summary>
    private void OnTriggerEnter2D(Collider2D collision)
    {
        // 필드 이탈 시 PlayerBullet 비활성화
        if (collision.CompareTag("Field"))
        {
            gameObject.SetActive(false);
        }
    }
}
