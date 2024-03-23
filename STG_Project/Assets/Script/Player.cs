using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    // 오브젝트 pool (탄, 아이템 등)
    public PoolManager pool;

    // 플레이어 샷 위치, 옵션, 무적 스프라이트
    public GameObject playerShootPos;
    public GameObject playerGuardSprite;

    // 플레이어 이동 벡터
    Vector2 PlayerMovingVec;

    float shootTime = 0, shootTime_Lv2 = 0;

    // 플레이어 인게임 상태 (평상시, 파괴됨, 무적 시간)
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
        Compare();  // 플레이어 레벨 확인
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
            if (shootTime > 0.07f)
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

            // 플레이어 Lv4일 때 강화탄 공격
            /*
            if (GameManager.playerLevel >= 4)
            {
                if (shootTime_Lv2 > 0.2f)
                {
                    int Lv3Count = 0;
                    while (Lv3Count < 2)
                    {
                        GameObject playerBullet = pool.MakeObject("Bullet_Lv3");
                        playerBullet.transform.position = (Lv3Count == 0) ?
                                playerShootPos.transform.position + new Vector3(-0.35f, 0f) :
                                playerShootPos.transform.position + new Vector3(0.35f, 0f);
                        Lv3Count++;
                    }
                    shootTime_Lv2 = 0f;
                }

            }
            */
        }
    }

    void Compare()
    {
        // 플레이어 평상시 상태, 레벨 4일 때 옵션 표시
        
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

        // PowerUp 아이템 2개 생성
        int summonItemLimit = 0;
        while (summonItemLimit < 2)
        {
            GameObject Item = pool.MakeObject("PowerUp");
            Item.transform.position = transform.position;
            summonItemLimit++;
        }

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
