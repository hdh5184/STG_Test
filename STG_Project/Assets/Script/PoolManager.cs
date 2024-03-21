using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PoolManager : MonoBehaviour
{
    public static PoolManager instance;


    public GameObject[] PlayerBullet;

    public GameObject[] EBulletSmall;
    public GameObject[] EBulletMedium;

    public GameObject[] Item;
    public GameObject[] Effect;

    public GameObject[] EnemySmall;
    public GameObject[] EnemyMedium;
    public GameObject[] EnemyLarge;
    public GameObject[] EnemyBig;



    public GameObject[]
        poolPBullet_Lv1, poolPBullet_Lv2, poolPBullet_Lv3, poolPBullet_Lv4;

    public GameObject[]
        poolEBulletSmall_A, poolEBulletSmall_B;
    public GameObject[]
        poolEBulletMedium_A, poolEBulletMedium_B;

    public GameObject[] poolItem_PowerUp;
    public GameObject[] poolItem_SilverCoin;
    public GameObject[] poolItem_GoldCoin;

    public GameObject[] poolEffect_Destroy;

    public GameObject[]
        poolEnemySmall_A, poolEnemySmall_B, poolEnemySmall_C, poolEnemySmall_D, poolEnemySmall_E;
    public GameObject[]
        poolEnemyMedium_A, poolEnemyMedium_B, poolEnemyMedium_C;
    public GameObject[]
        poolEnemyLarge_A, poolEnemyLarge_B;
    public GameObject[]
        poolEnemyBig_A;

    public GameObject[] targetPool;

    private void Awake()
    {
        if (instance == null) instance = this;

        MakePool(PlayerBullet[0], ref poolPBullet_Lv1, 20);
        MakePool(PlayerBullet[1], ref poolPBullet_Lv2, 20);
        MakePool(PlayerBullet[2], ref poolPBullet_Lv3, 20);
        MakePool(PlayerBullet[3], ref poolPBullet_Lv4, 80);

        MakePool(EBulletSmall[0], ref poolEBulletSmall_A, 200);
        MakePool(EBulletSmall[1], ref poolEBulletSmall_B, 200);
        MakePool(EBulletMedium[0], ref poolEBulletMedium_A, 100);
        MakePool(EBulletMedium[1], ref poolEBulletMedium_B, 100);

        MakePool(Item[0], ref poolItem_PowerUp, 8);
        MakePool(Item[1], ref poolItem_SilverCoin, 300);
        MakePool(Item[2], ref poolItem_GoldCoin, 300);
        MakePool(Effect[0], ref poolEffect_Destroy, 25);

        MakePool(EnemySmall[0], ref poolEnemySmall_A, 15);
        MakePool(EnemySmall[1], ref poolEnemySmall_B, 15);
        MakePool(EnemySmall[2], ref poolEnemySmall_C, 15);
        MakePool(EnemySmall[3], ref poolEnemySmall_D, 15);
        MakePool(EnemySmall[4], ref poolEnemySmall_E, 15);

        MakePool(EnemyMedium[0], ref poolEnemyMedium_A, 8);
        MakePool(EnemyMedium[1], ref poolEnemyMedium_B, 8);
        MakePool(EnemyMedium[2], ref poolEnemyMedium_C, 8);

        MakePool(EnemyLarge[0], ref poolEnemyLarge_A, 5);
        MakePool(EnemyLarge[1], ref poolEnemyLarge_B, 5);

        MakePool(EnemyBig[0], ref poolEnemyBig_A, 3);
    }

    public void MakePool(GameObject input, ref GameObject[] pool, int count)
    {
        pool = new GameObject[count];
        for (int i = 0; i < pool.Length; i++)
        {
            pool[i] = Instantiate(input);
            pool[i].SetActive(false);
        }
    }

    public GameObject MakeObject(string obj)
    {
        switch (obj)
        {
            case "Bullet_Lv1": targetPool = poolPBullet_Lv1; break;
            case "Bullet_Lv2": targetPool = poolPBullet_Lv2; break;
            case "Bullet_Lv3": targetPool = poolPBullet_Lv3; break;
            case "Bullet_Lv4": targetPool = poolPBullet_Lv4; break;

            case "EBS_A": targetPool = poolEBulletSmall_A; break;
            case "EBS_B": targetPool = poolEBulletSmall_B; break;
            case "EBM_A": targetPool = poolEBulletMedium_A; break;
            case "EBM_B": targetPool = poolEBulletMedium_B; break;

            case "PowerUp": targetPool = poolItem_PowerUp; break;
            case "SilverCoin": targetPool = poolItem_SilverCoin; break;
            case "GoldCoin": targetPool = poolItem_GoldCoin; break;

            case "EDestroy": targetPool = poolEffect_Destroy; break;

            case "EnemyS_A": targetPool = poolEnemySmall_A; break;
            case "EnemyS_B": targetPool = poolEnemySmall_B; break;
            case "EnemyS_C": targetPool = poolEnemySmall_C; break;
            case "EnemyS_D": targetPool = poolEnemySmall_D; break;
            case "EnemyS_E": targetPool = poolEnemySmall_E; break;

            case "EnemyM_A": targetPool = poolEnemyMedium_A; break;
            case "EnemyM_B": targetPool = poolEnemyMedium_B; break;
            case "EnemyM_C": targetPool = poolEnemyMedium_C; break;

            case "EnemyL_A": targetPool = poolEnemyLarge_A; break;
            case "EnemyL_B": targetPool = poolEnemyLarge_B; break;

            case "EnemyB_A": targetPool = poolEnemyBig_A; break;

            default:         targetPool = null; break;
        }

        if (targetPool != null)
        {
            for (int i = 0; i < targetPool.Length; i++)
            {
                if (!targetPool[i].activeSelf)
                {
                    targetPool[i].SetActive(true);
                    return targetPool[i];
                }
            }
        }

        Debug.Log($"\"{obj}\" 오브젝트를 찾을 수 없습니다.");
        return null;
    }
}
