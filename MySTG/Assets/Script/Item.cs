using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Item;

public class Item : MonoBehaviour
{
    Rigidbody2D rb;
    SpriteRenderer sr;

    public ItemType itemType;

    Vector3 playerVec;
    bool coinPosLR; // L : true / R = false

    Vector2 ItemMoving;
    bool movingisLeft, movingisDown;
    float fieldTime = 0f;

    Vector2 CoinMoving;
    float playerCoinDis;
    float coinStretch;

    public enum ItemType
    {
        PowerUp, SilverCoin, GoldCoin
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
            case ItemType.PowerUp:
                fieldTime = 0f;
                sr.enabled = true;
                movingisLeft = (Random.Range(-1, 1) == -1) ? true : false;
                movingisDown = true;
                ItemMoving = new Vector2(Random.Range(1f, 2f), -Random.Range(1f, 2f));
                break;
            case ItemType.SilverCoin:
            case ItemType.GoldCoin:
                coinPosLR = (transform.position.x <= GameManager.instance.playerPos.x) ? true : false;
                rb.gravityScale = 1f;
                rb.velocity = Vector2.up * 5f;
                break;
        }

        
    }

    void Update()
    {
        fieldTime += Time.deltaTime;

        switch (itemType)
        {
            case ItemType.PowerUp:      PowerUp();  break;
            case ItemType.SilverCoin:   SilverCoin(); break;
            case ItemType.GoldCoin:     GoldCoin(); break;
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

    void SilverCoin()
    {
        playerVec = GameManager.instance.playerPos;
        if (fieldTime >= 0.5f)
        {
            CoinMoving = playerVec - transform.position;
            playerCoinDis = Vector2.Distance(playerVec, transform.position);
            coinStretch = (2 / playerCoinDis);
            rb.AddForce(coinStretch * CoinMoving, ForceMode2D.Force);
            transform.position = new Vector2(Mathf.Clamp(transform.position.x,
                (coinPosLR) ? transform.position.x : playerVec.x,
                (!coinPosLR) ? transform.position.x : playerVec.x),
                transform.position.y);
        }
    }

    void GoldCoin()
    {

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
                    GameManager.Score += 100; break;
                case ItemType.GoldCoin:
                    GameManager.Score += 300; break;
            }
            
            gameObject.SetActive(false);
        }
    }
}
