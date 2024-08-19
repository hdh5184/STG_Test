using System.Collections;
using UnityEngine;
using static StageManager;
using static Logic_Enemy;
using static Init_Enemy;

/* <Enemy 클래스> */
public class Enemy : MonoBehaviour
{
    // 1. 매니저
    public PoolManager pool;
    public AudioManager audioManager;

    // 2. 플레이어 참조
    public Player player;
    public Vector3 playerPos;

    // 3. Enemy 상태 및 타입
    public EnemyState enemyState;

    public MoveType moveType;
    public AttackType attackType;

    // 4. case 데이터
    public string getDropItemName;
    public string getMovingType;
    public string getBulletType;
    public string getBulletName;
    public string getPatternType;

    // 5. 이동 및 필드
    public Vector2 moveVec;
    public Vector2 moveDesVec;
    public Vector2 moveExitVec;
    public float degreeZ = 0f;
    public float movSpeed;

    // 6. 탄
    public float bulletSpeed;
    public float degree = 0f;
    public int shootLimit;
    public bool isLock;

    // 7. 기준 시간
    public float firstWaitTime;
    public float waitTime;
    public float fieldTimeLimit;
    public float shootTime = 0.1f;

    // 8-1. Enemy 고정 속성
    public Transform[] shootPos;
    public int setScore;
    public int setHealth;
    public bool isBoss;

    // 8-2. Enemy 변동 속성
    public float[] shootPos_Degree;
    public bool[] shootPos_isShoot;
    public int Health;
    
    // 9. 시간, 카운트 갱신
    public float fieldTime = 0;
    public float attackTime = 0;
    public float idleTime = 0;
    
    public int shootCount = 0;
    


    // d. 대리자
    public delegate void Set_Attack(ref AttackData data);
    public delegate void Set_Move(ref MoveData data);
    public delegate void Set_Act();
    protected Set_Attack set_Atk;
    protected Set_Move set_Mov;
    protected Set_Act set_Act;

    // e. 속성 모음
    public enum EnemyState { Idle, Play, Wait, Exit, Dead }
    public enum MoveType { Straight, Accel, SlowDown, None }
    public enum AttackType { Straight, n_Way, Circle, Spread, Spread_Random, Vortex, Down, None }

    // s. 데이터 구조체
    public struct MoveData
    {
        public Transform transform;
        public Vector2 moveVec;
        public float movSpeed;
        public float fieldTime;
    }

    public struct AttackData
    {
        public PoolManager pool;
        public Vector2 firePos;
        public string getBulletName;
        public string getBulletType;
        public int shootCount;
        public int shootLimit;
        public float bulletSpeed;
        public float degree;
        public float degreeLimit;
    }

    public MoveData moveData;
    public AttackData attackData;





    /*************** 게임 루프 ***************/

    /// <summary> Enemy 초기 설정 </summary>
    protected virtual void Awake()
    {
        pool = PoolManager.instance;
        audioManager = AudioManager.instance;
        isBoss = false;
    }

    /// <summary> Enemy 생성 </summary>
    protected virtual void OnEnable()
    {
        Init_Attribute();
        Init_Type();
        Init_Coroutine();
        Init_Data();
    }

    /// <summary> Enemy 로직 실행 </summary>
    protected virtual void Update()
    {
        Timing();
        Move();
        Attack();
        Act();
        CompareExit();
    }





    /*************** 초기화 모음 ***************/

    /// <summary> 기본 속성 초기화 </summary>
    private void Init_Attribute()
    {
        // 1. State
        enemyState = EnemyState.Idle;

        // 2. Time
        fieldTime = 0;
        attackTime = 0;
        idleTime = 0;
        fieldTimeLimit += firstWaitTime;

        // 3. isLock
        switch (getPatternType)
        {
            case "way": case "cir": case "spr":
            isLock = true; break;

            default:
            isLock = false; break;
        }

        // 4. delegate
        set_Atk = null;
        set_Mov = null;
        set_Act = FirstIdle;

        // @. etc.
        Health = setHealth;
        shootCount = 0;
        degree = 0;
    }

    /// <summary> Enemy 내 타입 초기화 </summary>
    public void Init_Type()
    {
        Init_Pattern(this, getPatternType);
        Init_Moving(this, getMovingType);
        Change_Attack(attackType);
        Change_Move(moveType);
    }

    /// <summary> Enemy 내 Coroutine 초기화 </summary>
    public void Init_Coroutine()
    {
        if (moveType == MoveType.SlowDown)
        StartCoroutine(SlowToStr());
    }

    /// <summary> Enemy 내 데이터 초기화 </summary>
    public void Init_Data()
    {
        moveData = new MoveData();
        attackData = new AttackData();
        Init_MoveData(this, ref moveData);
        Init_AttackData(this, ref attackData);
    }





    /*************** 초기 로직 모음 ***************/

