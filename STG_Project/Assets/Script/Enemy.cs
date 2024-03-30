using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using static Enemy;

public class Enemy : MonoBehaviour
{
    public PoolManager pool;
    public AudioManager audioManager;

    // 오디오 따로 클래스 생성
    //SpriteRenderer sr;

    public Queue<BulletPattern> bulletPatterns = new Queue<BulletPattern>();
    public Queue<BossLogic> bossLogics = new Queue<BossLogic>();
    public Queue<BossLogic> bossLogicsFinal = new Queue<BossLogic>();

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
    public string getMovingType;
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

    public float degreeZ = 0f;
    float degree = 0f;
    bool isBossFinal = false;

    bool shoot1, shoot2, shoot3, shoot4, shoot5;
    float degree1, degree2, degree3, degree4, degree5;
    int setBossPos = 0;

    // 적 타입
    public enum EnemyState { Idle, Play, Wait, Exit, Dead }
    public enum EnemyType { Small, Medium, Large, Big, Boss }
    public enum MovingType { Straight, Accel, SlowDown }
    public enum BulletPattern
    {
        Straight, n_Way, Circle, Spread, Spread_Random, Vortex, Down,
        None
    }

    public GameObject
        BossShootPos1, BossShootPos2, BossShootPos3,
        BossShootPos4, BossShootPos5;

    private void Awake()
    {
        //sr = GetComponent<SpriteRenderer>();
    }

