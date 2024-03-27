using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Enemy;

public class Enemy : MonoBehaviour
{
    public PoolManager pool;

    public Queue<BulletPattern> bulletPatterns = new Queue<BulletPattern>();

    // 적 타입, 체력, 출현 시간, 공격 쿨타임
    public EnemyState enemyState;
    public EnemyType enemyType;
    public MovingType movingType;
    public BulletPattern bulletPattern;
    public string getPatternType;
    public Vector3 playerPos;
    public Vector2 moveVec;
    public Vector2 moveDesVec;
    public Vector2 moveExitVec;
    public float speed;
    public float movSpeed;
    public float bulletSpeed;
    public string getBulletName;
    public string getmovingType;
    public string getDropItemName;
    int setScore;

    public string getBulletType;

    public int Health;

    // 적 공격
    float fieldTime = 0;
    float IdleTime = 0;
    float shootTime = 0.1f;
    public float firstWaitTime;
    public float waitTime;
    int shootCount = 0;
    public int shootLimit;
    public float fieldTimeLimit;

    // 적기 코드
    float bulletFromCode = 0;
    public float degreeZ = 0f;
    float degree = 0f;

    // 적 타입
    public enum EnemyState { Idle, Play, Wait, Exit, Dead }
    public enum EnemyType { Small, Medium, Large, Big }
    public enum MovingType { Straight, Accel, SlowDown }

    public enum BulletPattern
    {
        Straight, n_Way, Circle, Spread, Spread_Random,
        None
    }

    

    private void Awake()
    {
        // 적기 코드 생성 후 기록
        bulletFromCode = Random.Range(0f, 1f) * Random.Range(0f, 1f);
        GameManager.EnemyCode.Add(bulletFromCode);
    }

    private void OnEnable()
    {
        enemyState = EnemyState.Idle;
        fieldTime = 0;
        IdleTime = 0;
        shootTime = 0.1f;
        shootCount = 0;

        switch (enemyType)
        {
            case EnemyType.Small:   Health = 3;     setScore = 100; break;
            case EnemyType.Medium:  Health = 40;    setScore = 500; break;
            case EnemyType.Large:   Health = 180;   setScore = 5000; break;
            case EnemyType.Big:     Health = 350;   setScore = 20000; break;
        }
    }

    public void Init()
    {
        switch (getPatternType)
        {
            case "str": bulletPattern = BulletPattern.Straight; break;
            case "way": bulletPattern = BulletPattern.n_Way; break;
            case "cir": bulletPattern = BulletPattern.Circle; break;
            case "spr": bulletPattern = BulletPattern.Spread; break;
            case "sprR": bulletPattern = BulletPattern.Spread_Random; break;
            case "none": bulletPattern = BulletPattern.None; break;
        }
        switch (getmovingType)
        {
            case "str": movingType = MovingType.Straight; break;
            case "acc": movingType = MovingType.Accel; break;
            case "slow": movingType = MovingType.SlowDown; break;
        }
        bulletPatterns.Clear();
        bulletPatterns.Enqueue(bulletPattern);
    }

    private void Start()
    {
        GetDegree();
    }

    void Update()
    {
        fieldTime += Time.deltaTime;
        IdleTime += Time.deltaTime;

        if (enemyState == EnemyState.Idle && IdleTime >= firstWaitTime)
        {
            enemyState = EnemyState.Play;
            IdleTime = 0f;
        }

        if (enemyState == EnemyState.Play)
        {
            if (IdleTime >= shootTime)
            {
                switch (bulletPattern)
                {
                    case BulletPattern.Straight: Straight(false); break;
                    case BulletPattern.n_Way: n_Way(true); break;
                    case BulletPattern.Circle: Circle(true); break;
                    case BulletPattern.Spread: Spread(true); break;
                    case BulletPattern.Spread_Random: Spread_Random(180); break;
                }

                //if (bulletPattern != BulletPattern.None)
                shootCount++;
            }
        }
        if (fieldTime >= fieldTimeLimit)
        {
            fieldTime = 0;
            enemyState = EnemyState.Exit;
            movingType = MovingType.Accel;
        }
        Moving();
        

        WaitCompare();
    }

