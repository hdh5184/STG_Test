using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    // 적기 코드 저장
    // 해당 적기가 발사한 탄 소거 및 점수 아이템 전환을 위한 연계용 코드 
    public static List<float> EnemyCode = new List<float>();

    public PoolManager pool;    // 오브젝트 Pool

    public GameObject debugObj;

    // 플레이어 위치 및 이동 벡터
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
        DebugTest debugtest = debugObj.GetComponent<DebugTest>();
        debugtest.pool = pool;

        SpawnEnemy();

    }

    void Update()
    {
        // 플레이어 이동 벡터 및 위치 저장
        playerMovingVec = player.transform.position - playerPos;
        playerPos = player.transform.position;
    }

    void SpawnEnemy()
    {
        GameObject enemy1 = pool.MakeObject("Enemy_Zako");
        Enemy enemyLogic = enemy1.GetComponent<Enemy>();
        enemyLogic.pool = pool;
        enemyLogic.playerPos = playerPos;

        enemy1.transform.position = new Vector2(2, 3);
    }
}
