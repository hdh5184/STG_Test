using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    public PoolManager outputBulletPool;

    public EnemyType enemyType;
    public int Health;
    public float fieldTime = 0;
    float shootTime = 0;
    float shootTime2 = 0, shootTime2_1 = 0;
    float pattenS1DegCos = 0f;

    Vector2 playerVec;
    bool isShoot = false;
    int shootCount = 0;
    float degree = 0f;
    public static float degreeCos = 0f;

    float eachDistance = 0f;

    public enum EnemyType
    {
        Zako, Small
    }   

    private void Awake()
    {
        
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
        switch (collision.tag)
        {
            case "PlayerBullet_Lv1": Health--;      collision.gameObject.SetActive(false); break;
            case "PlayerBullet_Lv2": Health -= 3;   collision.gameObject.SetActive(false); break;
            case "PlayerBullet_Lv3": Health -= 5;   collision.gameObject.SetActive(false); break;
            case "PlayerBullet_Lv4": Health--;      collision.gameObject.SetActive(false); break;
        }

        

        if (Health <= 0)
        {
            gameObject.SetActive(false);
            for (int i = 0; i < outputBulletPool.poolEffect_EDestroy.Length; i++)
            {
                GameObject Explosion = outputBulletPool.poolEffect_EDestroy[i];
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
            for (int i = 0; i < outputBulletPool.poolEBullet_SP1.Length; i++)
            {
                GameObject bullet = outputBulletPool.poolEBullet_SP1[i];
                if (bullet.activeSelf == false)
                {
                    bullet.transform.position = transform.position;
                    bullet.transform.rotation = Quaternion.Euler(0, 0, degree);
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
                for (int i = 0; i < outputBulletPool.poolEBullet_SP1.Length; i++)
                {
                    GameObject bullet = outputBulletPool.poolEBullet_SP1[i];
                    if (bullet.activeSelf == false)
                    {
                        bullet.transform.position = transform.position;

                        if      (bulletWayCount == 0) bullet.transform.rotation = Quaternion.Euler(0, 0, degree - 10);
                        else if (bulletWayCount == 1) bullet.transform.rotation = Quaternion.Euler(0, 0, degree);
                        else if (bulletWayCount == 2) bullet.transform.rotation = Quaternion.Euler(0, 0, degree + 10);
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
