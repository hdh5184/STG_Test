using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public PoolManager pool;

    public GameObject playerShootPos;

    Animator animator;

    Vector2 PlayerMovingVec;

    float shootTime = 0, shootTimeII = 0;

    private void Awake()
    {
        animator = GetComponent<Animator>();
    }

    void Start()
    {
        
    }

    void Update()
    {
        Move();
        Fire();
    }

    void Move()
    {
        PlayerMovingVec = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));

        if (Input.GetKey(KeyCode.LeftShift))
        {
            transform.Translate(PlayerMovingVec * Time.deltaTime * 2.5f);
        }
        else
        {
            transform.Translate(PlayerMovingVec * Time.deltaTime * 4f);
        }

        transform.position = new Vector2(
            Mathf.Clamp(transform.position.x, -2.3f, 2.3f),
            Mathf.Clamp(transform.position.y, -4.5f, 4.5f));
    }

    void Fire()
    {
        shootTime += Time.deltaTime;
        shootTimeII += Time.deltaTime;

        if (Input.GetKey(KeyCode.Z))
        {
            if(shootTime > 0.07f)
            {
                for (int i = 0; i < 20; i++) //pool.poolPBullet_Lv1.Length = 20
                {
                    GameObject playerBullet = null;

                    switch (GameManager.playerLevel)
                    {
                        case 1: playerBullet = pool.poolPBullet_Lv1[i]; break;
                        case 2:
                        case 3: playerBullet = pool.poolPBullet_Lv2[i]; break;
                    }

                    if (playerBullet.activeSelf == false)
                    {
                        playerBullet.gameObject.SetActive(true);
                        playerBullet.transform.position = playerShootPos.transform.position;
                        shootTime = 0f;
                        break;
                    }
                }
            }
            

            if (GameManager.playerLevel == 3)
            {
                if (shootTimeII > 0.2f)
                {
                    int Lv3Count = 0;
                    for (int i = 0; i < 20; i++) //pool.poolPBullet_Lv3.Length = 20
                    {
                        GameObject playerBullet = null;
                        playerBullet = pool.poolPBullet_Lv3[i];

                        if (playerBullet.activeSelf == false)
                        {
                            playerBullet.gameObject.SetActive(true);
                            playerBullet.transform.position = (Lv3Count == 0) ?
                                new Vector2(playerShootPos.transform.position.x - 0.35f, playerShootPos.transform.position.y) :
                                new Vector2(playerShootPos.transform.position.x + 0.35f, playerShootPos.transform.position.y);
                            Lv3Count++;

                            if (Lv3Count == 2) { shootTimeII = 0f; break; }
                        }
                    }
                }
                
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("EnemyBullet"))
        {
            Debug.Log("Ooooooooooooooof");
        }
    }
}
