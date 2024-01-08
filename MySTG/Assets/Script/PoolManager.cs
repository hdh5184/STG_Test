using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PoolManager : MonoBehaviour
{
    public static PoolManager instance;

    public GameObject inputPBullet_Lv1;
    public GameObject inputPBullet_Lv2;
    public GameObject inputPBullet_Lv3;

    public GameObject inputEBullet_SG1;
    public GameObject inputEBullet_SB1;
    public GameObject inputEBullet_SP1;
    public GameObject inputEBullet_BG1;

    public GameObject inputEffect_EDestroy;

    public GameObject[] poolPBullet_Lv1;
    public GameObject[] poolPBullet_Lv2;
    public GameObject[] poolPBullet_Lv3;

    public GameObject[] poolEBullet_SG1;
    public GameObject[] poolEBullet_SB1;
    public GameObject[] poolEBullet_SP1;
    public GameObject[] poolEBullet_BG1;

    public GameObject[] poolEffect_EDestroy;

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

        poolEffect_EDestroy = new GameObject[20];
        for (int i = 0; i < poolEffect_EDestroy.Length; i++)
        {
            poolEffect_EDestroy[i] = Instantiate(inputEffect_EDestroy);
            poolEffect_EDestroy[i].SetActive(false);
        }
    }

    void Start()
    {
        
    }

    void Update()
    {
        
    }
}