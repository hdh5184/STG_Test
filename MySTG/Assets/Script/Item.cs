using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Item;

public class Item : MonoBehaviour
{
    public ItemType itemType;

    Vector2 ItemMoving;
    bool movingisLeft, movingisUp;

    public enum ItemType
    {
        PowerUp
    }

    void Start()
    {
        
    }

    void Update()
    {
        if      (transform.position.x >= GameManager.instance.transform.position.x + 2.5f) movingisLeft = true;
        else if (transform.position.x <= GameManager.instance.transform.position.x - 2.5f) movingisLeft = false;
        else if (transform.position.y <= GameManager.instance.transform.position.x - 4.5f) movingisUp = true;
        if      (transform.position.y >= GameManager.instance.transform.position.x + 4.5f) movingisUp = false;

        transform.Translate(
            new Vector2(ItemMoving.x * ((movingisLeft) ? -1 : 1), ItemMoving.y * ((movingisUp) ? 1 : -1)) * Time.deltaTime);
    }

    private void OnEnable()
    {
        movingisLeft = (Random.Range(-1, 1) == -1)? true : false;
        movingisUp = (Random.Range(-1, 1) == 0) ? true : false;
        ItemMoving = new Vector2(Random.Range(1f, 2f), Random.Range(1f, 2f));
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
            }
            
            gameObject.SetActive(false);
        }
    }
}
