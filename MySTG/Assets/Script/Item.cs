using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Item;

public class Item : MonoBehaviour
{
    SpriteRenderer renderer;

    public ItemType itemType;

    Vector2 ItemMoving;
    bool movingisLeft, movingisDown;
    float timeLimit = 0f;

    public enum ItemType
    {
        PowerUp
    }

    void Awake()
    {
        renderer = GetComponent<SpriteRenderer>();
    }

    void Update()
    {
        timeLimit += Time.deltaTime;

        if      (transform.position.x >= GameManager.instance.transform.position.x + 2.5f) movingisLeft = true;
        else if (transform.position.x <= GameManager.instance.transform.position.x - 2.5f) movingisLeft = false;
        if      (transform.position.y >= GameManager.instance.transform.position.x + 4.5f) movingisDown = true;
        else if (transform.position.y <= GameManager.instance.transform.position.x - 4.5f) movingisDown = false;

        transform.Translate(
            new Vector2(ItemMoving.x * ((movingisLeft) ? -1 : 1), ItemMoving.y * ((movingisDown) ? 1 : -1)) * Time.deltaTime);

        if (timeLimit >= 8f)    renderer.enabled = !renderer.enabled;
        if (timeLimit >= 10f)   gameObject.SetActive(false);
    }

    private void OnEnable()
    {
        timeLimit = 0f;
        renderer.enabled = true;
        movingisLeft = (Random.Range(-1, 1) == -1)? true : false;
        movingisDown = true;
        ItemMoving = new Vector2(Random.Range(1f, 2f), -Random.Range(1f, 2f));
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
