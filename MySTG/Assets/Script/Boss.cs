using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boss : MonoBehaviour
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

    public enum EnamyType
    {
        Diamond1
    }

    private void Awake()
    {
        switch (enamyType)
        {
            case EnamyType.Diamond1:
                Health = 200;
                break;
        }
    }

    void Start()
    {

    }

    void Update()
    {
        degree += 0.005f;
        degreeCos = Mathf.Cos(degree) * 7f;

        fieldTime += Time.deltaTime;

        if (fieldTime > 2f)
        {
            int pattenB1 = 0;
            int pattenS1 = 0;

            shootTime1 += Time.deltaTime;
            shootTime2 += Time.deltaTime;
            shootTime2_1 += Time.deltaTime;

            if (shootTime1 > 0.08f)
            {
                for (int i = 0; i < outputBulletPool.poolEBullet_BG1.Length; i++)
                {

                    GameObject bullet = outputBulletPool.poolEBullet_BG1[i];
                    if (bullet.activeSelf == false)
                    {
                        bullet.transform.position = transform.position;
                        bullet.transform.rotation =
                            (pattenB1 == 0) ? Quaternion.Euler(0, 0, 60f) : Quaternion.Euler(0, 0, -60f);
                        bullet.SetActive(true);
                        pattenB1++;
                    }
                    if (pattenB1 == 2) { shootTime1 = 0; break; }
                }
            }

            if (shootTime2 > 0.3f)
            {
                if (shootTime2_1 > 0.05f)
                {
                    for (int i = 0; i < outputBulletPool.poolEBullet_SB1.Length; i++)
                    {

                        GameObject bullet = outputBulletPool.poolEBullet_SB1[i];
                        if (bullet.activeSelf == false)
                        {
                            bullet.transform.position = transform.position + new Vector3(0, 1, 0);
                            bullet.transform.rotation = Quaternion.Euler(0, 0, pattenS1DegCos - 30f + 15f * pattenS1);

                            bullet.SetActive(true);
                            pattenS1++;
                        }
                        if (pattenS1 == 4) { shootTime2_1 = 0; break; }
                    }
                }
                if (shootTime2 >= 0.6f)
                {
                    pattenS1DegCos = Mathf.Cos(degree) * 15f;
                    shootTime2 = 0;
                }
            }

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
