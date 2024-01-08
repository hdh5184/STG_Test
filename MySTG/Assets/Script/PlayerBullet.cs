using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerBullet : MonoBehaviour
{
    float shootTime = 0;
    void Start()
    {
        
    }

    private void OnEnable()
    {
        shootTime = 0;
    }

    void Update()
    {
        shootTime += 0.05f;
        switch (gameObject.tag)
        {
            case "PlayerBullet_Lv1":
            case "PlayerBullet_Lv2": transform.Translate(Vector2.up * 15f * Time.deltaTime); break;
            case "PlayerBullet_Lv3": transform.Translate(Vector2.up * 1f * shootTime * Time.deltaTime); break;

        }
        

        if (transform.position.y > GameManager.instance.transform.position.y + 8f)
        {
            gameObject.SetActive(false);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Field"))
        {
            gameObject.SetActive(false);
        }
    }
}
