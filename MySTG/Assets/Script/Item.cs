using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Item;

public class Item : MonoBehaviour
{
    Rigidbody2D rb;
    SpriteRenderer sr;

    public ItemType itemType;

    Vector2 ItemMoving;
    bool movingisLeft, movingisDown;
    float timeLimit = 0f;

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
                timeLimit = 0f;
                sr.enabled = true;
                movingisLeft = (Random.Range(-1, 1) == -1) ? true : false;
                movingisDown = true;
                ItemMoving = new Vector2(Random.Range(1f, 2f), -Random.Range(1f, 2f));
                break;
            case ItemType.SilverCoin:
            case ItemType.GoldCoin:
                rb.gravityScale = 1f;
                rb.velocity = Vector2.up * 5f;
                break;
        }

        
    }

    void Update()
    {
        switch (itemType)
        {
            case ItemType.PowerUp:      PowerUp();  break;
            case ItemType.SilverCoin:   SilverCoin(); break;
            case ItemType.GoldCoin:     GoldCoin(); break;
        }
    }

    void PowerUp()
    {
        timeLimit += Time.deltaTime;

        if (transform.position.x >= GameManager.instance.transform.position.x + 2.5f) movingisLeft = true;
        else if (transform.position.x <= GameManager.instance.transform.position.x - 2.5f) movingisLeft = false;
        if (transform.position.y >= GameManager.instance.transform.position.x + 4.5f) movingisDown = true;
        else if (transform.position.y <= GameManager.instance.transform.position.x - 4.5f) movingisDown = false;

        transform.Translate(
            new Vector2(ItemMoving.x * ((movingisLeft) ? -1 : 1), ItemMoving.y * ((movingisDown) ? 1 : -1)) * Time.deltaTime);

        if (timeLimit >= 8f) sr.enabled = !sr.enabled;
        if (timeLimit >= 10f) gameObject.SetActive(false);
    }

    void SilverCoin()
    {
        //transform.Translate(Vector2.down * 3f * Time.deltaTime);
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
