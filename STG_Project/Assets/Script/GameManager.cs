using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;


    // 적기 코드 저장
    // 해당 적기가 발사한 탄 소거 및 점수 아이템 전환을 위한 연계용 코드 
    public static List<float> EnemyCode = new List<float>();
    public static List<GameObject> EnemyList = new List<GameObject>();

    public List<Spawn> spawnList;
    public int spawnIndex;
    public int spawnAmount;
    public bool spawnEnd;
    public float currentSpawnTime;
    public float nextSpawnDelay;

    

    public PoolManager pool;    // 오브젝트 Pool

    public GameObject debugObj;

    // 플레이어 위치 및 이동 벡터
    public GameObject player;
    public static Vector3 playerPos;
    public Vector3 playerMovingVec;
    public static int playerLevel = 1;

    public static int Score = 0;


    private void Awake()
    {
        instance = this;
        playerPos = player.transform.position;
        spawnList = new List<Spawn>();
        currentSpawnTime = 0;
        nextSpawnDelay = 0;
        
    }

    void ReadSpawnFile()
    {
        spawnList.Clear();
        spawnIndex = 0;
        spawnEnd = false;

        TextAsset textFile = Resources.Load("Spawn_Stage1") as TextAsset;
        StringReader stringReader = new StringReader(textFile.text);

        while (stringReader != null)
        {
            string line = stringReader.ReadLine();
            //Debug.Log(line);

            if (line == null) break;

            Spawn spawnData = new Spawn();
            string[] dataSpilt = line.Split(',');

            spawnData.delay = float.Parse(dataSpilt[0]);
            spawnData.enemyType = dataSpilt[1];
            spawnData.posX = float.Parse(dataSpilt[2]);
            spawnData.posY = float.Parse(dataSpilt[3]);
            spawnData.movX = float.Parse(dataSpilt[4]);
            spawnData.movY = float.Parse(dataSpilt[5]);
            spawnData.speed = float.Parse(dataSpilt[6]);
            spawnData.patternType = dataSpilt[7];
            spawnData.bulletType = dataSpilt[8];
            spawnData.bulletName = dataSpilt[9];
            spawnData.shootLevel = int.Parse(dataSpilt[10]);
            spawnData.firstWaitTime = float.Parse(dataSpilt[11]);
            spawnData.waitTime = float.Parse(dataSpilt[12]);
            spawnData.spawnCode = dataSpilt[12];
            spawnList.Add(spawnData);
            //Debug.Log(spawnData);
        }
        stringReader.Close();
        spawnAmount = spawnList.Count;
        nextSpawnDelay = spawnList[0].delay;
    }

    void Start()
    {
        Score = 0;
        DebugTest debugtest = debugObj.GetComponent<DebugTest>();
        debugtest.pool = pool;
        player.GetComponent<Player>().pool = pool;

        ReadSpawnFile();
        SpawnEnemy();
    }

    void Update()
    {
        // 플레이어 이동 벡터 및 위치 저장
        playerMovingVec = player.transform.position - playerPos;
        playerPos = player.transform.position;

        currentSpawnTime += Time.deltaTime;
        if (!spawnEnd) SpawnEnemy();

        //Debug.Log($"적 기체 수 : {EnemyList.Count}");
    }

    void SpawnEnemy()
    {
        if (currentSpawnTime >= nextSpawnDelay)
        {
            GameObject enemy = pool.MakeObject(spawnList[spawnIndex].enemyType);
            enemy.transform.position = new Vector2(
                spawnList[spawnIndex].posX, spawnList[spawnIndex].posY);

            Enemy enemyLogic = enemy.GetComponent<Enemy>();
            enemyLogic.pool = pool;
            enemyLogic.moveVec = new Vector2(
                spawnList[spawnIndex].movX, spawnList[spawnIndex].movY).normalized;
            enemyLogic.speed = spawnList[spawnIndex].speed;
            enemyLogic.patternType = spawnList[spawnIndex].patternType;
            enemyLogic.setBulletType = spawnList[spawnIndex].bulletType;
            enemyLogic.bulletName = spawnList[spawnIndex].bulletName;
            enemyLogic.shootLimit = spawnList[spawnIndex].shootLevel;
            enemyLogic.firstWaitTime = spawnList[spawnIndex].firstWaitTime;
            enemyLogic.waitTime = spawnList[spawnIndex].waitTime;

            spawnIndex++;
            if (spawnAmount == spawnIndex) spawnEnd = true;
            else nextSpawnDelay = spawnList[spawnIndex].delay;
            currentSpawnTime = 0;

            EnemyList.Add(enemy);
            enemyLogic.Init();
        }
        

        /*
         * public string patternType;
        public string bulletType;
        public string bulletName;
        public int shootLevel;
        public float waitTime;
        */


        //GameObject enemy1 = pool.MakeObject("EnemyS_A");

        /*
        GameObject enemy2 = pool.MakeObject("EnemyB_A");
        EnemyList.Add(enemy2);
        Enemy enemyLogic1 = enemy2.GetComponent<Enemy>();
        enemyLogic1.pool = pool;
        enemyLogic1.playerPos = playerPos;
        enemyLogic1.setBulletType = "str";

        enemy2.transform.position = new Vector2(0, 3);
        */
        
        /*
        enemy2 = pool.MakeObject("EnemyM_A");
        enemy2.transform.position = new Vector2(2, 1);
        EnemyList.Add(enemy2);
        enemyLogic = enemy2.GetComponent<Enemy>();
        enemyLogic.pool = pool;
        enemyLogic.playerPos = playerPos;
        enemy2 = pool.MakeObject("EnemyM_A");
        enemy2.transform.position = new Vector2(2, 2);
        EnemyList.Add(enemy2);
        enemyLogic = enemy2.GetComponent<Enemy>();
        enemyLogic.pool = pool;
        enemyLogic.playerPos = playerPos;
        enemy2 = pool.MakeObject("EnemyM_A");
        enemy2.transform.position = new Vector2(2, 3);
        EnemyList.Add(enemy2);
        enemyLogic = enemy2.GetComponent<Enemy>();
        enemyLogic.pool = pool;
        enemyLogic.playerPos = playerPos;
        */
    }


}