    protected IEnumerator SlowToStr()
    {
        yield return new WaitForSeconds(1f);
        moveData.moveVec = moveVec;
        Change_Move(MoveType.Straight);
    }

    protected virtual void FirstIdle()
    {
        if (fieldTime < firstWaitTime) return;

        enemyState = EnemyState.Play;
        set_Act = CompareWait;
    }





    /*************** 로직 모음 ***************/

    /// <summary> 시간 갱신 </summary>
    protected void Timing()
    {
        fieldTime += Time.deltaTime;    // 필드 내 출현 시간

        if (enemyState == EnemyState.Wait)
        idleTime += Time.deltaTime;

        if (enemyState == EnemyState.Play)
        attackTime += Time.deltaTime;     // 대기 시간
    }

    /// <summary> 이동 로직 </summary>
    protected void Move()
    {
        if (enemyState == EnemyState.Dead) return;
        moveData.fieldTime = fieldTime;
        set_Mov(ref moveData);
    }

    /// <summary> 공격 로직 </summary>
    protected void Attack()
    {
        if (enemyState != EnemyState.Play) return;
        if (attackTime < shootTime) return;

        for (int i = 0; i < shootPos.Length; i++)
        {
            if (!shootPos_isShoot[i]) continue;
            attackData.firePos = shootPos[i].position;
            attackData.shootCount = shootCount;

            if (!Compare_DegreeLock())
            {
                shootPos_Degree[i] = SetDegree(attackData.firePos);
            }

            attackData.degree = shootPos_Degree[i];
            set_Atk(ref attackData);
        }

        shootCount++;
        attackTime = 0;
    }

    /// <summary> 행동 로직 </summary>
    protected void Act() => set_Act();

    /// <summary> 필드 탈출 검사 로직 </summary>
    protected void CompareExit()
    {
        if (enemyState == EnemyState.Exit) return;
        if (fieldTime < fieldTimeLimit) return;

        enemyState = EnemyState.Exit;
        fieldTime = 0;
        moveData.fieldTime = 0;
        moveData.moveVec = moveExitVec;
        Change_Move(MoveType.Accel);

        set_Act = Act_None;
    }





    /*************** 공격 속성 갱신 모음 ***************/

    /// <summary> 공격 기준 각 변동 유무 확인 </summary>
    bool Compare_DegreeLock()
    {
        if (!isLock)            return false;

        if (shootCount == 0)    return false;
        else                    return true;
    }





    /*************** 대리자 갱신 모음 ***************/

    /// <summary> 이동 대리자 갱신 </summary>
    protected void Change_Move(MoveType type)
    { moveType = type; set_Mov = SetSwitch_Move(moveType); }

    /// <summary> 공격 대리자 갱신 </summary>
    protected void Change_Attack(AttackType type)
    { attackType = type; set_Atk = SetSwitch_Attack(attackType); }





    /*************** 행동 갱신 모음 ***************/

    /// <summary> 행동 : 공격 중지 검사 </summary>
    protected virtual void CompareWait()
    {
        if (shootCount < shootLimit) return;

        enemyState = EnemyState.Wait;
        shootCount = 0;
        attackTime = 0;
        Init_AttackData(this, ref attackData);

        set_Act = ComparePlay;
    }

    /// <summary> 행동 : 공격 재개 검사 </summary>
    protected void ComparePlay()
    {
        if (idleTime < waitTime) return;

        enemyState = EnemyState.Play;
        idleTime = 0;

        set_Act = CompareWait;
    }

    /// <summary> 행동 없음 </summary>
    protected void Act_None() { }





    /*************** 기체 충돌 로직 모음 ***************/

    /// <summary> 충돌 </summary>
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (enemyState == EnemyState.Dead) return;

        if (collision.tag == "PlayerBullet")
        {
            Health -= collision.GetComponent<PlayerBullet>().power;
            collision.gameObject.SetActive(false);
        }

        if (Health <= 0) Dead();
    }

    /// <summary> 필드 이탈 </summary>
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Field_Out") && enemyState != EnemyState.Dead)
        {
            EnemyList.Remove(gameObject);
            gameObject.SetActive(false);
        }
    }





    /*************** 기체 파괴 로직 모음 ***************/

    /// <summary> 기체 파괴 </summary>
    protected virtual void Dead()
    {
        DropItem();

        enemyState = EnemyState.Dead;
        score += setScore;
        EnemyList.Remove(gameObject);
        gameObject.SetActive(false);
    }

    /// <summary> 기체 전리품 떨구기 </summary>
    protected void DropItem(Vector3? dropPos = null)
    {
        GameObject item = null;

        switch (getDropItemName)
        {
            case "silver":  item = pool.MakeObject("SilverCoin"); break;
            case "gold":    item = pool.MakeObject("GoldCoin"); break;
            case "pow":     item = pool.MakeObject("PowerUp"); break;
            case "hp":      item = pool.MakeObject("Heal"); break;

            default:        return;
        }

        item.transform.position = dropPos ?? transform.position;
    }
}