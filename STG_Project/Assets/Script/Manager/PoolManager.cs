using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PoolManager : MonoBehaviour
{
    public static PoolManager instance;

    // 1. 탄 Prefab
    public GameObject[] PlayerBulletA;
    public GameObject[] PlayerBulletB;
    public GameObject[] PlayerBulletC;

    public GameObject[] EBulletSmall;
    public GameObject[] EBulletMedium;

    // 2. 아이템 & 이펙트 Prefab
    public GameObject[] Item;
    public GameObject[] Effect;

    // 3. 플레이어 Prefab
    public GameObject[] Player;

    // 4. Enemy Prefab
    public GameObject[] EnemySmall;
    public GameObject[] EnemyMedium;
    public GameObject[] EnemyLarge;
    public GameObject[] EnemyBig;
    public GameObject[] EnemyBoss;


    // A. 탄 Pool
    public GameObject[]
        poolPBulletA_Lv1, poolPBulletA_Lv2, poolPBulletA_Lv3, poolPBulletA_LvMAX;
    public GameObject[]
        poolPBulletB_Lv1, poolPBulletB_Lv2, poolPBulletB_Lv3, poolPBulletB_LvMAX;
    public GameObject[]
        poolPBulletC_Lv1, poolPBulletC_Lv2, poolPBulletC_Lv3, poolPBulletC_LvMAX;

    public GameObject[]
        poolEBulletSmall_A, poolEBulletSmall_B;
    public GameObject[]
        poolEBulletMedium_A, poolEBulletMedium_B;

    // B. 아이템 & 이펙트 Pool
    public GameObject[] poolItem_PowerUp;
    public GameObject[] poolItem_Heal;
    public GameObject[] poolItem_SilverCoin;
    public GameObject[] poolItem_GoldCoin;

    public GameObject[] poolEffect_Explode_A_Boss;
    public GameObject[]
        poolEffect_Explode_A, poolEffect_Explode_B, poolEffect_Explode_C;
    public GameObject[]
        poolEffect_Explode_Short, poolEffect_Smoke;

    // C. 플레이어 Pool
    public GameObject[]
        poolPlayer_A, poolPlayer_B, poolPlayer_C;

    // D. Enemy Pool
    public GameObject[]
        poolEnemySmall_A, poolEnemySmall_B, poolEnemySmall_C, poolEnemySmall_D, poolEnemySmall_E;
    public GameObject[]
        poolEnemyMedium_A, poolEnemyMedium_B, poolEnemyMedium_C;
    public GameObject[]
        poolEnemyLarge_A, poolEnemyLarge_B;
    public GameObject[]
        poolEnemyBig_A, poolEnemyBig_B;
    public GameObject[]
        poolEnemyBoss_A, poolEnemyBoss_B, poolEnemyBoss_C, poolEnemyBoss_D;

    // @. 생성할 오브젝트 target 지정
    public GameObject[] targetPool;

    private void Awake()
    {
        if (instance != this && instance != null)
        {
            Destroy(gameObject); return;
        }
        else
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }

        InitPool();
    }

    /// <summary> Pool 초기화 </summary>
    private void InitPool()
    {
        if (instance == null) instance = this;

        MakePool(PlayerBulletA[0], ref poolPBulletA_Lv1, 15);
        MakePool(PlayerBulletA[1], ref poolPBulletA_Lv2, 15);
        MakePool(PlayerBulletA[2], ref poolPBulletA_Lv3, 15);
        MakePool(PlayerBulletA[3], ref poolPBulletA_LvMAX, 16);
        MakePool(PlayerBulletB[0], ref poolPBulletB_Lv1, 15);
        MakePool(PlayerBulletB[1], ref poolPBulletB_Lv2, 15);
        MakePool(PlayerBulletB[2], ref poolPBulletB_Lv3, 15);
        MakePool(PlayerBulletB[3], ref poolPBulletB_LvMAX, 40);
        MakePool(PlayerBulletC[0], ref poolPBulletC_Lv1, 15);
        MakePool(PlayerBulletC[1], ref poolPBulletC_Lv2, 15);
        MakePool(PlayerBulletC[2], ref poolPBulletC_Lv3, 15);
        MakePool(PlayerBulletC[3], ref poolPBulletC_LvMAX, 16);

        MakePool(EBulletSmall[0], ref poolEBulletSmall_A, 200);
        MakePool(EBulletSmall[1], ref poolEBulletSmall_B, 200);
        MakePool(EBulletMedium[0], ref poolEBulletMedium_A, 200);
        MakePool(EBulletMedium[1], ref poolEBulletMedium_B, 200);

        MakePool(Item[0], ref poolItem_PowerUp, 8);
        MakePool(Item[1], ref poolItem_Heal, 8);
        MakePool(Item[2], ref poolItem_SilverCoin, 300);
        MakePool(Item[3], ref poolItem_GoldCoin, 300);

        MakePool(Effect[0], ref poolEffect_Explode_A_Boss, 1);

        MakePool(Effect[0], ref poolEffect_Explode_A, 20);
        MakePool(Effect[1], ref poolEffect_Explode_B, 20);
        MakePool(Effect[2], ref poolEffect_Explode_C, 20);
        MakePool(Effect[3], ref poolEffect_Explode_Short, 20);
        MakePool(Effect[4], ref poolEffect_Smoke, 20);

        MakePool(Player[0], ref poolPlayer_A, 1);
        MakePool(Player[1], ref poolPlayer_B, 1);
        MakePool(Player[2], ref poolPlayer_C, 1);

        MakePool(EnemySmall[0], ref poolEnemySmall_A, 30);
        MakePool(EnemySmall[1], ref poolEnemySmall_B, 30);
        MakePool(EnemySmall[2], ref poolEnemySmall_C, 30);
        MakePool(EnemySmall[3], ref poolEnemySmall_D, 30);
        MakePool(EnemySmall[4], ref poolEnemySmall_E, 30);

        MakePool(EnemyMedium[0], ref poolEnemyMedium_A, 8);
        MakePool(EnemyMedium[1], ref poolEnemyMedium_B, 8);
        MakePool(EnemyMedium[2], ref poolEnemyMedium_C, 8);

        MakePool(EnemyLarge[0], ref poolEnemyLarge_A, 5);
        MakePool(EnemyLarge[1], ref poolEnemyLarge_B, 5);

        MakePool(EnemyBig[0], ref poolEnemyBig_A, 3);
        MakePool(EnemyBig[1], ref poolEnemyBig_B, 3);

        MakePool(EnemyBoss[0], ref poolEnemyBoss_A, 1);
        MakePool(EnemyBoss[1], ref poolEnemyBoss_B, 1);
        MakePool(EnemyBoss[2], ref poolEnemyBoss_C, 1);
        MakePool(EnemyBoss[3], ref poolEnemyBoss_D, 1);
    }

    /// <summary> Pool 생성 </summary>
    public void MakePool(GameObject input, ref GameObject[] pool, int count)
    {
        pool = new GameObject[count];
        for (int i = 0; i < pool.Length; i++)
        {
            pool[i] = Instantiate(input, transform);
            pool[i].SetActive(false);
        }
    }

    /// <summary> Pool - 오브젝트 선택 </summary>
    public GameObject MakeObject(string obj, bool isSetActive = true)
    {
        switch (obj)
        {
            case "BulletA_Lv1":  targetPool = poolPBulletA_Lv1; break;
            case "BulletA_Lv2":  targetPool = poolPBulletA_Lv2; break;
            case "BulletA_Lv3":  targetPool = poolPBulletA_Lv3; break;
            case "BulletA_LvMAX":targetPool = poolPBulletA_LvMAX; break;
            case "BulletB_Lv1": targetPool = poolPBulletB_Lv1; break;
            case "BulletB_Lv2": targetPool = poolPBulletB_Lv2; break;
            case "BulletB_Lv3": targetPool = poolPBulletB_Lv3; break;
            case "BulletB_LvMAX": targetPool = poolPBulletB_LvMAX; break;
            case "BulletC_Lv1": targetPool = poolPBulletC_Lv1; break;
            case "BulletC_Lv2": targetPool = poolPBulletC_Lv2; break;
            case "BulletC_Lv3": targetPool = poolPBulletC_Lv3; break;
            case "BulletC_LvMAX": targetPool = poolPBulletC_LvMAX; break;

            case "EBS_A":       targetPool = poolEBulletSmall_A; break;
            case "EBS_B":       targetPool = poolEBulletSmall_B; break;
            case "EBM_A":       targetPool = poolEBulletMedium_A; break;
            case "EBM_B":       targetPool = poolEBulletMedium_B; break;

            case "PowerUp":     targetPool = poolItem_PowerUp; break;
            case "Heal":        targetPool = poolItem_Heal; break;
            case "SilverCoin":  targetPool = poolItem_SilverCoin; break;
            case "GoldCoin":    targetPool = poolItem_GoldCoin; break;

            case "ExplodeA":    targetPool = poolEffect_Explode_A; break;
            case "ExplodeB":    targetPool = poolEffect_Explode_B; break;
            case "ExplodeC":    targetPool = poolEffect_Explode_C; break;
            case "ExplodeShort":targetPool = poolEffect_Explode_Short; break;
            case "Smoke":       targetPool = poolEffect_Smoke; break;

            case "ExplodeBoss": targetPool = poolEffect_Explode_A_Boss; break;

            case "Player_A": targetPool = poolPlayer_A; break;
            case "Player_B": targetPool = poolPlayer_B; break;
            case "Player_C": targetPool = poolPlayer_C; break;

            case "EnemyS_A":    targetPool = poolEnemySmall_A; break;
            case "EnemyS_B":    targetPool = poolEnemySmall_B; break;
            case "EnemyS_C":    targetPool = poolEnemySmall_C; break;
            case "EnemyS_D":    targetPool = poolEnemySmall_D; break;
            case "EnemyS_E":    targetPool = poolEnemySmall_E; break;

            case "EnemyM_A":    targetPool = poolEnemyMedium_A; break;
            case "EnemyM_B":    targetPool = poolEnemyMedium_B; break;
            case "EnemyM_C":    targetPool = poolEnemyMedium_C; break;

            case "EnemyL_A":    targetPool = poolEnemyLarge_A; break;
            case "EnemyL_B":    targetPool = poolEnemyLarge_B; break;

            case "EnemyB_A":    targetPool = poolEnemyBig_A; break;
            case "EnemyB_B":    targetPool = poolEnemyBig_B; break;

            case "Boss_A":      targetPool = poolEnemyBoss_A; break;
            case "Boss_B":      targetPool = poolEnemyBoss_B; break;
            case "Boss_C":      targetPool = poolEnemyBoss_C; break;
            case "Boss_D":      targetPool = poolEnemyBoss_D; break;

            default:            targetPool = null; break;
        }

        if (targetPool != null)
        {
            for (int i = 0; i < targetPool.Length; i++)
            {
                if (!targetPool[i].activeSelf)
                {
                    targetPool[i].SetActive(isSetActive);
                    return targetPool[i];
                }
            }
        }
        Debug.Log($"\"{obj}\" 오브젝트의 여분이 없거나 찾을 수 없습니다.");
        return null;
    }
    
}
