using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EBullet_BG1 : MonoBehaviour
{
    //public PoolManager outputBulletPool;
    PoolManager PoolInstance;

    float summonTime = 0f;

    private void OnEnable()
    {
        summonTime = 0f;
    }

    void Start()
    {
        PoolInstance = PoolManager.instance.gameObject.GetComponent<PoolManager>();
    }

    void Update()
    {
        summonTime += Time.deltaTime;

        if (summonTime >= 0.2f)
        {
            for (int i = 0; i < PoolInstance.poolEBullet_SG1.Length; i++)
            {
                GameObject bullet = PoolInstance.poolEBullet_SG1[i];
                if (bullet.activeSelf == false)
                {
                    bullet.transform.position = transform.position;
                    bullet.transform.rotation = Quaternion.Euler(0, 0, Boss.degreeCos);
                    bullet.SetActive(true);
                    break;
                }
            }
            summonTime = 0f;
        }
    }
}
