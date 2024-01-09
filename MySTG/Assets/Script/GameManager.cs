using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    public PoolManager pool;

    public GameObject player;

    public Vector3 playerPos;
    public Vector3 playerMovingVec;

    public static int playerLevel = 1;

    void Start()
    {
        instance = this;
        playerPos = player.transform.position;
    }

    void Update()
    {
        playerMovingVec = player.transform.position - playerPos;
        playerPos = player.transform.position;
        Debug.Log(playerLevel);
    }
}
