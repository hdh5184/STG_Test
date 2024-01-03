using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    public PoolManager pool;

    public GameObject player;

    public Vector2 playerPos;

    public static int playerLevel = 1;

    void Start()
    {
        instance = this;
    }

    void Update()
    {
        playerPos = new Vector2(player.transform.position.x, player.transform.position.y);
    }
}
