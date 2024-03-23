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
        Small, Medium, Large, Big
    }

    public enum BulletPattern
    {

        Straight, n_Way, Random, Spread, 
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
            case EnemyType.Small:   Health = 3; break;
            case EnemyType.Medium:  Health = 30; break;
            case EnemyType.Large:   Health = 120; break;
            case EnemyType.Big:     Health = 270; break;
        }
    }

    void Start()
    {

    }

    void Update()
    {
        switch (enemyType)
        {
            case EnemyType.Small: Small(); break;
        }
        
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // 플레이어 공격 충돌 시 체력 감소
        switch (collision.tag)
        {
            case "PlayerBullet_Lv1": Health -= 3;   collision.gameObject.SetActive(false); break;
            case "PlayerBullet_Lv2": Health -= 4;   collision.gameObject.SetActive(false); break;
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

            foreach (var code in temp)
            {
                GameManager.EnemyCode.Remove(code);
            }

            GameObject Explosion = pool.MakeObject("EDestroy");
            Explosion.transform.position = transform.position;
            Explosion.SetActive(true);
        }
    }

    private void Small()
    {
        shootTime += Time.deltaTime;
        playerPos = GameManager.instance.playerPos;

        float subdeg = degree;

        degree = Mathf.Atan2
                (playerPos.y - transform.position.y, playerPos.x - transform.position.x)
                / Mathf.PI * 180 + 90;
        transform.rotation = Quaternion.Euler(0, 0, degree);

        subdeg = degree - subdeg;

        Debug.Log(subdeg);

        if (shootTime > 0.1f)
        {
            GameObject bullet = pool.MakeObject("EBS_A");
            bullet.transform.position = transform.position;
            bullet.transform.rotation = Quaternion.Euler(0, 0, degree);

            EnemyBullet bulletFrom = bullet.GetComponent<EnemyBullet>();
            bulletFrom.enemyFromCode = bulletFromCode;

            shootTime = 0;
        }
    }
}
