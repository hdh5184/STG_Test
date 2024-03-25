using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBullet : MonoBehaviour
{
    public EBulletType_Moving bulletType;

    public float enemyFromCode = 0;
    public float speed = 0f;

    Vector3 playerPos;
    Vector3 ShootVec = Vector2.down;
    float degreeZ = 0f;
    float fieldTime = 0f;

    public enum EBulletType_Moving { Straight, Accel, Homing, Bomb }

    void Init()
    {
        speed = 5f;
        ShootVec = Vector2.down;
        degreeZ = 0f;
        fieldTime = 0.5f;

        transform.rotation = Quaternion.Euler(0, 0, 0);
    }

    private void OnEnable() => Init();

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
                transform.Translate(Vector2.down * 5f * Time.deltaTime);
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
        
        Vector3 v1, v2, v3;

        v1 = (playerPos - transform.position).normalized;

        float radian = Mathf.PI / 180 * 0.5f;
        v2 = new Vector2(
            ShootVec.x * Mathf.Cos(radian) - ShootVec.y * Mathf.Sin(radian),
            ShootVec.x * Mathf.Sin(radian) + ShootVec.y * Mathf.Cos(radian));

        if (Vector2.Dot(ShootVec, v1) >= Vector2.Dot(ShootVec, v2))
        {
            degreeZ = Mathf.Atan2(v1.y, v1.x) / Mathf.PI * 180 + 90;
            ShootVec = v1;
        }
        else
        {
            v3 = new Vector2(
                ShootVec.x * Mathf.Cos(radian) + ShootVec.y * Mathf.Sin(radian),
                -ShootVec.x * Mathf.Sin(radian) + ShootVec.y * Mathf.Cos(radian));

            Vector3 pv = playerPos - transform.position;

            if (Vector2.Dot(pv, v2) >= Vector2.Dot(pv, v3))
            {
                degreeZ = Mathf.Atan2(v2.y, v2.x) / Mathf.PI * 180 + 90;
                ShootVec = v2;
            }
            else
            {
                degreeZ = Mathf.Atan2(v3.y, v3.x) / Mathf.PI * 180 + 90;
                ShootVec = v3;
            }
        }
        transform.rotation = Quaternion.Euler(0, 0, degreeZ);
    }
}
