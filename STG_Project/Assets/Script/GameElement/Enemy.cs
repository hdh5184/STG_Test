using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using static LobbyManager;
using static StageManager;

public class Enemy : MonoBehaviour
{
    // 1. 매니저
    public PoolManager pool;
    public AudioManager audioManager;

    // 2. 패턴
    public Queue<BulletPattern> bulletPatterns = new Queue<BulletPattern>();
    public Queue<BossLogic> bossLogics = new Queue<BossLogic>();
    public Queue<BossLogic> bossLogicsFinal = new Queue<BossLogic>();

    // 3. 플레이어 참조
    public Vector3 playerPos;

    // 4. Enemy 및 탄 속성
    public EnemyState enemyState;
    public EnemyType enemyType;
    public MovingType movingType;
    public BulletPattern bulletPattern;

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
    private int setScore;
    private int Health;
    public float firstWaitTime, waitTime;

    // 9. 시간, 카운트
    float fieldTime = 0;
    float IdleTime = 0;
    float shootTime = 0.1f;
    int shootCount = 0;

    // 10. 탄도 각
    float degree = 0f;

    // A. 보스
    public GameObject[] BigShootPos;
    public GameObject BossShootPos1, BossShootPos2, BossShootPos3, BossShootPos4, BossShootPos5;
    int setBossPos = 0;
    bool shoot1, shoot2, shoot3, shoot4, shoot5;
    float degree1, degree2, degree3, degree4, degree5;
    bool isBossFinal = false;

    // B. 보조 무기
    public GameObject[] subWeaponObj;
    public GameObject subWeaponOwner;

    // e. 속성 모음
    public enum EnemyState { Idle, Play, Wait, Exit, Dead }
    public enum EnemyType { Small, Medium, Large, Big, Boss, SubWeapon }
    public enum MovingType { Straight, Accel, SlowDown, none }
    public enum BulletPattern { Straight, n_Way, Circle, Spread, Spread_Random, Vortex, Down, None }

    /// <summary> Enemy 기체 데이터 초기화 </summary>
    private void OnEnable()
    {
        enemyState = EnemyState.Idle;
        fieldTime = 0;
        IdleTime = 0;
        shootTime = 0.1f;
        shootCount = 0;
        degree = 0;

        if (subWeaponObj != null)   foreach (var item in subWeaponObj) item.SetActive(true);
        if (pool == null )          pool = PoolManager.instance;
        if (audioManager == null)   audioManager = AudioManager.instance;

        switch (enemyType)
        {
            case EnemyType.Small:   Health = 3;     setScore = 100;     break;
            case EnemyType.Medium:  Health = 30;    setScore = 500;     break;
            case EnemyType.Large:   Health = 120;   setScore = 5000;    break;
            case EnemyType.Big:     Health = 350;   setScore = 20000;   break;
            case EnemyType.Boss:    Health = 1200;  setScore = 80000;   break;

            case EnemyType.SubWeapon: Health = 30;  setScore = 150;     break;
        }

        bulletPatterns.Clear();
        bossLogicsFinal.Clear();
    }

    /// <summary> Enemy - 공격, 이동, 기체 회전 속성 초기화 (+ 보스 공격) </summary>
    public void EnemyInit()
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

