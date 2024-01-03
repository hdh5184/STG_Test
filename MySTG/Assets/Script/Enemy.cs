using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    public PoolManager outputBulletPool;

    public EnamyType enamyType;
    public int Health;
    public float fieldTime = 0;
    float shootTime1 = 0;
    float shootTime2 = 0, shootTime2_1 = 0;
    float pattenS1DegCos = 0f;

    float degree = 0f;
    public static float degreeCos = 0f;

    float eachDistance = 0f;

    public enum EnamyType
    {
        Zako,
    }   

    private void Awake()
    {
        switch (enamyType)
        {
            case EnamyType.Zako:
                Health = 5;
                break;
        }
    }

    void Start()
    {
        
    }

    void Update()
    {
        shootTime1 += Time.deltaTime;

        if(shootTime1 > 0.2f)
        {
            for (int i = 0; i < outputBulletPool.poolEBullet_SP1.Length; i++)
            {
                GameObject bullet = outputBulletPool.poolEBullet_SP1[i];
                if (bullet.activeSelf == false)
                {
                    bullet.transform.position = transform.position;

                    var playerVec = GameManager.instance.playerPos;
                    float degree;
                    degree = Mathf.Atan2
                            (playerVec.y - transform.position.y, playerVec.x - transform.position.x)
                            / Mathf.PI * 180;
                    //Debug.Log(degree);
                    bullet.transform.rotation = Quaternion.Euler(0, 0, degree + 90);
                    bullet.SetActive(true);
                    break;
                }
            }
            shootTime1 = 0;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("PlayerBullet"))
        {
            Health--;
            collision.gameObject.SetActive(false);
        }

        if (Health <= 0)
        {
            gameObject.SetActive(false);
        }
    }
}
