using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using static Enemy;
using static Enemy_pre;
using static UnityEditor.PlayerSettings;

public class Enemy_pre : MonoBehaviour
{
    // 1. 매니저
    public PoolManager pool;
    public AudioManager audioManager;
    //public Logic_Enemy logic;

    // 2. 패턴
    // 자식 클래스로 분류 예정
    //public Queue<BulletPattern> bulletPatterns = new Queue<BulletPattern>();

    // 3. 플레이어 참조
    public Player player;
    public Vector3 playerPos; // Player 컴포넌트에서 참조 가능하다고 판단하여 삭제 가능성 있음

    // 4. Enemy 및 탄 속성
    public EnemyState enemyState;

    // 부모 클래스에서 초기 설정에 사용될 예정
    public MoveType moveType;
    public AttackType bulletPattern;

    // 5. case 데이터
    public string getDropItemName;
    public string getMovingType;
    public string getBulletType;
    public string getBulletName;
    public string getPatternType;

    // 6. 이동 및 필드 데이터
    public float degreeZ = 0f;
    public float movSpeed;
    public Vector2 moveVec, moveDesVec, moveExitVec;
    public float fieldTimeLimit;

    // 7. 탄 데이터
    public float bulletSpeed;
    public int shootLimit;

    // 8. Enemy 기체 및 필드 데이터
    public int setScore;
    public int Health;
    public float firstWaitTime, waitTime;

    // 9. 시간, 카운트
    public float fieldTime = 0;
    public float IdleTime = 0;
    public float shootTime = 0.1f;
    public int shootCount = 0;

    // 10. 탄도 각
    public float degree = 0f;

    // e. 속성 모음
    public enum EnemyState { Idle, Play, Wait, Exit, Dead }
    public enum MoveType { Straight, Accel, SlowDown, none }
    public enum AttackType { Straight, n_Way, Circle, Spread, Spread_Random, Vortex, Down, None }


    public delegate void Set_Act();
    public delegate void Set_Attack(ref AttackData data);
    public delegate void Set_Move(ref MoveData data);
    Set_Act set_Act;
    Set_Attack set_Atk;
    Set_Move set_Mov;

    public Vector2[] shootPos;

    


    public struct MoveData
    {
        public Transform transform;
        public Vector2 moveVec;
        public Vector2 moveDesVec;
        public Vector2 moveExitVec;
        public float movSpeed;
        public float fieldTime;
    }

    public struct AttackData
    {
        public PoolManager pool;
        public Vector2 firePos;
        public string getBulletName;
        public string getBulletType;
        public float bulletSpeed;
        public float degree;
        public float degreeLimit;
    }

    MoveData moveData;
    AttackData attackData;


    /// <summary> Enemy 초기 설정 </summary>
    protected virtual void Awake()
    {
        pool = PoolManager.instance;
        audioManager = AudioManager.instance;
        //logic = Logic_Enemy.instance;
    }

    /// <summary> Enemy 생성 </summary>
    protected virtual void OnEnable()
    {
        AttributeInit();
    }

    protected virtual void Update()
    {
        
    }

    private void AttributeInit()
    {
        // 1. Queue Data
        //bulletPatterns.Clear();
        //bossLogicsFinal.Clear();

        // 2. State
        enemyState = EnemyState.Idle;

        // 3. Time
        fieldTime = 0;
        IdleTime = 0;
        shootTime = 0.1f;

        // 4. etc.
        shootCount = 0;
        degree = 0;

        set_Atk = null;
        set_Mov = null;
    }

    protected void Move()
    {
        set_Mov(ref moveData);
    }

    protected void Attack()
    {
        attackData.degree = Logic_Enemy.SetDegree(attackData.firePos);
        set_Atk(ref attackData);
    }

    protected void Act()
    {
        
    }

    protected void Change_Attack(AttackType type)
    {
        //bulletPattern = type;
        //Logic_Enemy.SetSwitch_Attack(bulletPattern);
    }


    protected IEnumerator Change_Move(MoveType type, float time)
    {
        yield return new WaitForSeconds(time);

        //moveType = type;
        //Logic_Enemy.SetSwitch_Move(moveType);
    }
    

    protected IEnumerator Change_EnemyState(EnemyState type, float time)
    {
        yield return new WaitForSeconds(time);

        enemyState = type;
    }

}