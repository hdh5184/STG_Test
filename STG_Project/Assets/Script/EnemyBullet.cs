using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBullet : MonoBehaviour
{
    public EBulletType bulletType;

    public float enemyFromCode = 0;

    public enum EBulletType
    {
        Big_G1,
        Small_G1, Small_B1, Small_P1,
        Homing
    }

    Vector2 playerPos;
    float degree = 0f;

    void Start()
    {
        
    }

    private void OnEnable()
    {
        
    }

    void Update()
    {
        MovingBullet();
        //EnemyDestroyCompare();
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
            case EBulletType.Homing:
                Homing();
                transform.Translate(Vector2.down * 3f * Time.deltaTime);
                break;
        }
    }

    void EnemyDestroyCompare()
    {
        bool findEnemy = false;
        foreach (var code in GameManager.EnemyCode)
        {
            if (code == enemyFromCode) { findEnemy = true; break; }
        }

        if (!findEnemy)
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

    void Homing()
    {
        float deg2 = degree;
        playerPos = GameManager.instance.playerPos;
        degree = (Mathf.Atan2
                (playerPos.y - transform.position.y, playerPos.x - transform.position.x)
                / Mathf.PI * 180 + 90);
        Debug.Log(degree - deg2);
        transform.rotation = Quaternion.Euler(0, 0, degree);
    }
}
