using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using static Logic_Enemy;
using static StageManager;
using static Spawn;

public class Enemy : MonoBehaviour
{
    // 1. 매니저
    public PoolManager pool;
    public AudioManager audioManager;

    // 3. 플레이어 참조
    public Player player;
    public Vector3 playerPos; // Player 컴포넌트에서 참조 가능하다고 판단하여 삭제 가능성 있음

    // 4. Enemy 및 탄 속성
    public EnemyState enemyState;

    // 부모 클래스에서 초기 설정에 사용될 예정
    public MoveType moveType;
    public AttackType attackType;

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
    public float attackTime = 0;
    public float shootTime = 0.1f;
    public int shootCount = 0;
    public bool isLock;

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

    

    public Spawn spawnData;
    public Spawn_Boss spawnBossData;

    public Vector2[] shootPos;


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


    /// <summary> Enemy 초기 설정 </summary>
    protected virtual void Awake()
    {
        pool = PoolManager.instance;
        audioManager = AudioManager.instance;
    }

    /// <summary> Enemy 생성 </summary>
    protected virtual void OnEnable()
    {
        Init_Attribute();
        Init_Type();
        Init_Coroutine();
    }

    protected virtual void Update()
    {
        Timing();
        Move();
        Attack();
        Act();
    }

    

    private void Init_Attribute()
    {
        // 2. State
        enemyState = EnemyState.Idle;

        // 3. Time
        fieldTime = 0;
        attackTime = 0;
        shootTime = 0.1f;

        // 4. etc.
        shootCount = 0;
        degree = 0;
        isLock = false;

        shootPos = new Vector2[1];

        set_Atk = null;
        set_Mov = null;
    }

    public void Init_Type()
    {
        Init_Pattern(this, getPatternType);
        Init_Moving(this, getMovingType);
    }

    public void Init_Coroutine()
    {
        StartCoroutine(Change_EnemyState(EnemyState.Play, firstWaitTime));
        StartCoroutine(Change_EnemyState(EnemyState.Exit, firstWaitTime + fieldTimeLimit));
    }

    protected Vector2 Init_ShootPos(GameObject obj = null)
    {
        if (obj == null)    return transform.position;
        else                return obj.transform.position;
    }

    protected void Timing()
    {
        fieldTime += Time.deltaTime;    // 필드 내 출현 시간

        if (enemyState == EnemyState.Play)
        attackTime += Time.deltaTime;     // 대기 시간
    }

    protected void Move()
    {
        set_Mov(ref moveData);
    }

    protected void Attack()
    {
        if (enemyState != EnemyState.Play) return;

        for (int i = 0; i < shootPos.Length; i++)
        {
            attackData.firePos = shootPos[i];

            if (!Compare_DegreeLock())
            attackData.degree = SetDegree(attackData.firePos);

            set_Atk(ref attackData);
        }

        attackTime = 0;
    }

    bool Compare_DegreeLock()
    {
        if (!isLock)            return false;

        if (shootCount == 0)    return false;
        else                    return true;
    }

    protected void Act()
    {
        if (shootCount >= shootLimit)
        {
            enemyState = EnemyState.Wait;
            shootCount = 0;
            attackTime = 0;

            StartCoroutine(Change_EnemyState(EnemyState.Play, waitTime));


            /*
            if (enemyType == EnemyType.Boss)
                setBossLogic(bossLogics.Dequeue());
            else bulletPatterns.Enqueue(bulletPattern);
            */
        }



        /*

        if (IdleTime >= waitTime)
        {
            enemyState = EnemyState.Play; IdleTime = 0f;
            if (enemyType != EnemyType.Boss)
                bulletPattern = bulletPatterns.Dequeue();
        }
        */
    }

    protected void Change_Attack(AttackType type)
    {
        attackType = type;
        SetSwitch_Attack(attackType);
    }


    protected IEnumerator Change_Move(MoveType type, float time)
    {
        yield return new WaitForSeconds(time);

        moveType = type;
        SetSwitch_Move(moveType);
    }
    

    protected IEnumerator Change_EnemyState(EnemyState type, float time)
    {
        yield return new WaitForSeconds(time);

        enemyState = type;

        if (enemyState == EnemyState.Exit)
        StopAllCoroutines();
    }








    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (enemyState == EnemyState.Dead) return;

        if (collision.tag == "PlayerBullet")
        {
            Health -= collision.GetComponent<PlayerBullet>().power;
            collision.gameObject.SetActive(false);
        }

        if (Health <= 0)
        {
            GameObject item;
            switch (getDropItemName)
            {
                case "silver": item = pool.MakeObject("SilverCoin"); break;
                case "gold": item = pool.MakeObject("GoldCoin"); break;
                case "pow": item = pool.MakeObject("PowerUp"); break;
                case "hp": item = pool.MakeObject("Heal"); break;
                default: item = null; break;
            }

            if (item != null) item.transform.position = transform.position;
            Dead();
        }
    }

    /// <summary> 기체 파괴 </summary>
    protected virtual void Dead()
    {
        enemyState = EnemyState.Dead;
        Score += setScore;
        EnemyList.Remove(gameObject);

        /*
        switch (enemyType)
        {
            case EnemyType.Small:
            case EnemyType.SubWeapon:
            case EnemyType.Medium: Explosion("ExplodeB", "EShotL"); break;
            case EnemyType.Large: Explosion("ExplodeC", "Explode"); break;
            case EnemyType.Big: StartCoroutine("MidBossDead"); return;
            case EnemyType.Boss: StartCoroutine("BossDead"); return;
        }
        */

        gameObject.SetActive(false);
    }

    /// <summary> 폭발 이펙트 출현 </summary>
    protected virtual GameObject Explosion(string obj, string audio)
    {
        GameObject explosion = pool.MakeObject(obj, false);

        explosion.transform.position = transform.position;
        explosion.GetComponent<Effect>().audio.clip =
            audioManager.getAudioClip(audio);
        explosion.SetActive(true);

        return explosion;
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Field_Out") && enemyState != EnemyState.Dead)
        {
            EnemyList.Remove(gameObject);
            gameObject.SetActive(false);
        }
    }
}