using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Item;

public class Item : MonoBehaviour
{
    Rigidbody2D rb;
    SpriteRenderer sr;

    public ItemType itemType;

    // 아이템 이동 벡터, 방향, 출현 시간
    Vector2 ItemMoving;
    bool movingisLeft, movingisDown;
    float fieldTime = 0f;

    public enum ItemType
    {
        PowerUp, Heal,
        SilverCoin, GoldCoin
    }

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        sr = GetComponent<SpriteRenderer>();
    }

    private void OnEnable()
    {
        switch (itemType)
        {
            case ItemType.PowerUp:      // 플레이어 레벨 증가
                fieldTime = 0f;
                sr.enabled = true;
                movingisLeft = (Random.Range(-1, 1) == -1) ? true : false;
                movingisDown = true;
                ItemMoving = new Vector2(Random.Range(1f, 2f), -Random.Range(3f, 4f));
                break;
            case ItemType.SilverCoin:   // 점수 아이템 소형	
            case ItemType.GoldCoin:     // 점수 아이템 대형
                rb.gravityScale = 0.5f;
                rb.velocity = Vector2.up * 3f;
                break;
        }
    }

    void Update()
    {
        fieldTime += Time.deltaTime;

        switch (itemType)
        {
            case ItemType.PowerUp:      PowerUp();  break;
        }
    }

    void PowerUp()
    {
        if (transform.position.x >= GameManager.instance.transform.position.x + 2.5f) movingisLeft = true;
        else if (transform.position.x <= GameManager.instance.transform.position.x - 2.5f) movingisLeft = false;
        if (transform.position.y >= GameManager.instance.transform.position.x + 4.5f) movingisDown = true;
        else if (transform.position.y <= GameManager.instance.transform.position.x - 4.5f) movingisDown = false;

        transform.Translate(
            new Vector2(ItemMoving.x * ((movingisLeft) ? -1 : 1), ItemMoving.y * ((movingisDown) ? 1 : -1)) * Time.deltaTime);

        if (fieldTime >= 8f) sr.enabled = !sr.enabled;
        if (fieldTime >= 10f) gameObject.SetActive(false);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            switch (itemType)
            {
                case ItemType.PowerUp:
                    GameManager.playerLevel = (GameManager.playerLevel == 4) ? 4 : GameManager.playerLevel + 1;
                    break;
                case ItemType.SilverCoin:
                    GameManager.Score += 50; break;
                case ItemType.GoldCoin:
                    GameManager.Score += 150; break;
            }
            gameObject.SetActive(false);
        }

        if (collision.CompareTag("Field_In"))
        {
            gameObject.SetActive(false);
        }
    }
}
