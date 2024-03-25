using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerBullet : MonoBehaviour
{
    //public GameObject Target;
    public GameObject targetEnemy = null;

    public PlayerBulletType PBType;

    Vector3 ShootVec = Vector2.up;

    float degreeZ;
    float fieldTime = 0f;

    public enum PlayerBulletType { Bullet, Accel, Homing }

    private void OnEnable() => Init();

    void Init()
    {
        targetEnemy = null;
        ShootVec = Vector2.up;
        degreeZ = 0f;
        fieldTime = 0f;

        transform.rotation = Quaternion.Euler(0, 0, 0);
    }

    void Update()
    {
        fieldTime += Time.deltaTime;

        switch (PBType)
        {
            case PlayerBulletType.Bullet:   Move_Bullet(); break;
            case PlayerBulletType.Accel:    Move_Accel(); break;
            case PlayerBulletType.Homing:   Move_Homing(); break;
        }

        if (transform.position.y > GameManager.instance.transform.position.y + 8f)
            gameObject.SetActive(false);
    }

    void Move_Bullet() => transform.Translate(Vector2.up * 12f * Time.deltaTime);
    void Move_Accel() => transform.Translate(Vector2.up * 15f * Time.deltaTime * fieldTime);

    void Move_Homing()
    {
        SelectTarget();

        if (targetEnemy != null)
        {
            if (targetEnemy.gameObject.activeSelf == false) targetEnemy = null;
            else Homing();
        }
        transform.Translate(Vector2.up * 8f * Time.deltaTime);
    }

    void Homing()
    {
        Vector3 v1, v2, v3;

        v1 = (targetEnemy.transform.position - transform.position).normalized;

        float radian = Mathf.PI / 180 * 2;
        v2 = new Vector2(
            ShootVec.x * Mathf.Cos(radian) - ShootVec.y * Mathf.Sin(radian),
            ShootVec.x * Mathf.Sin(radian) + ShootVec.y * Mathf.Cos(radian));

        if (Vector2.Dot(ShootVec, v1) >= Vector2.Dot(ShootVec, v2))
        {
            degreeZ = Mathf.Atan2(v1.y, v1.x) / Mathf.PI * 180 - 90;
            ShootVec = v1;
        }
        else
        {
            v3 = new Vector2(
                ShootVec.x * Mathf.Cos(radian) + ShootVec.y * Mathf.Sin(radian),
                -ShootVec.x * Mathf.Sin(radian) + ShootVec.y * Mathf.Cos(radian));

            Vector3 pv = targetEnemy.transform.position - transform.position;

            if (Vector2.Dot(pv, v2) >= Vector2.Dot(pv, v3))
            {
                degreeZ = Mathf.Atan2(v2.y, v2.x) / Mathf.PI * 180 - 90;
                ShootVec = v2;
            }
            else
            {
                degreeZ = Mathf.Atan2(v3.y, v3.x) / Mathf.PI * 180 - 90;
                ShootVec = v3;
            }
        }
        transform.rotation = Quaternion.Euler(0, 0, degreeZ);
    }

    void SelectTarget()
    {
        if (targetEnemy == null)
        {
            float targetDis = 100;
            foreach (var enemy in GameManager.EnemyList)
            {
                Vector2 distanceVec = enemy.gameObject.transform.position - transform.position;
                float distance =
                    Mathf.Sqrt(Mathf.Pow(distanceVec.x, 2) + Mathf.Pow(distanceVec.y, 2));

                if (distance <= targetDis)
                {
                    targetDis = distance;
                    targetEnemy = enemy;
                }
            }
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
