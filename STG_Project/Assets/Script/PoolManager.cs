using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PoolManager : MonoBehaviour
{
    public static PoolManager instance;

    public GameObject inputPBullet_Lv1;
    public GameObject inputPBullet_Lv2;
    public GameObject inputPBullet_Lv3;
    public GameObject inputPBullet_Lv4;

    public GameObject inputEBullet_SG1;
    public GameObject inputEBullet_SB1;
    public GameObject inputEBullet_SP1;
    public GameObject inputEBullet_BG1;

    public GameObject inputEBullet_SG1_Homing;

    public GameObject inputEffect_EDestroy;

    public GameObject inputItem_PowerUp;
    public GameObject inputItem_SilverCoin;
    public GameObject inputItem_GoldCoin;

    public GameObject inputEnemy_Zako;
    public GameObject inputEnemy_Small;



    public GameObject[] poolPBullet_Lv1;
    public GameObject[] poolPBullet_Lv2;
    public GameObject[] poolPBullet_Lv3;
    public GameObject[] poolPBullet_Lv4;

    public GameObject[] poolEBullet_SG1;
    public GameObject[] poolEBullet_SB1;
    public GameObject[] poolEBullet_SP1;
    public GameObject[] poolEBullet_BG1;

    public GameObject[] poolEBullet_SG1_Homing;

    public GameObject[] poolEffect_EDestroy;

    public GameObject[] poolItem_PowerUp;
    public GameObject[] poolItem_SilverCoin;
    public GameObject[] poolItem_GoldCoin;

    public GameObject[] poolEnemy_Zako;
    public GameObject[] poolEnemy_Small;

    public GameObject[] targetPool;

    private void Awake()
    {
        if (instance == null) instance = this;

        poolPBullet_Lv1 = new GameObject[20];
        for (int i = 0; i < poolPBullet_Lv1.Length; i++)
        {
            poolPBullet_Lv1[i] = Instantiate(inputPBullet_Lv1);
            poolPBullet_Lv1[i].SetActive(false);
        }

        poolPBullet_Lv2 = new GameObject[20];
        for (int i = 0; i < poolPBullet_Lv2.Length; i++)
        {
            poolPBullet_Lv2[i] = Instantiate(inputPBullet_Lv2);
            poolPBullet_Lv2[i].SetActive(false);
        }

        poolPBullet_Lv3 = new GameObject[20];
        for (int i = 0; i < poolPBullet_Lv3.Length; i++)
        {
            poolPBullet_Lv3[i] = Instantiate(inputPBullet_Lv3);
            poolPBullet_Lv3[i].SetActive(false);
        }

        poolPBullet_Lv4 = new GameObject[80];
        for (int i = 0; i < poolPBullet_Lv4.Length; i++)
        {
            poolPBullet_Lv4[i] = Instantiate(inputPBullet_Lv4);
            poolPBullet_Lv4[i].SetActive(false);
        }


        poolEBullet_SG1 = new GameObject[500];
        for (int i = 0; i < poolEBullet_SG1.Length; i++)
        {
            poolEBullet_SG1[i] = Instantiate(inputEBullet_SG1);
            poolEBullet_SG1[i].SetActive(false);
        }

        poolEBullet_SB1 = new GameObject[300];
        for (int i = 0; i < poolEBullet_SB1.Length; i++)
        {
            poolEBullet_SB1[i] = Instantiate(inputEBullet_SB1);
            poolEBullet_SB1[i].SetActive(false);
        }

        poolEBullet_SP1 = new GameObject[500];
        for (int i = 0; i < poolEBullet_SP1.Length; i++)
        {
            poolEBullet_SP1[i] = Instantiate(inputEBullet_SP1);
            poolEBullet_SP1[i].SetActive(false);
        }

        poolEBullet_BG1 = new GameObject[50];
        for (int i = 0; i < poolEBullet_BG1.Length; i++)
        {
            poolEBullet_BG1[i] = Instantiate(inputEBullet_BG1);
            poolEBullet_BG1[i].SetActive(false);
        }

        poolEBullet_SG1_Homing = new GameObject[500];
        for (int i = 0; i < poolEBullet_SG1_Homing.Length; i++)
        {
            poolEBullet_SG1_Homing[i] = Instantiate(inputEBullet_SG1_Homing);
            poolEBullet_SG1_Homing[i].SetActive(false);
        }



        poolEffect_EDestroy = new GameObject[20];
        for (int i = 0; i < poolEffect_EDestroy.Length; i++)
        {
            poolEffect_EDestroy[i] = Instantiate(inputEffect_EDestroy);
            poolEffect_EDestroy[i].SetActive(false);
        }

        poolItem_PowerUp = new GameObject[5];
        for (int i = 0; i < poolItem_PowerUp.Length; i++)
        {
            poolItem_PowerUp[i] = Instantiate(inputItem_PowerUp);
            poolItem_PowerUp[i].SetActive(false);
        }

        poolItem_SilverCoin = new GameObject[500];
        for (int i = 0; i < poolItem_SilverCoin.Length; i++)
        {
            poolItem_SilverCoin[i] = Instantiate(inputItem_SilverCoin);
            poolItem_SilverCoin[i].SetActive(false);
        }

        poolItem_GoldCoin = new GameObject[500];
        for (int i = 0; i < poolItem_GoldCoin.Length; i++)
        {
            poolItem_GoldCoin[i] = Instantiate(inputItem_GoldCoin);
            poolItem_GoldCoin[i].SetActive(false);
        }

        poolEnemy_Zako = new GameObject[20];
        for (int i = 0; i < poolEnemy_Zako.Length; i++)
        {
            poolEnemy_Zako[i] = Instantiate(inputEnemy_Zako);
            poolEnemy_Zako[i].SetActive(false);
        }

        poolEnemy_Small = new GameObject[10];
        for (int i = 0; i < poolEnemy_Small.Length; i++)
        {
            poolEnemy_Small[i] = Instantiate(inputEnemy_Small);
            poolEnemy_Small[i].SetActive(false);
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

            case "Bullet_SG1": targetPool = poolEBullet_SG1; break;
            case "Bullet_SB1": targetPool = poolEBullet_SB1; break;
            case "Bullet_SP1": targetPool = poolEBullet_SP1; break;
            case "Bullet_BG1": targetPool = poolEBullet_BG1; break;

            case "Bullet_SG1_Homing": targetPool = poolEBullet_SG1_Homing; break;

            case "PowerUp": targetPool = poolItem_PowerUp; break;
            case "SilverCoin": targetPool = poolItem_SilverCoin; break;
            case "GoldCoin": targetPool = poolItem_GoldCoin; break;

            case "EDestroy": targetPool = poolEffect_EDestroy; break;

            case "Enemy_Zako": targetPool = poolEnemy_Zako; break;
            case "Enemy_Small": targetPool = poolEnemy_Small; break;
        }

        for (int i = 0; i < targetPool.Length; i++)
        {
            if (!targetPool[i].activeSelf)
            {
                targetPool[i].SetActive(true);
                return targetPool[i];
            }
        }

        return null;
    }
}