        if (enemyType != EnemyType.Boss) bulletPatterns.Enqueue(bulletPattern);
        transform.rotation = Quaternion.Euler(0, 0, degreeZ);
    }


    /// <summary> Enemy 로직 </summary>
    void Update()
    {
        if (LobbyManager.menuSelected == MenuSelected.Main) gameObject.SetActive(false);
        if (StageManager.stageState == StageState.End ||
            StageManager.stageState == StageState.Pause) return;
        if (enemyState == EnemyState.Dead) return;

        fieldTime += Time.deltaTime;
        IdleTime += Time.deltaTime;

        switch (enemyState)
        {
            case EnemyState.Idle:
                if (IdleTime >= firstWaitTime)
                { enemyState = EnemyState.Play; IdleTime = 0f; } break;
            case EnemyState.Play:
                if (IdleTime >= shootTime)
                {
                    if (enemyType == EnemyType.Boss) BossPattern();
                    else if (enemyType == EnemyType.SubWeapon)
                    {
                        foreach (var item in BigShootPos)
                        {
                            SelectPattern(item, 0); shootCount++;
                        }
                    }
                    else
                    {
                        SelectPattern(null, 0); shootCount++;
                    }
                }

                if (!isBossFinal && fieldTime >= fieldTimeLimit && enemyType == EnemyType.Boss)
                {
                    isBossFinal = true;
                    setBossLogic(bossLogicsFinal.Peek());
                    bossLogics.Clear();
                    bossLogics = bossLogicsFinal;
                }
                break;
        }

        if (enemyType == EnemyType.SubWeapon)
        {
            GetDegree(null); transform.rotation = Quaternion.Euler(0, 0, degree);
        }

        Moving();
        WaitCompare();
        ExitCompare();
    }

    /// <summary> 공격 패턴 설정 </summary>
    void SelectPattern(GameObject shootPos, int pos)
    {
        if (enemyType == EnemyType.Boss) setBossPos = pos;

        switch (bulletPattern)
        {
            case BulletPattern.Straight:    Straight(false, shootPos); break;
            case BulletPattern.n_Way:       n_Way(true, shootPos); break;
            case BulletPattern.Circle:      Circle(true, shootPos); break;
            case BulletPattern.Spread:      Spread(true, shootPos); break;
            case BulletPattern.Spread_Random: Spread_Random(180, shootPos); break;
            case BulletPattern.Vortex:      Vortex(); break;
            case BulletPattern.Down:        Down(shootPos); break;
        }
    }

    /// <summary> 보스 공격 패턴 설정 </summary>
    void BossPattern()
    {
        if (shoot1) SelectPattern(BossShootPos1, 1);
        if (shoot2) SelectPattern(BossShootPos2, 2);
        if (shoot3) SelectPattern(BossShootPos3, 3);
        if (shoot4) SelectPattern(BossShootPos4, 4);
        if (shoot5) SelectPattern(BossShootPos5, 5);
        shootCount++;
    }

    /// <summary> 보스 Enemy 로직 설정 </summary>
    void setBossLogic(BossLogic bossLogic)
    {
        waitTime = bossLogic.delay;
        movSpeed = bossLogic.movSpeed;
        getMovingType = bossLogic.movingType;
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

        EnemyInit();

        Debug.Log($"보스 공격 {bossLogic.BossLogicCode}");
        bossLogics.Enqueue(bossLogic);
    }

    /// <summary> Enemy 이동 로직 </summary>
    void Moving()
    {
        switch (movingType)
        {
            case MovingType.Straight:
                transform.Translate(moveVec * movSpeed * Time.deltaTime); break;
            case MovingType.Accel:
                if (enemyState == EnemyState.Exit)
                    transform.Translate(moveExitVec * movSpeed * Time.deltaTime * fieldTime * 2);
                else transform.Translate(moveVec * movSpeed * Time.deltaTime * fieldTime * 2); break;
            case MovingType.SlowDown:
                if (fieldTime < 1)
                    transform.position = Vector2.Lerp(transform.position, moveDesVec, 0.07f);
                else movingType = MovingType.Straight; break;
        }
    }

    /// <summary> 공격 - 대기 로직 </summary>
    void WaitCompare()
    {
        switch (enemyState)
        {
            case EnemyState.Wait:
                if (IdleTime >= waitTime)
                {
                    enemyState = EnemyState.Play; IdleTime = 0f;
                    if (enemyType != EnemyType.Boss)
                        bulletPattern = bulletPatterns.Dequeue();
                }
                break;
            case EnemyState.Play:
                if (shootCount >= shootLimit)
                {
                    enemyState = EnemyState.Wait; shootCount = 0;
                    if (enemyType == EnemyType.Boss)
                        setBossLogic(bossLogics.Dequeue());
                    else bulletPatterns.Enqueue(bulletPattern);
                }
                break;
        }
    }

    /// <summary> 후퇴 로직 </summary>
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

    /// <summary> 탄도 각 설정 </summary>
    void GetDegree(GameObject shootPos)
    {
        playerPos = StageManager.playerPos;
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

    /// <summary> 보스 공격 Position 별 탄도 각 설정 </summary>
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

    /// <summary> 탄 발사 로직 </summary>
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


    /*************** 공격 패턴 모음 ***************/

    /// <summary> str : 플레이어 조준 공격 </summary>
    private void Straight(bool isLock, GameObject shootPos)
    {
        if (!isLock || shootCount == 0) GetDegree(shootPos);
        setBossDegree();
        Fire(degree, shootPos);
    }

    /// <summary> way : n개 탄 방사 공격 </summary>
    private void n_Way(bool isLock, GameObject shootPos)
    {
        if (!isLock || shootCount == 0) GetDegree(shootPos);

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

    /// <summary> cir : 원형 공격 </summary>
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

    /// <summary> spr : 역삼각형 모양 방사 반복 공격 </summary>
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

    /// <summary> sprR : 플레이어 기준 전방 무작위 각도 방사 공격 </summary>
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
 
    /// <summary> down : 전방 고정 공격 </summary>
    private void Down(GameObject shootPos) => Fire(0, shootPos);


    /*************** 기체 파괴 처리 ***************/

    /// <summary> 기체 파괴 </summary>
    public void Dead()
    {
        enemyState = EnemyState.Dead;
        StageManager.Score += setScore;
        StageManager.EnemyList.Remove(gameObject);

        switch (enemyType)
        {
            case EnemyType.Small:
            case EnemyType.SubWeapon:
            case EnemyType.Medium:  Explosion("ExplodeB", "EShotL"); break;
            case EnemyType.Large:   Explosion("ExplodeC", "Explode"); break;
            case EnemyType.Big:     StartCoroutine("MidBossDead"); return;
            case EnemyType.Boss:    StartCoroutine("BossDead"); return;
        }

        if (subWeaponOwner != null)
            subWeaponOwner.GetComponent<Enemy>().Health -= 50;

        gameObject.SetActive(false);
    }

    /// <summary> 폭발 이펙트 출현 </summary>
    GameObject Explosion(string obj, string audio)
    {
        GameObject explosion = pool.MakeObject(obj);
        explosion.GetComponent<Effect>().audio.clip =
            audioManager.getAudioClip(audio);
        explosion.transform.position = transform.position;
            
        return explosion;
    }

    /// <summary> 폭발 이벤트 반복 출현 (범위 내 무작위) </summary>
    void RandomExplosion()
    {
        GameObject explosion = Explosion("ExplodeB", "EShotL");
        Vector2 bossPos = transform.position;
        Vector2 ExplosionPos = new Vector2(
            Random.Range(bossPos.x - 2, bossPos.x + 2),
            Random.Range(bossPos.y - 1, bossPos.y + 1));
        explosion.transform.position = ExplosionPos;

        if (enemyType == EnemyType.Big)
        {
            GameObject item = pool.MakeObject("SilverCoin");
            item.transform.position = ExplosionPos;
        }
    }

    /// <summary> 중 보스 파괴 표현 </summary>
    IEnumerator MidBossDead()
    {
        InvokeRepeating("RandomExplosion", 0f, 0.12f);
        yield return new WaitForSeconds(1.2f);

        CancelInvoke("RandomExplosion");
        GameObject explosion = Explosion("ExplodeBoss", "ExplodeBoss");
        explosion.transform.localScale = new Vector3(10, 10, 1);
        gameObject.SetActive(false);
    }

    /// <summary> 보스 파괴 표현 </summary>
    IEnumerator BossDead()
    {
        StageManager.instance.GameClear();
        InvokeRepeating("RandomExplosion", 0f, 0.16f);
        StageManager.bossExist = false;
        yield return new WaitForSeconds(1.5f);

        CancelInvoke("RandomExplosion");
        StartCoroutine("BossDestroyed");

    }

    /// <summary> 보스 파괴 표현 2 </summary>
    IEnumerator BossDestroyed()
    {
        InvokeRepeating("RandomExplosion", 0f, 0.1f);
        yield return new WaitForSeconds(1.5f);

        CancelInvoke("RandomExplosion");
        GameObject explosion = Explosion("ExplodeBoss", "ExplodeBoss");
        explosion.transform.localScale = new Vector3(10, 10, 1);
        gameObject.SetActive(false);
    }

    /*************** 충돌 처리 ***************/

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (enemyState == EnemyState.Dead) return;

        switch (collision.tag)
        {
            case "PlayerBullet_Lv1":        Health -= 3; collision.gameObject.SetActive(false); break;
            case "PlayerBullet_Lv2":        Health -= 4; collision.gameObject.SetActive(false); break;
            case "PlayerBullet_Lv3":        Health -= 5; collision.gameObject.SetActive(false); break;
            case "PlayerBullet_LvMAX_A":    Health -= 8; collision.gameObject.SetActive(false); break;
            case "PlayerBullet_LvMAX_B":    Health -= 4; collision.gameObject.SetActive(false); break;
            case "PlayerBullet_LvMAX_C":    Health -= 5; collision.gameObject.SetActive(false); break;
        }

        if (Health <= 0)
        {
            GameObject item;
            switch (getDropItemName)
            {
                case "silver":  item = pool.MakeObject("SilverCoin"); break;
                case "gold":    item = pool.MakeObject("GoldCoin"); break;
                case "pow":     item = pool.MakeObject("PowerUp"); break;
                case "hp":      item = pool.MakeObject("Heal"); break;
                default:        item = null; break;
            }

            if (item != null)   item.transform.position = transform.position;
            Dead();
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Field_Out") && enemyState != EnemyState.Dead)
        {
            StageManager.EnemyList.Remove(gameObject);
            gameObject.SetActive(false);
        }
    }
}