    private void OnEnable()
    {
        //sr.enabled = true;
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
            case EnemyType.Boss:    Health = 1200;  setScore = 80000; break;
        }
        bulletPatterns.Clear();
    }

    public void Init()
    {
        switch (getPatternType)
        {
            case "str":     bulletPattern = BulletPattern.Straight; break;
            case "way":     bulletPattern = BulletPattern.n_Way; break;
            case "cir":     bulletPattern = BulletPattern.Circle; break;
            case "spr":     bulletPattern = BulletPattern.Spread; break;
            case "sprR":    bulletPattern = BulletPattern.Spread_Random; break;
            case "vtx":     bulletPattern = BulletPattern.Vortex; break;
            case "down":    bulletPattern = BulletPattern.Down; break;
            case "none":    bulletPattern = BulletPattern.None; break;
        }
        switch (getMovingType)
        {
            case "str":     movingType = MovingType.Straight; break;
            case "acc":     movingType = MovingType.Accel; break;
            case "slow":    movingType = MovingType.SlowDown; break;
        }
        if (enemyType != EnemyType.Boss)
            bulletPatterns.Enqueue(bulletPattern);
    }

    private void Start()
    {
        GetDegree(null);
    }

    void Update()
    {
        if (enemyState == EnemyState.Dead) return;

        fieldTime += Time.deltaTime;
        IdleTime += Time.deltaTime;

        if (enemyState == EnemyState.Idle && IdleTime >= firstWaitTime)
        {
            enemyState = EnemyState.Play; IdleTime = 0f;
        }

        if (!isBossFinal && fieldTime >= fieldTimeLimit && enemyType == EnemyType.Boss)
        {
            isBossFinal = true;
            setBossLogic(bossLogicsFinal.Peek());
            bossLogics.Clear();
            bossLogics = bossLogicsFinal;
        }

        if (enemyState == EnemyState.Play && IdleTime >= shootTime)
        {
            if (enemyType == EnemyType.Boss) BossPattern();
            else
            {
                SelectPattern(null, 0);
                shootCount++;
            }
            
        }
        
        Moving();
        WaitCompare();
        ExitCompare();
    }

    void SelectPattern(GameObject shootPos, int pos)
    {
        if (enemyType == EnemyType.Boss) setBossPos = pos;

        switch (bulletPattern)
        {
            case BulletPattern.Straight: Straight(false, shootPos); break;
            case BulletPattern.n_Way: n_Way(true, shootPos); break;
            case BulletPattern.Circle: Circle(true, shootPos); break;
            case BulletPattern.Spread: Spread(true, shootPos); break;
            case BulletPattern.Spread_Random: Spread_Random(180, shootPos); break;
            case BulletPattern.Vortex: Vortex(); break;
            case BulletPattern.Down: Down(shootPos); break;
        }
    }

    void BossPattern()
    {
        if (shoot1) SelectPattern(BossShootPos1, 1);
        if (shoot2) SelectPattern(BossShootPos2, 2);
        if (shoot3) SelectPattern(BossShootPos3, 3);
        if (shoot4) SelectPattern(BossShootPos4, 4);
        if (shoot5) SelectPattern(BossShootPos5, 5);
        shootCount++;
    }

    void setBossLogic(BossLogic bossLogic)
    {
        waitTime = bossLogic.delay;
        // posX, posY
        // movX, movY
        // degreeZ,
        movSpeed = bossLogic.movSpeed;
        getMovingType = bossLogic.movingType;
        // movDesX, movDesY
        moveExitVec = new Vector2(bossLogic.movExitX, bossLogic.movExitY).normalized;

        shoot1 = (bossLogic.shootPos1 == "1") ? true : false;
        shoot2 = (bossLogic.shootPos2 == "1") ? true : false;
        shoot3 = (bossLogic.shootPos3 == "1") ? true : false;
        shoot4 = (bossLogic.shootPos4 == "1") ? true : false;
        shoot5 = (bossLogic.shootPos5 == "1") ? true : false;

        getBulletType = bossLogic.bulletType;
        getBulletName = bossLogic.bulletName;
        getPatternType = bossLogic.patternType;
        bulletSpeed = bossLogic.bulletSpeed;
        shootLimit = bossLogic.shootLimit;
        //waitTime = bossLogic.waitTime;

        Init();

        Debug.Log($"보스 공격 {bossLogic.BossLogicCode}");
        bossLogics.Enqueue(bossLogic);
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


    void WaitCompare()
    {
        if (enemyState == EnemyState.Wait && IdleTime >= waitTime)
        {
            enemyState = EnemyState.Play; IdleTime = 0f;
            if (enemyType != EnemyType.Boss)
                bulletPattern = bulletPatterns.Dequeue();
        }
        if (enemyState == EnemyState.Play && shootCount >= shootLimit)
        {
            enemyState = EnemyState.Wait; shootCount = 0;
            if (enemyType == EnemyType.Boss)
                setBossLogic(bossLogics.Dequeue());
            else bulletPatterns.Enqueue(bulletPattern);
        }
    }

    void ExitCompare()
    {
        if (enemyType == EnemyType.Boss) return;
        if (fieldTime >= fieldTimeLimit)
        {
            fieldTime = 0;
            enemyState = EnemyState.Exit;
            movingType = MovingType.Accel;
        }
    }

    void GetDegree(GameObject shootPos)
    {
        playerPos = GameManager.playerPos;
        if (enemyType == EnemyType.Boss && shootPos != null)
        {
            float deg = Mathf.Atan2
                (playerPos.y - shootPos.transform.position.y,
                playerPos.x - shootPos.transform.position.x)
                / Mathf.PI * 180 + 90;

            switch (setBossPos)
            {
                case 1: degree1 = deg; break;
                case 2: degree2 = deg; break;
                case 3: degree3 = deg; break;
                case 4: degree4 = deg; break;
                case 5: degree5 = deg; break;
            }
        }
        else
        {
            degree = Mathf.Atan2
                (playerPos.y - transform.position.y,
                playerPos.x - transform.position.x)
                / Mathf.PI * 180 + 90;
        }
    }

    void setBossDegree()
    {
        if (enemyType == EnemyType.Boss)
        {
            switch (setBossPos)
            {
                case 1: degree = degree1; break;
                case 2: degree = degree2; break;
                case 3: degree = degree3; break;
                case 4: degree = degree4; break;
                case 5: degree = degree5; break;
            }
        }
    }

    private void Fire(float deg, GameObject shootPos)
    {
        GameObject bullet = pool.MakeObject(getBulletName);

        if (shootPos == null)
            bullet.transform.position = transform.position;
        else
            bullet.transform.position = shootPos.transform.position;

        bullet.transform.rotation = Quaternion.Euler(0, 0, deg);


        EnemyBullet bulletFrom = bullet.GetComponent<EnemyBullet>();
        bulletFrom.getBulletType = getBulletType;
        bulletFrom.speed = bulletSpeed;
        bulletFrom.Init();

        IdleTime = 0;
    }


    // 공격 패턴 모음
    private void Straight(bool isLock, GameObject shootPos)
    {
        if (!isLock || shootCount == 0) GetDegree(shootPos);
        setBossDegree();
        Fire(degree, shootPos);
    }

    private void n_Way(bool isLock, GameObject shootPos)
    {
        if (!isLock || shootCount == 0)
        {
            GetDegree(shootPos);
        }

        setBossDegree();

        int n_Count = shootLimit;
        float degEach = 20f;
        float setDeg = degree - degEach * (n_Count - 1) / 2;

        for (int i = 0; i < n_Count; i++)
        {
            Fire(setDeg, shootPos);
            setDeg += degEach;
        }
    }

    private void Circle(bool isLock, GameObject shootPos)
    {
        if (!isLock || shootCount == 0) GetDegree(shootPos);

        setBossDegree();

        int n_Count = 20;
        float degEach = 360 / n_Count;
        float setDeg = degree;

        for (int i = 0; i < n_Count; i++)
        {
            Fire(setDeg, shootPos);
            setDeg += degEach;
        }
    }

    //역삼각형 모양처럼 흩뿌리기
    private void Spread(bool isLock, GameObject shootPos)
    {
        if (!isLock || shootCount == 0) GetDegree(shootPos);

        setBossDegree();

        float degEach = 10f;

        for (int i = 0; i < 2; i++)
        {
            float setDeg = (i == 0) ? 
                degree - degEach * (shootCount % 4) : degree + degEach * (shootCount % 4);
            Fire(setDeg, shootPos);
            if (shootCount % 4 == 0) break;
        }
    }

    private void Spread_Random(float degLimit, GameObject shootPos)
    {
        int n_Count = 4;

        setBossDegree();

        for (int i = 0; i < n_Count; i++)
        {
            float setDeg = degree + Random.Range(-degLimit / 2, degLimit / 2);
            Fire(setDeg, shootPos);
        }
    }

    private void Vortex()
    {

    }

    private void Down(GameObject shootPos) => Fire(0, shootPos);




    public void Dead()
    {
        enemyState = EnemyState.Dead;
        fieldTime = 0;

        if (Health <= 0)
        {
            GameManager.Score += setScore;
        }

        //sr.enabled = false;


        GameManager.EnemyList.Remove(gameObject);

        GameObject Explosion;

        if (enemyType == EnemyType.Boss && Health <= 0)
        {
            StartCoroutine("BossDead");
        }
        else
        {
            switch (enemyType)
            {
                case EnemyType.Large:
                case EnemyType.Big:
                    Explosion = pool.MakeObject("ExplodeC");
                    Explosion.GetComponent<Effect>().audio.clip =
                        audioManager.getAudioClip("Explode"); break;
                default:
                    Explosion = pool.MakeObject("ExplodeB");
                    Explosion.GetComponent<Effect>().audio.clip =
                        audioManager.getAudioClip("EShotL"); break;
            }
            Explosion.transform.position = transform.position;
            //Explosion.SetActive(true);
            gameObject.SetActive(false);
        }
    }

    IEnumerator BossDead()
    {
        GameManager.instance.GameClear();
        InvokeRepeating("BossExplosion", 0f, 0.16f);

        yield return new WaitForSeconds(1.5f);

        CancelInvoke("BossExplosion");
        StartCoroutine("BossDestroyed");

    }

    IEnumerator BossDestroyed()
    {
        InvokeRepeating("BossExplosion", 0f, 0.1f);

        yield return new WaitForSeconds(1.5f);

        CancelInvoke("BossExplosion");
        GameObject Explosion = pool.MakeObject("ExplodeA");
        Explosion.transform.localScale = new Vector3(10, 10, 1);
        Explosion.GetComponent<Effect>().audio.clip =
                        audioManager.getAudioClip("ExplodeBoss");
        Explosion.transform.position = transform.position;
        gameObject.SetActive(false);
    }

    void BossExplosion()
    {
        GameObject Explosion = pool.MakeObject("ExplodeB");
        Explosion.GetComponent<Effect>().audio.clip =
                        audioManager.getAudioClip("EShotL");
        Vector2 bossPos = transform.position;
        Explosion.transform.position = new Vector2(
            Random.Range(bossPos.x - 2, bossPos.x + 2),
            Random.Range(bossPos.y - 1, bossPos.y + 1));
    }


    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (enemyState == EnemyState.Dead) return;

        // 플레이어 공격 충돌 시 체력 감소
        switch (collision.tag)
        {
            case "PlayerBullet_Lv1": Health -= 3; collision.gameObject.SetActive(false); break;
            case "PlayerBullet_Lv2": Health -= 4; collision.gameObject.SetActive(false); break;
            case "PlayerBullet_Lv3": Health -= 5; collision.gameObject.SetActive(false); break;
            case "PlayerBullet_LvMAX_A": Health -= 8; collision.gameObject.SetActive(false); break;
            case "PlayerBullet_LvMAX_B": Health -= 4; collision.gameObject.SetActive(false); break;
            case "PlayerBullet_LvMAX_C": Health -= 5; collision.gameObject.SetActive(false); break;
        }

        // 적기 파괴
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

            if (item != null)
            {
                item.transform.position = transform.position;
            }

            Dead();
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Field_Out") && enemyState != EnemyState.Dead)
        {
            GameManager.EnemyList.Remove(gameObject);
            gameObject.SetActive(false);
        }
    }
}
