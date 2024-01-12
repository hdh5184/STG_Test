using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    public static List<float> EnemyCode = new List<float>();

    public PoolManager pool;

    public GameObject player;

    public Vector3 playerPos;
    public Vector3 playerMovingVec;

    public static int playerLevel = 1;

    public static int Score = 0;

    private void Awake()
    {
        instance = this;
        playerPos = player.transform.position;
    }

    void Start()
    {
        Score = 0;
    }

    void Update()
    {
        playerMovingVec = player.transform.position - playerPos;
        playerPos = player.transform.position;
    }
}
