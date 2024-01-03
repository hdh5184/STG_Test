using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public PoolManager pool;

    public GameObject playerShootPos;

    Animator animator;

    Vector2 PlayerMovingVec;

    float shootTime = 0;

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

        if (Input.GetKey(KeyCode.Z) && shootTime > 0.07f && GameManager.playerLevel == 1)
        {
            for (int i = 0; i < pool.poolPBullet.Length; i++)
            {
                GameObject playerBullet = pool.poolPBullet[i];
                if (playerBullet.activeSelf == false)
                {
                    playerBullet.gameObject.SetActive(true);
                    playerBullet.transform.position = playerShootPos.transform.position;
                    shootTime = 0f;
                    break;
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
