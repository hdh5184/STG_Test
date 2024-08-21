using UnityEngine;
using static StageManager;

/* <Item : 플레이어 공격력 강화> */
public class Item_PowerUp : Item
{
    // 1. SpriteRenderer
    SpriteRenderer sr;

    // 2. 이동 속성
    Vector2 posVec;
    Vector2 moveVec;
    float fieldTime;
    
    bool flipX, flipY;

    protected override void Awake()
    {
        base.Awake();
        sr = GetComponent<SpriteRenderer>();
    }

    protected override void OnEnable()
    {
        sr.enabled = true;
        fieldTime = 0f;

        float ValueX, ValueY;

        // 이동 방향 설정
        flipX = Random.value < 0.5f;
        flipY = true;

        ValueX = Random.Range(1f, 1.5f);
        ValueY = Random.Range(2f, 3f);
        ValueX *= (flipX) ? -1 : 1;
        ValueY *= (flipY) ? -1 : 1;

        posVec = transform.position;
        moveVec = new Vector2(ValueX, ValueY);
    }

    protected override void Update()
    {
        Timing();
        Move();
        Compare_Clamp();
        Compare_Exit();
    }





    /*************** 로직 모음 ***************/

    /// <summary> 시간 갱신 </summary>
    void Timing() { fieldTime += Time.deltaTime; }

    /// <summary> 이동 로직 </summary>
    void Move() { transform.Translate(moveVec * Time.deltaTime); }

    /// <summary> 필드 이탈 방지 검사 </summary>
    void Compare_Clamp()
    {
        posVec = transform.position;
        ClampX();
        ClampY();
    }

    /// <summary> 이동 벡터 x 재조정 </summary>
    void ClampX()
    {
        if (posVec.x < 2.5f && posVec.x > -2.5f) return;

        flipX = !flipX;
        moveVec.x = Random.Range(1f, 1.5f);
        moveVec.x *= (flipX) ? -1 : 1;
        transform.position =
        new Vector2(Mathf.Clamp(posVec.x, -2.5f, 2.5f), posVec.y);
    }

    /// <summary> 이동 벡터 y 재조정 </summary>
    void ClampY()
    {
        if (posVec.y < 3f && posVec.y > -4.5f) return;

        flipY = !flipY;
        moveVec.y = Random.Range(2f, 3f);
        moveVec.y *= (flipY) ? -1 : 1;
        transform.position =
        new Vector2(posVec.x, Mathf.Clamp(posVec.y, -4.5f, 3f));
    }

    /// <summary> 필드 탈출 검사 로직 </summary>
    void Compare_Exit()
    {
        if (fieldTime >= 8f) sr.enabled = !sr.enabled;
        if (fieldTime >= 10f) gameObject.SetActive(false);
    }

    public override void GetItem(AudioSource audio)
    {
        if (playerLevel != 4) playerLevel++;
        audio.clip = audioManager.getAudioClip("GetItem");
    }
}
