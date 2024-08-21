using UnityEngine;
using static StageManager;

/* <Item : 점수 증가> */
public class Item_Coin : Item
{
    // 1. Rigidbody2D
    Rigidbody2D rb;

    // e. Coin 타입 모음
    public enum CoinType { Silver = 50, Gold = 250 }
    public CoinType coinType;

    protected override void Awake()
    {
        base.Awake();

        rb = GetComponent<Rigidbody2D>();
        rb.gravityScale = 0.25f;
    }

    public override void GetItem(AudioSource audio)
    {
        score += (int)coinType;
        audio.clip = audioManager.getAudioClip("GetCoin");
    }





    /*************** Enemy 충돌 로직 모음 ***************/

    /// <summary> 필드 이탈 </summary>
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Field_In"))
            gameObject.SetActive(false);
    }
}
