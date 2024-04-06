using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Item;

public class Player : MonoBehaviour
{
    // 오브젝트 pool (탄, 아이템 등)
    public PoolManager pool;
    public AudioManager audioManager;
    public StageManager stageManager;

    public AudioSource audioMain;
    public AudioSource audioSub;

    // 플레이어 샷 위치, 옵션, 무적 스프라이트
    public PlayerType playerType;
    public GameObject playerShootPos;
    public GameObject playerGuardSprite;
    public GameObject playerAudio;

    // 플레이어 이동 벡터
    //Vector3 CurrentTouchPos;
    Vector3 PlayerMovingVec;
    Vector3 TouchPos;

    float shootTime = 0, shootTime_Lv2 = 0;

    // 플레이어 인게임 상태 (평상시, 파괴됨, 무적 시간)
    public enum PlayerType { A, B, C }
    enum PlayerState { Play, Dead, Guard }
    PlayerState playerState;
    bool isShoot;

    private void Awake()
    {
        audioMain = GetComponent<AudioSource>();
        audioSub = playerAudio.GetComponent<AudioSource>();
    }

    private void OnEnable()
    {
        
    }

    void PlayerInit()
    {
        audioMain.clip = null; audioSub.clip = null;
        shootTime = 0; shootTime_Lv2 = 0;
        isShoot = false;
    }

    void Start()
    {
        // 플레이어 평상시 상태, 레벨 4일 때 옵션 표시
        playerState = PlayerState.Play;
    }

    void Update()
    {
        if (Input.GetMouseButton(0)) if (TouchScreenPos()) return;

        Move();     // 플레이어 이동
        Fire();     // 플레이어 공격
    }

    bool TouchScreenPos()
    {
        TouchPos = Camera.main.ScreenToWorldPoint(new Vector3(
                    Input.mousePosition.x, Input.mousePosition.y, -Camera.main.transform.position.z));

        if (TouchPos.x < -2.8f || TouchPos.x > 2.8f ||
            TouchPos.y < -5 || TouchPos.y > 4f) return true;
        else return false;
    }



    void Move()
    {
        if (StageManager.stageState == StageManager.StageState.End) return;

        if (Input.GetMouseButton(0))
        {
            PlayerMovingVec = TouchPos - transform.position;
            //PlayerMovingVec = (Vector3.Distance(TouchPos, transform.position) <= 1f) ?
            //    PlayerMovingVec : PlayerMovingVec.normalized;

            transform.position += PlayerMovingVec.normalized * 7f * Time.deltaTime;
        }
        else
        {
            // 방향키 입력에 따른 플레이어 이동
            PlayerMovingVec = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
            transform.Translate(PlayerMovingVec * Time.deltaTime * 4f);
        }

        // 필드 외에 나가지 않도록 이동 범위 제한
        transform.position = new Vector2(
            Mathf.Clamp(transform.position.x, -2.3f, 2.3f),
            Mathf.Clamp(transform.position.y, -4.5f, 3.75f));
    }

    void Fire()
    {
        if (StageManager.stageState == StageManager.StageState.End ||
            StageManager.stageState == StageManager.StageState.Pause)
        {
            isShoot = false;
            audioMain.loop = false;
            audioMain.Stop();
            return;
        }

        // 기본탄(Lv1~3), 강화탄(Lv4)
        shootTime += Time.deltaTime;
        shootTime_Lv2 += Time.deltaTime;

        // 공격
        if (Input.GetKey(KeyCode.Z) || Input.GetMouseButton(0))
        {
            if (!isShoot)
            {
                isShoot = true;
                audioMain.clip = audioManager.getAudioClip("PShoot");
                audioMain.loop = true;
                audioMain.Play();
            }
            
            // 플레이어 기본 공격
            if (shootTime > 0.12f)
            {
                GameObject playerBullet = null;

                switch (StageManager.playerLevel)
                {
                    case 1: playerBullet = pool.MakeObject("Bullet_Lv1"); break;
                    case 2: playerBullet = pool.MakeObject("Bullet_Lv2"); break;
                    case 3:
                    case 4: playerBullet = pool.MakeObject("Bullet_Lv3"); break;
                }
                playerBullet.transform.position = playerShootPos.transform.position;
                shootTime = 0f;
            }

            if (StageManager.playerLevel >= 4)
            {
                switch (playerType)
                {
                    case PlayerType.A: if (shootTime_Lv2 > 0.6f)    Fire_LvMAX_A(); break;
                    case PlayerType.B: if (shootTime_Lv2 > 0.3f)    Fire_LvMAX_B(); break;
                    case PlayerType.C: if (shootTime_Lv2 > 1f)      Fire_LvMAX_C(); break;
                }
            }
        }
        else
        {
            isShoot = false;
            audioMain.loop = false;
            audioMain.Stop();
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
        if (playerState == PlayerState.Play && collision.CompareTag("EnemyBullet"))
        {
            Debug.Log("플레이어 파괴됨");

            DestroyPlayer();

            if (StageManager.playerHealth < 0)
            {
                Debug.Log("게임 끝");
                StageManager.instance.GameDefeat();
            }
            else Invoke("ReloadPlayer", 1.5f);
            collision.gameObject.SetActive(false);
        }
            
        if (playerState != PlayerState.Dead && collision.CompareTag("Item"))
        {
            Item item = collision.GetComponent<Item>();

            switch (item.itemType)
            {
                case ItemType.PowerUp:
                    StageManager.playerLevel = (StageManager.playerLevel == 4) ? 4 : StageManager.playerLevel + 1;
                    audioSub.clip = audioManager.getAudioClip("GetItem"); break;
                case ItemType.SilverCoin:
                    StageManager.Score += 50;
                    audioSub.clip = audioManager.getAudioClip("GetCoin"); break;
                case ItemType.GoldCoin:
                    StageManager.Score += 250;
                    audioSub.clip = audioManager.getAudioClip("GetCoin"); break;
            }
            audioSub.Play();
            collision.gameObject.SetActive(false);
        }
    }

    // 플레이어 기체 파괴
    private void DestroyPlayer()
    {
        playerState = PlayerState.Dead;
        audioMain.Stop();
        audioSub.Stop();
        audioMain.clip = null;
        audioSub.clip = null;

        gameObject.SetActive(false);

        // 폭발 연출 생성
        GameObject Explosion = pool.MakeObject("ExplodeA");
        Explosion.transform.position = transform.position;

        AudioSource effectAudio = Explosion.GetComponent<Effect>().audio;
        effectAudio.clip = audioManager.getAudioClip("ExplodePlayer");

        // PowerUp 아이템 생성
        GameObject Item = pool.MakeObject("PowerUp");
        Item.transform.position = transform.position;

        // 플레이어 레벨 1로 감소, 옵션 해제
        StageManager.playerLevel = (StageManager.playerLevel <= 2) ?
            1 : StageManager.playerLevel - 2;
        StageManager.playerHealth--;
    }

    // 무적 상태로 재생성
    void ReloadPlayer()
    {
        playerState = PlayerState.Guard;

        transform.position = GameManager.instance.transform.position + new Vector3(0, -3f);
        playerGuardSprite.SetActive(true);
        gameObject.SetActive(true);

        // 2.5초 후 무적 해제
        Invoke("Unguard", 2.5f);
        Debug.Log("플레이어 재출격");
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
            DestroyPlayer();
            Invoke("ReloadPlayer", 1.5f);
        }
    }
}
