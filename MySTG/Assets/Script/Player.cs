using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public PoolManager pool;

    List<GameObject> triggerCompare = new List<GameObject>();

    public GameObject playerShootPos;
    public GameObject playerOptionL, playerOptionR;
    public GameObject playerGuardSprite;

    Animator animator;

    Vector2 PlayerMovingVec;

    bool isGuard = false;
    bool isDestroyed = false;

    float shootTime = 0, shootTimeII = 0, shootTimeIII;

    private void Awake()
    {
        animator = GetComponent<Animator>();
    }

    void Start()
    {
        if (GameManager.playerLevel == 4) { playerOptionL.SetActive(true); playerOptionR.SetActive(true); }
        else { playerOptionL.SetActive(false); playerOptionR.SetActive(false); }
    }

    void Update()
    {
        Move();
        Fire();
        Compare();
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

        playerOptionL.transform.position = transform.position + new Vector3(-0.6f, -0.5f);
        playerOptionR.transform.position = transform.position + new Vector3(0.6f, -0.5f);
    }

    void Fire()
    {
        shootTime += Time.deltaTime;
        shootTimeII += Time.deltaTime;
        shootTimeIII += Time.deltaTime;

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
                        case 3:
                        case 4: playerBullet = pool.poolPBullet_Lv2[i]; break;
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
            

            if (GameManager.playerLevel >= 3)
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
                                playerShootPos.transform.position + new Vector3(-0.35f, 0f) :
                                playerShootPos.transform.position + new Vector3(0.35f, 0f);
                            Lv3Count++;

                            if (Lv3Count == 2) { shootTimeII = 0f; break; }
                        }
                    }
                }
                
            }

            if (GameManager.playerLevel == 4)
            {
                playerOptionL.SetActive(true); playerOptionR.SetActive(true);

                if (shootTimeIII > 0.03f)
                {
                    int Lv4Count = 0;
                    for (int i = 0; i < 80; i++) //pool.poolPBullet_Lv4.Length = 80
                    {
                        GameObject playerBullet = null;
                        playerBullet = pool.poolPBullet_Lv4[i];

                        if (playerBullet.activeSelf == false)
                        {
                            playerBullet.gameObject.SetActive(true);
                            playerBullet.transform.position =
                                (Lv4Count == 0) ? playerOptionL.transform.position : playerOptionR.transform.position;
                            Lv4Count++;

                            if (Lv4Count == 2) { shootTimeIII = 0f; break; }
                        }
                    }
                }

            }
        }
    }

    void Compare()
    {
        if (GameManager.playerLevel == 4) { playerOptionL.SetActive(true); playerOptionR.SetActive(true); }
        else { playerOptionL.SetActive(false); playerOptionR.SetActive(false); }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("EnemyBullet") && !isGuard && !isDestroyed)
        {
            Debug.Log("1111");
            //foreach (var trigger in triggerCompare)
            //{
            //    if (trigger == collision.gameObject) return;
            //}
            //triggerCompare.Add(collision.gameObject);
            StartCoroutine("DestroyPlayer");
            Invoke("ReloadPlayer", 1.5f);

            //Invoke("ReloadPlayer", 1.5f);
        }
    }

    private IEnumerator DestroyPlayer()
    {
        isDestroyed = true;
        GetComponent<Collider2D>().isTrigger = false;
        gameObject.SetActive(false);
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

        int summonItemLimit = 0;
        for (int i = 0; i < pool.poolItem_PowerUp.Length; i++)
        {
            GameObject Item = pool.poolItem_PowerUp[i];
            if (Item.activeSelf == false)
            {
                
                Item.transform.position = transform.position;
                Item.SetActive(true);
                summonItemLimit++;
                if (summonItemLimit == 2) break;
            }
        }
        
        transform.position = GameManager.instance.transform.position + new Vector3(0, -3f);

        GameManager.playerLevel = 1;
        playerOptionL.SetActive(false); playerOptionR.SetActive(false);

        yield return new WaitForSeconds(1.5f);
    }

    void ReloadPlayer()
    {
        //triggerCompare.Clear();
        GetComponent<Collider2D>().isTrigger = true;
        isDestroyed = false;
        isGuard = true;
        playerGuardSprite.SetActive(true);
        gameObject.SetActive(true);
        Invoke("Unguard", 2.5f);
    }

    void Unguard() { isGuard = false; playerGuardSprite.SetActive(false); }
}
