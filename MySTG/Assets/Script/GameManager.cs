using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    public GameObject Player;

    public Vector2 playerPos;

    void Start()
    {
        instance = this;
    }

    void Update()
    {
        playerPos = new Vector2(Player.transform.position.x, Player.transform.position.y);
    }
}
