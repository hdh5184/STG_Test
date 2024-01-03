using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBullet : MonoBehaviour
{
    public EBulletType bulletType;

    public enum EBulletType
    {
        Big_G1,
        Small_G1, Small_B1, Small_P1
    }

    Vector2 playerVec;

    void Start()
    {
        
    }

    void Update()
    {
        MovingBullet();
    }

    void MovingBullet()
    {
        switch (bulletType)
        {
            case EBulletType.Big_G1:
                transform.Translate(Vector2.down * 5f * Time.deltaTime);
                break;
            case EBulletType.Small_G1:
                transform.Translate(Vector2.down * 5f * Time.deltaTime);
                break;
            case EBulletType.Small_B1:
                transform.Translate(Vector2.down * 7f * Time.deltaTime);
                break;
            case EBulletType.Small_P1:
                transform.Translate(Vector2.down * 5f * Time.deltaTime);
                break;
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
