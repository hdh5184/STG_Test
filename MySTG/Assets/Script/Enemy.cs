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
    Vector2 playerVec;


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

            for (int i = 0; i < pool.poolEBullet_SP1.Length; i++)
            {
                GameObject bullet = pool.poolEBullet_SP1[i];
                EnemyBullet bulletFrom = bullet.GetComponent<EnemyBullet>();

                if (bullet.activeSelf && bulletFrom.enemyFromCode == bulletFromCode)
                {
                    bullet.SetActive(false);
                    for (int j = 0; j < pool.poolItem_SilverCoin.Length; j++)
                    {
                        GameObject Coin = pool.poolItem_SilverCoin[j];
                        if (Coin.activeSelf == false)
                        {
                            Coin.transform.position = bullet.transform.position;
                            Coin.SetActive(true);
                            break;
                        }
                    }
                }
            }

            foreach (var code in temp)
            {
                GameManager.EnemyCode.Remove(code);
            }



            for (int i = 0; i < pool.poolEffect_EDestroy.Length; i++)
            {
                GameObject Explosion = pool.poolEffect_EDestroy[i];
                if (Explosion.activeSelf == false)
                {
                    Explosion.transform.position = transform.position;
                    Explosion.SetActive(true);
                    break;
                }
            }
        }
    }

    private void Zako()
    {
        shootTime += Time.deltaTime;
        playerVec = GameManager.instance.playerPos;
        degree = Mathf.Atan2
                (playerVec.y - transform.position.y, playerVec.x - transform.position.x)
                / Mathf.PI * 180 + 90;
        transform.rotation = Quaternion.Euler(0, 0, degree);

        if (shootTime > 0.1f)
        {
            for (int i = 0; i < pool.poolEBullet_SP1.Length; i++)
            {
                GameObject bullet = pool.poolEBullet_SP1[i];
                if (bullet.activeSelf == false)
                {
                    bullet.transform.position = transform.position;
                    bullet.transform.rotation = Quaternion.Euler(0, 0, degree);

                    EnemyBullet bulletFrom = bullet.GetComponent<EnemyBullet>();
                    bulletFrom.enemyFromCode = bulletFromCode;

                    bullet.SetActive(true);
                    break;
                }
            }
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
                playerVec = GameManager.instance.playerPos;
                degree = Mathf.Atan2
                        (playerVec.y - transform.position.y, playerVec.x - transform.position.x)
                        / Mathf.PI * 180 + 90;
                isShoot = !isShoot;
            }
        }
        else if (isShoot)
        {
            if (shootTime >= 0.08f)
            {
                int bulletWayCount = 0;
                for (int i = 0; i < pool.poolEBullet_SP1.Length; i++)
                {
                    GameObject bullet = pool.poolEBullet_SP1[i];
                    if (bullet.activeSelf == false)
                    {
                        bullet.transform.position = transform.position;

                        if      (bulletWayCount == 0) bullet.transform.rotation = Quaternion.Euler(0, 0, degree - 10);
                        else if (bulletWayCount == 1) bullet.transform.rotation = Quaternion.Euler(0, 0, degree);
                        else if (bulletWayCount == 2) bullet.transform.rotation = Quaternion.Euler(0, 0, degree + 10);

                        EnemyBullet bulletFrom = bullet.GetComponent<EnemyBullet>();
                        bulletFrom.enemyFromCode = bulletFromCode;

                        bullet.SetActive(true);

                        if (bulletWayCount == 2) break; else bulletWayCount++;
                    }
                }
                shootTime = 0; shootCount++;

                if(shootCount == 10) { shootCount = 0; isShoot = !isShoot; }
            }
        }

        
    }
}
