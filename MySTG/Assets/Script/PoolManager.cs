using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PoolManager : MonoBehaviour
{
    public static PoolManager instance;

    public GameObject inputPBullet;
    public GameObject inputEBullet_SG1;
    public GameObject inputEBullet_SB1;
    public GameObject inputEBullet_SP1;
    public GameObject inputEBullet_BG1;

    public GameObject[] poolPBullet;
    public GameObject[] poolEBullet_SG1;
    public GameObject[] poolEBullet_SB1;
    public GameObject[] poolEBullet_SP1;
    public GameObject[] poolEBullet_BG1;

    private void Awake()
    {
        if (instance == null) instance = this;

        poolPBullet = new GameObject[50];
        for (int i = 0; i < poolPBullet.Length; i++)
        {
            poolPBullet[i] = Instantiate(inputPBullet);
            poolPBullet[i].SetActive(false);
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
    }

    void Start()
    {
        
    }

    void Update()
    {
        
    }
}
