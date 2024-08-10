using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static LobbyManager;
using static StageManager;

public class EnemyBullet : MonoBehaviour
{
    // 1. 타입
    public string getBulletType;

    // 2. 참조 데이터
    Vector3 playerPos;

    // 3. 속성
    Vector2 MoveVec;
    Vector3 ShootVec = Vector2.down;
    public float speed = 0f;
    float degreeZ = 0f;

    // 4. 시간
    float fieldTime = 0f;

    // @. 대리자
    delegate void Move_Set();
    Move_Set Move;





    /*************** 게임 루프 ***************/

    /// <summary> EnemyBullet 생성 </summary>
    private void OnEnable()
    {
        transform.rotation = Quaternion.Euler(0, 0, 0);
    }

    /// <summary> EnemyBullet 로직 </summary>
    void Update()
    {
        if (!Compare_isPlay()) return;

        Timing();
        Move();
    }





    /*************** EnemyBullet 초기화 매커니즘 ***************/

    /// <summary> EnemyBullet 속성 지정 </summary>
    public void Set_Attribute(string bulletType, float fireSpeed)
    {
        getBulletType = bulletType;
        speed = fireSpeed;
    }

    /// <summary> EnemyBullet 초기화 모음 </summary>
    public void Init()
    {
        AttributeInit();
        TypeInit();
    }

    /// <summary> EnemyBullet 속성 초기화 </summary>
    void AttributeInit()
    {
        // 1. Vector
        ShootVec = Vector2.down;

        // 2. Time
        fieldTime = 0.5f;

        // 3. etc.
        degreeZ = 0f;
    }

    /// <summary> EnemyBullet 타입 초기화 </summary>
    void TypeInit()
    {
        switch (getBulletType)
        {
            case "str": Move = Move_Str; MoveVec = Vector2.down * speed; break;
            case "acc": Move = Move_Acc; MoveVec = Vector2.down * speed; break;
            case "hom": Move = Move_Hom; MoveVec = Vector2.down * 5; break;
        }
    }





    /*************** EnemyBullet 동작 로직 ***************/

    /// <summary> 게임 진행 유무 검사 </summary>
    bool Compare_isPlay()
    {
        // 스테이지 일시정지 및 종료 시 EnemyBullet 로직 미실행
        if (StageManager.stageState == StageState.End)      return false;
        if (StageManager.stageState == StageState.Pause)    return false;
        if (LobbyManager.menuSelected == MenuSelected.Main)
        {
            gameObject.SetActive(false);                    return false;
        }

        return true;
    }

    /// <summary> 시간 측정 </summary>
    void Timing() => fieldTime += Time.deltaTime;





    /*************** EnemyBullet 타입 별 이동 모음 ***************/

    /// <summary> 직진 이동 </summary>
    void Move_Str() => transform.Translate(MoveVec * Time.deltaTime);
    /// <summary> 가속 이동 </summary>
    void Move_Acc() => transform.Translate(MoveVec * Time.deltaTime * fieldTime);
    /// <summary> Player 위치 추적 이동 </summary>
    void Move_Hom()
    {
        playerPos = StageManager.playerPos;

        Vector3 v1, v2, v3;

        v1 = (playerPos - transform.position).normalized;

        float radian = Mathf.PI / 180 * 0.5f;
        v2 = new Vector2(
            ShootVec.x * Mathf.Cos(radian) - ShootVec.y * Mathf.Sin(radian),
            ShootVec.x * Mathf.Sin(radian) + ShootVec.y * Mathf.Cos(radian));

        if (Vector2.Dot(ShootVec, v1) >= Vector2.Dot(ShootVec, v2))
        {
            degreeZ = Mathf.Atan2(v1.y, v1.x) / Mathf.PI * 180 + 90;
            ShootVec = v1;
        }
        else
        {
            v3 = new Vector2(
                ShootVec.x * Mathf.Cos(radian) + ShootVec.y * Mathf.Sin(radian),
                -ShootVec.x * Mathf.Sin(radian) + ShootVec.y * Mathf.Cos(radian));

            Vector3 pv = playerPos - transform.position;

            if (Vector2.Dot(pv, v2) >= Vector2.Dot(pv, v3))
            {
                degreeZ = Mathf.Atan2(v2.y, v2.x) / Mathf.PI * 180 + 90;
                ShootVec = v2;
            }
            else
            {
                degreeZ = Mathf.Atan2(v3.y, v3.x) / Mathf.PI * 180 + 90;
                ShootVec = v3;
            }
        }

        transform.rotation = Quaternion.Euler(0, 0, degreeZ);
        transform.Translate(MoveVec * Time.deltaTime);
    }





    /*************** EnemyBullet 충돌 처리 로직 ***************/

    /// <summary> 충돌 처리 </summary>
    private void OnTriggerEnter2D(Collider2D collision)
    {
        // 필드 이탈 시 EnemyBullet 비활성화
        if (collision.CompareTag("Field"))
        gameObject.SetActive(false);
    }
}
