using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    public PoolManager pool;

    // 적 타입, 체력, 출현 시간, 공격 쿨타임
    public EnemyType enemyType;
    public int Health;
    public float fieldTime = 0;
    float shootTime = 0;


    // 적기 코드
    float bulletFromCode = 0;

    // 플레이어 위치 참조용
    public Vector3 playerPos;


    bool isShoot = false;
    int shootCount = 0;
    float degree = 0f;
    public static float degreeCos = 0f;

    // 적 타입
    public enum EnemyType
    {
        Zako, Small
    }   

    private void Awake()
    {
        // 적기 코드 생성 후 기록
        bulletFromCode = Random.Range(0f, 1f) * Random.Range(0f, 1f);
        GameManager.EnemyCode.Add(bulletFromCode);
    }

    private void OnEnable()
    {
        switch (enemyType)
        {
            case EnemyType.Zako: Health = 5; break;
            case EnemyType.Small: Health = 20; break;
        }
    }

    void Start()
    {

    }

    void Update()
    {
        switch (enemyType)
        {
            case EnemyType.Zako: Zako(); break;
            case EnemyType.Small: Small(); break;
        }
        
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // 플레이어 공격 충돌 시 체력 감소
        switch (collision.tag)
        {
            case "PlayerBullet_Lv1": Health--;      collision.gameObject.SetActive(false); break;
            case "PlayerBullet_Lv2": Health -= 3;   collision.gameObject.SetActive(false); break;
            case "PlayerBullet_Lv3": Health -= 5;   collision.gameObject.SetActive(false); break;
            case "PlayerBullet_Lv4": Health--;      collision.gameObject.SetActive(false); break;
        }

        // 적기 파괴
        if (Health <= 0)
        {
            gameObject.SetActive(false);

            List<float> temp = new List<float>();

            foreach (var code in GameManager.EnemyCode)
            {
                if (code == bulletFromCode) temp.Add(bulletFromCode);
            }


            foreach (var compareBullet in pool.poolEBullet_SP1)
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
            }

            foreach (var code in temp)
            {
                GameManager.EnemyCode.Remove(code);
            }

            GameObject Explosion = pool.MakeObject("EDestroy");
            Explosion.transform.position = transform.position;
            Explosion.SetActive(true);
        }
    }

    private void Zako()
    {
        shootTime += Time.deltaTime;

        playerPos = GameManager.instance.playerPos;
        degree = Mathf.Atan2
                (playerPos.y - transform.position.y, playerPos.x - transform.position.x)
                / Mathf.PI * 180 + 90;
        transform.rotation = Quaternion.Euler(0, 0, degree);

        if (shootTime > 0.1f)
        {
            GameObject bullet = pool.MakeObject("Bullet_SP1");
            bullet.transform.position = transform.position;
            bullet.transform.rotation = Quaternion.Euler(0, 0, degree);

            EnemyBullet bulletFrom = bullet.GetComponent<EnemyBullet>();
            bulletFrom.enemyFromCode = bulletFromCode;

            shootTime = 0;
        }
    }

    private void Small()
    {
        shootTime += Time.deltaTime;

        if (!isShoot)
        {
            if(shootTime >= 0.5f)
            {
                playerPos = GameManager.instance.playerPos;
                degree = Mathf.Atan2
                        (playerPos.y - transform.position.y, playerPos.x - transform.position.x)
                        / Mathf.PI * 180 + 90;
                isShoot = !isShoot;
            }
        }
        else if (isShoot)
        {
            if (shootTime >= 0.08f)
            {
                int bulletWayCount = 0;

                while (bulletWayCount < 3)
                {
                    GameObject bullet = pool.MakeObject("Bullet_SP1");

                    bullet.transform.position = transform.position;

                    if (bulletWayCount == 0) bullet.transform.rotation = Quaternion.Euler(0, 0, degree - 10);
                    else if (bulletWayCount == 1) bullet.transform.rotation = Quaternion.Euler(0, 0, degree);
                    else if (bulletWayCount == 2) bullet.transform.rotation = Quaternion.Euler(0, 0, degree + 10);

                    EnemyBullet bulletFrom = bullet.GetComponent<EnemyBullet>();
                    bulletFrom.enemyFromCode = bulletFromCode;

                    bulletWayCount++;
                }

                shootTime = 0; shootCount++;

                if(shootCount == 10) { shootCount = 0; isShoot = !isShoot; }
            }
        }

        
    }
}
