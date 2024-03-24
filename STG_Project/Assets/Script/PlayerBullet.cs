using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerBullet : MonoBehaviour
{
    //public GameObject Target;
    public GameObject targetEnemy = null;
    float degree;
    public PlayerBulletType PBType;
    float fieldTime = 0f;


    public enum PlayerBulletType
    {
        Bullet, Accel, Homing
    }

    void Start()
    {
        
    }

    private void OnEnable()
    {
        fieldTime = 0f;
        targetEnemy = null;
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
        

        if (transform.position.y > GameManager.instance.transform.position.y + 8f) gameObject.SetActive(false);
    }

    void Move_Bullet() => transform.Translate(Vector2.up * 12f * Time.deltaTime);
    void Move_Accel() => transform.Translate(Vector2.up * 15f * Time.deltaTime * fieldTime);

    void Move_Homing()
    {
        SelectTarget();

        if (targetEnemy != null)
        {
            if (targetEnemy.gameObject.activeSelf == false)
            {
                targetEnemy = null;
                transform.rotation = Quaternion.Euler(0, 0, 0);
            }
            else
            {
                GetDegree();
                transform.rotation = Quaternion.Euler(0, 0, degree);
            }
        }
        transform.Translate(Vector2.up * 8f * Time.deltaTime);

        }

    void GetDegree()
    {
        Vector2 targetPos = targetEnemy.transform.position;
        degree = Mathf.Atan2
                (targetPos.y - transform.position.y, targetPos.x - transform.position.x)
                / Mathf.PI * 180 - 90;
    }

    void SelectTarget()
    {
        if (targetEnemy == null)
        {
            float targetDis = 100;
            foreach (var enemy in GameManager.EnemyList)
            {
                float distanceX = enemy.gameObject.transform.position.x - transform.position.x;
                float distanceY = enemy.gameObject.transform.position.y - transform.position.y;
                float distance = Mathf.Sqrt(Mathf.Pow(distanceX, 2) + Mathf.Pow(distanceY, 2));

                if (distance <= targetDis)
                {
                    targetDis = distance;
                    targetEnemy = enemy;
                }
            }
            Debug.Log(targetEnemy);
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
