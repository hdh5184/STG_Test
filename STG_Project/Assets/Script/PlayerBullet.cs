using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerBullet : MonoBehaviour
{
    public PlayerBulletType PBType;

    public enum PlayerBulletType
    {
        Bullet
    }

    void Start()
    {
        
    }

    private void OnEnable()
    {
        
    }

    void Update()
    {
        switch (PBType)
        {
            case PlayerBulletType.Bullet:   transform.Translate(Vector2.up * 12f * Time.deltaTime); break;
        }
        

        if (transform.position.y > GameManager.instance.transform.position.y + 8f) gameObject.SetActive(false);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Field"))
        {
            gameObject.SetActive(false);
        }
    }
}
