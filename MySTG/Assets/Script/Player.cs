using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    // 오브젝트 pool (탄, 아이템 등)
    public PoolManager pool;

    // 플레이어 샷 위치, 옵션, 무적 스프라이트
    public GameObject playerShootPos;
    public GameObject playerOptionL, playerOptionR;
    public GameObject playerGuardSprite;

    Animator animator;

    // 플레이어 이동 벡터
    Vector2 PlayerMovingVec;

    float shootTime = 0, shootTime_Lv2 = 0, shootTime_Lv3 = 0f;

    // 플레이어 인게임 상태 (평상시, 파괴됨, 무적 시간)
    enum PlayerState { Play, Dead, Guard }
    PlayerState playerState;

    private void Awake()
    {
        animator = GetComponent<Animator>();
    }

    void Start()
    {
        // 플레이어 평상시 상태, 레벨 4일 때 옵션 표시
        playerState = PlayerState.Play;
        if (GameManager.playerLevel == 4)   { playerOptionL.SetActive(true); playerOptionR.SetActive(true); }
        else                                { playerOptionL.SetActive(false); playerOptionR.SetActive(false); }
    }

    void Update()
    {
        Move();     // 플레이어 이동
        Fire();     // 플레이어 공격
        Compare();  // 플레이어 레벨 확인
    }

    void Move()
    {
        // 방향키 입력에 따른 벡터 이동 지정
        PlayerMovingVec = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));

        // 왼쪽 shift 키를 누르면서 느린 속도 이동
        if (Input.GetKey(KeyCode.LeftShift))    transform.Translate(PlayerMovingVec * Time.deltaTime * 2.5f);
        else                                    transform.Translate(PlayerMovingVec * Time.deltaTime * 4f);

        // 필드 외에 나가지 않도록 이동 범위 제한
        transform.position = new Vector2(
            Mathf.Clamp(transform.position.x, -2.3f, 2.3f),
            Mathf.Clamp(transform.position.y, -4.5f, 4.5f));

        // 플레이어 옵션 위치 지정 - 양 쪽에 하나 씩
        playerOptionL.transform.position = transform.position + new Vector3(-0.6f, -0.5f);
        playerOptionR.transform.position = transform.position + new Vector3(0.6f, -0.5f);
    }

    void Fire()
    {
        // 기본탄(Lv1~2), 강화탄(Lv3), 레이저(Lv4) 공격 시간 카운트
        shootTime += Time.deltaTime;
        shootTime_Lv2 += Time.deltaTime;
        shootTime_Lv3 += Time.deltaTime;

        // 공격
        if (Input.GetKey(KeyCode.Z))
        {
            // 플레이어 기본 공격
            if (shootTime > 0.07f)
            {
                for (int i = 0; i < 20; i++) //pool.poolPBullet_Lv1.Length = 20
                {
                    GameObject playerBullet = null;

                    switch (GameManager.playerLevel)
                    {
                        case 1: playerBullet = pool.poolPBullet_Lv1[i]; break;  // Lv1 : 탄 1개
                        case 2:
                        case 3:
                        case 4: playerBullet = pool.poolPBullet_Lv2[i]; break;  // Lv2 이상 : 탄 3개
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

            // 플레이어 Lv3일 때 강화탄 공격 (플레이어 양쪽 배치, 총 2개)
            if (GameManager.playerLevel >= 3)
            {
                if (shootTime_Lv2 > 0.2f)
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

                            if (Lv3Count == 2) { shootTime_Lv2 = 0f; break; }
                        }
                    }
                }

            }

            // 플레이어 Lv4일 때 레이저 공격 (플레이어 옵션이 레이저 발사, 총 2개)
            if (GameManager.playerLevel == 4)
            {
                playerOptionL.SetActive(true); playerOptionR.SetActive(true);

                if (shootTime_Lv3 > 0.03f)
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

                            if (Lv4Count == 2) { shootTime_Lv3 = 0f; break; }
                        }
                    }
                }

            }
        }
    }

    void Compare()
    {
        // 플레이어 평상시 상태, 레벨 4일 때 옵션 표시
        if (GameManager.playerLevel == 4)   { playerOptionL.SetActive(true); playerOptionR.SetActive(true); }
        else                                { playerOptionL.SetActive(false); playerOptionR.SetActive(false); }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // 적 탄알에 피탄 시 플레이어 기체 파괴, 1.5초 후 무적 상태로 재생성
        if (collision.CompareTag("EnemyBullet") && playerState == PlayerState.Play)
        {
            //Debug.Log("Player Destroyed");
            StartCoroutine("DestroyPlayer");
            Invoke("ReloadPlayer", 1.5f);
        }
    }

    // 플레이어 기체 파괴
    private IEnumerator DestroyPlayer()
    {
        playerState = PlayerState.Dead;
        //GetComponent<Collider2D>().isTrigger = false;
        gameObject.SetActive(false);

        // 폭발 연출 생성
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

        // PowerUp 아이템 2개 생성
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

        // 플레이어 레벨 1로 감소, 옵션 해제
        GameManager.playerLevel = 1;
        playerOptionL.SetActive(false); playerOptionR.SetActive(false);

        // 부활 쿨타임 1.5초
        yield return new WaitForSeconds(1.5f);
    }

    // 무적 상태로 재생성
    void ReloadPlayer()
    {
        playerState = PlayerState.Guard;

        transform.position = GameManager.instance.transform.position + new Vector3(0, -3f);
        //GetComponent<Collider2D>().isTrigger = true;
        playerGuardSprite.SetActive(true);
        gameObject.SetActive(true);

        // 2.5초 후 무적 해제
        Invoke("Unguard", 2.5f);
    }

    // 무적 해제
    void Unguard()
    {
        playerState = PlayerState.Play;
        playerGuardSprite.SetActive(false);
    }

    // 디버그용 플레이어 기체 파괴
    public void Debug_Dead()
    {
        if(playerState == PlayerState.Play)
        {
            StartCoroutine("DestroyPlayer");
            Invoke("ReloadPlayer", 1.5f);
        }
    }
}
