using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBullet : MonoBehaviour
{
    public EBulletType_Moving bulletType;

    public float enemyFromCode = 0;

    Vector2 playerPos;
    float degreeZ = 0f;
    float fieldTime = 0f;
    float speed = 0f;

    public enum EBulletType_Moving
    {
        Straight, Accel, Homing, Bomb
    }


    void Start()
    {
        
    }

    private void OnEnable()
    {
        speed = 5f;
        fieldTime = 0.5f;
        degreeZ = 0f;
    }

    void Update()
    {
        fieldTime += Time.deltaTime;
        MovingBullet();
        //EnemyDestroyCompare();
    }

    void MovingBullet()
    {
        switch (bulletType)
        {
            case EBulletType_Moving.Straight:
                transform.Translate(Vector2.down * speed * Time.deltaTime); break;
            case EBulletType_Moving.Accel:
                transform.Translate(Vector2.down * speed * Time.deltaTime * fieldTime); break;
            case EBulletType_Moving.Homing:
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
        playerPos = GameManager.instance.playerPos;
        float degTemp = degreeZ;
        float degLimit = (Mathf.Atan2
                (playerPos.y - transform.position.y, playerPos.x - transform.position.x)
                / Mathf.PI * 180 + 90);
        Debug.Log(degLimit - degTemp);
        float degTurn = degLimit - degTemp;

        if      (degTurn < -0.5f)   degreeZ -= 0.5f;
        else if (degTurn > 0.5f)    degreeZ += 0.5f;
        else                        degreeZ += degTurn;


        transform.rotation = Quaternion.Euler(0, 0, degreeZ);
    }
}