    void Moving()
    {
        switch (movingType)
        {
            case MovingType.Straight:
                transform.Translate(moveVec * movSpeed * Time.deltaTime); break;
            case MovingType.Accel:
                if (enemyState == EnemyState.Exit)
                {
                    Debug.Log(moveExitVec);
                    transform.Translate(moveExitVec * movSpeed * Time.deltaTime * fieldTime * 2); break;
                }
                transform.Translate(moveVec * movSpeed * Time.deltaTime * fieldTime * 2); break;
            case MovingType.SlowDown:
                if (fieldTime < 1)
                transform.position = Vector2.Lerp(transform.position, moveDesVec, 0.03f);
                else movingType = MovingType.Straight; break;
        }
        transform.rotation = Quaternion.Euler(0, 0, degreeZ);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // 플레이어 공격 충돌 시 체력 감소
        switch (collision.tag)
        {
            case "PlayerBullet_Lv1":    Health -= 3;    collision.gameObject.SetActive(false); break;
            case "PlayerBullet_Lv2":    Health -= 4;    collision.gameObject.SetActive(false); break;
            case "PlayerBullet_Lv3":    Health -= 5;    collision.gameObject.SetActive(false); break;
            case "PlayerBullet_LvMAX_A":Health -= 10;   collision.gameObject.SetActive(false); break;
            case "PlayerBullet_LvMAX_B": Health -= 4;   collision.gameObject.SetActive(false); break;
            case "PlayerBullet_LvMAX_C": Health -= 6;   collision.gameObject.SetActive(false); break;
        }

        // 적기 파괴
        if (Health <= 0)
        {
            enemyState = EnemyState.Dead;

            GameObject item;
            switch (getDropItemName)
            {
                case "silver": item = pool.MakeObject("SilverCoin"); break;
                case "gold": item = pool.MakeObject("GoldCoin"); break;
                case "pow": item = pool.MakeObject("PowerUp"); break;
                case "hp":  item = pool.MakeObject("Heal"); break;
                default:    item = null; break;
            }
            if(item != null) item.transform.position = transform.position;
            Dead();
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Field_Out") && enemyState != EnemyState.Dead)
        {
            Dead();
        }
    }


    void WaitCompare()
    {
        if (enemyState == EnemyState.Wait && IdleTime >= waitTime)
        {
            enemyState = EnemyState.Play; IdleTime = 0f;
            bulletPattern = bulletPatterns.Dequeue();
        }
        if (enemyState == EnemyState.Play && shootCount >= shootLimit)
        {
            enemyState = EnemyState.Wait; shootCount = 0;
            bulletPatterns.Enqueue(bulletPattern);
        }
    }

    public void Dead()
    {
        if (Health <= 0) GameManager.Score += setScore;
        gameObject.SetActive(false);

        List<float> temp = new List<float>();

        foreach (var code in GameManager.EnemyCode)
        {
            if (code == bulletFromCode) temp.Add(bulletFromCode);
        }


        /*foreach (var compareBullet in pool.poolEBullet_SP1)
        {
            EnemyBullet compareBulletFrom = compareBullet.GetComponent<EnemyBullet>();

            if (compareBullet.activeSelf && compareBulletFrom.enemyFromCode == bulletFromCode)
            {
                compareBullet.SetActive(false);
                if (compareBulletFrom.enemyFromCode == bulletFromCode)
                {
                    GameObject coin = pool.MakeObject("SilverCoin");
                    coin.transform.position = compareBullet.transform.position;
                    coin.SetActive(true);
                }
            }
        }*/
        GameManager.EnemyList.Remove(gameObject);

        foreach (var code in temp)
        {
            GameManager.EnemyCode.Remove(code);
        }

        GameObject Explosion = pool.MakeObject("EDestroy");
        Explosion.transform.position = transform.position;
        Explosion.SetActive(true);
    }

    void GetDegree()
    {
        playerPos = GameManager.playerPos;
        degree = Mathf.Atan2
                (playerPos.y - transform.position.y, playerPos.x - transform.position.x)
                / Mathf.PI * 180 + 90;
    }

    private void Fire(float deg)
    {
        GameObject bullet = pool.MakeObject(getBulletName);
        bullet.transform.position = transform.position;
        bullet.transform.rotation = Quaternion.Euler(0, 0, deg);


        EnemyBullet bulletFrom = bullet.GetComponent<EnemyBullet>();
        //bulletFrom.enemyFromCode = bulletFromCode;
        bulletFrom.getBulletType = getBulletType;
        bulletFrom.speed = bulletSpeed;
        bulletFrom.Init();

        IdleTime = 0;
    }


    // 공격 패턴 모음
    private void Straight(bool isLock)
    {
        if (!isLock || shootCount == 0) GetDegree();
        Fire(degree);
    }

    private void n_Way(bool isLock)
    {
        if (!isLock || shootCount == 0) GetDegree();

        int n_Count = shootLimit;
        float degEach = 16f;
        float setDeg = degree - degEach * (n_Count - 1) / 2;

        for (int i = 0; i < n_Count; i++)
        {
            Fire(setDeg);
            setDeg += degEach;
        }
    }

    private void Circle(bool isLock)
    {
        if (!isLock || shootCount == 0) GetDegree();

        int n_Count = 20;
        float degEach = 360 / n_Count;
        float setDeg = degree;

        for (int i = 0; i < n_Count; i++)
        {
            Fire(setDeg);
            setDeg += degEach;
        }
    }

    //역삼각형 모양처럼 흩뿌리기
    private void Spread(bool isLock)
    {
        if (!isLock || shootCount == 0) GetDegree();

        float degEach = 10f;

        for (int i = 0; i < 2; i++)
        {
            float setDeg = (i == 0) ? 
                degree - degEach * (shootCount % 4) : degree + degEach * (shootCount % 4);
            Fire(setDeg);
            if (shootCount % 4 == 0) break;
        }
    }

    private void Spread_Random(float degLimit)
    {
        int n_Count = 10;

        for (int i = 0; i < n_Count; i++)
        {
            float setDeg = degree + Random.Range(-degLimit / 2, degLimit / 2);
            Fire(setDeg);
        }
    }
}
