using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    // 오브젝트 pool (탄, 아이템 등)
    public PoolManager pool;

    // 플레이어 샷 위치, 옵션, 무적 스프라이트
    public PlayerType playerType;
    public GameObject playerShootPos;
    public GameObject playerGuardSprite;

    // 플레이어 이동 벡터
    Vector2 PlayerMovingVec;

    float shootTime = 0, shootTime_Lv2 = 0;

    // 플레이어 인게임 상태 (평상시, 파괴됨, 무적 시간)
    public enum PlayerType { A, B, C }
    enum PlayerState { Play, Dead, Guard }
    PlayerState playerState;

    


    private void Awake()
    {

    }

    void Start()
    {
        // 플레이어 평상시 상태, 레벨 4일 때 옵션 표시
        playerState = PlayerState.Play;
    }

    void Update()
    {
        Move();     // 플레이어 이동
        Fire();     // 플레이어 공격
    }


    void Move()
    {
        // 방향키 입력에 따른 플레이어 이동
        PlayerMovingVec = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
        transform.Translate(PlayerMovingVec * Time.deltaTime * 4f);

        // 필드 외에 나가지 않도록 이동 범위 제한
        transform.position = new Vector2(
            Mathf.Clamp(transform.position.x, -2.3f, 2.3f),
            Mathf.Clamp(transform.position.y, -4.5f, 4.5f));
    }

    void Fire()
    {
        // 기본탄(Lv1~3), 강화탄(Lv4)
        shootTime += Time.deltaTime;
        shootTime_Lv2 += Time.deltaTime;

        // 공격
        if (Input.GetKey(KeyCode.Z))
        {
            // 플레이어 기본 공격
            if (shootTime > 0.12f)
            {
                GameObject playerBullet = null;

                switch (GameManager.playerLevel)
                {
                    case 1: playerBullet = pool.MakeObject("Bullet_Lv1"); break;
                    case 2: playerBullet = pool.MakeObject("Bullet_Lv2"); break;
                    case 3:
                    case 4: playerBullet = pool.MakeObject("Bullet_Lv3"); break;
                }
                playerBullet.transform.position = playerShootPos.transform.position;
                shootTime = 0f;
            }

            if (GameManager.playerLevel >= 4)
            {
                switch (playerType)
                {
                    case PlayerType.A: if (shootTime_Lv2 > 0.6f)    Fire_LvMAX_A(); break;
                    case PlayerType.B: if (shootTime_Lv2 > 0.3f)    Fire_LvMAX_B(); break;
                    case PlayerType.C: if (shootTime_Lv2 > 1f)      Fire_LvMAX_C(); break;
                }
            }
        }
    }

    void Fire_LvMAX_A()
    {
        for (int i = 0; i < 2; i++)
        {
            GameObject playerBullet = pool.MakeObject("Bullet_LvMAX");
            playerBullet.transform.position = (i == 0) ?
                    playerShootPos.transform.position + new Vector3(-0.35f, 0f) :
                    playerShootPos.transform.position + new Vector3(0.35f, 0f);
        }
        shootTime_Lv2 = 0f;
    }

    void Fire_LvMAX_B()
    {
        for (int i = 1; i < 3; i++)
        {
            for (int j = 0; j < 2; j++)
            {
                GameObject playerBullet = pool.MakeObject("Bullet_LvMAX");
                playerBullet.transform.position = (j == 0) ?
                        playerShootPos.transform.position + new Vector3(-0.4f, 0f) :
                        playerShootPos.transform.position + new Vector3(0.4f, 0f);
                playerBullet.transform.rotation = Quaternion.Euler(0, 0, (j == 0) ? 5f * i : -5f * i);
            }

        }
        shootTime_Lv2 = 0f;
    }

    void Fire_LvMAX_C()
    {
        for (int i = 0; i < 2; i++)
        {
            GameObject playerBullet = pool.MakeObject("Bullet_LvMAX");
            playerBullet.transform.position = (i == 0) ?
                    playerShootPos.transform.position + new Vector3(-0.25f, 0f) :
                    playerShootPos.transform.position + new Vector3(0.25f, 0f);
        }
        shootTime_Lv2 = 0f;
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
        GameObject Explosion = pool.MakeObject("EDestroy");
        Explosion.transform.position = transform.position;

        // PowerUp 아이템 생성
        GameObject Item = pool.MakeObject("PowerUp");
        Item.transform.position = transform.position;

        // 플레이어 레벨 1로 감소, 옵션 해제
        GameManager.playerLevel = 1;

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
