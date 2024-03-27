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
    public GameObject background;
    public Renderer background_ren;
    float background_offset = 0;

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

            spawnData.spawnCode = dataSpilt[0];

            spawnData.delay =           float.Parse(dataSpilt[1]);
            spawnData.enemyType =       dataSpilt[2];
            spawnData.posX =            float.Parse(dataSpilt[3]);
            spawnData.posY =            float.Parse(dataSpilt[4]);
            spawnData.dropItemName =    dataSpilt[5];

            spawnData.movX =            float.Parse(dataSpilt[6]);
            spawnData.movY =            float.Parse(dataSpilt[7]);
            spawnData.degreeZ =         float.Parse(dataSpilt[8]);
            spawnData.movSpeed =        float.Parse(dataSpilt[9]);
            spawnData.movingType =      dataSpilt[10];
            spawnData.movDesX =         float.Parse(dataSpilt[11]);
            spawnData.movDesY =         float.Parse(dataSpilt[12]);
            spawnData.movExitX =        float.Parse(dataSpilt[13]);
            spawnData.movExitY =        float.Parse(dataSpilt[14]);
            spawnData.fieldTimeLimit =  float.Parse(dataSpilt[15]);
            
            spawnData.bulletType =      dataSpilt[16];
            spawnData.bulletName =      dataSpilt[17];
            spawnData.patternType =     dataSpilt[18];
            spawnData.bulletSpeed =     float.Parse(dataSpilt[19]);
            spawnData.shootLimit =      int.Parse(dataSpilt[20]);
            spawnData.firstWaitTime =   float.Parse(dataSpilt[21]);
            spawnData.waitTime =        float.Parse(dataSpilt[22]);

            spawnList.Add(spawnData);
            Debug.Log(spawnData.spawnCode);
        }
        stringReader.Close();
        spawnAmount = spawnList.Count;
        nextSpawnDelay = spawnList[0].delay;
    }

    void Start()
    {
        background_ren = background.GetComponent<Renderer>();
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

        background_offset += 0.02f * Time.deltaTime;
        background_ren.material.mainTextureOffset = new Vector2(0, background_offset);

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
            enemyLogic.getDropItemName = spawnList[spawnIndex].dropItemName;

            enemyLogic.moveVec = new Vector2(
                spawnList[spawnIndex].movX, spawnList[spawnIndex].movY).normalized;
            enemyLogic.degreeZ = spawnList[spawnIndex].degreeZ;
            enemyLogic.movSpeed = spawnList[spawnIndex].movSpeed;
            enemyLogic.getmovingType = spawnList[spawnIndex].movingType;
            enemyLogic.moveDesVec = new Vector2(
                spawnList[spawnIndex].movDesX, spawnList[spawnIndex].movDesY);
            enemyLogic.moveExitVec = new Vector2(
                spawnList[spawnIndex].movExitX, spawnList[spawnIndex].movExitY).normalized;
            enemyLogic.fieldTimeLimit = spawnList[spawnIndex].fieldTimeLimit;

            enemyLogic.getBulletType = spawnList[spawnIndex].bulletType;
            enemyLogic.getBulletName = spawnList[spawnIndex].bulletName;
            enemyLogic.getPatternType = spawnList[spawnIndex].patternType;
            enemyLogic.bulletSpeed = spawnList[spawnIndex].bulletSpeed;
            enemyLogic.shootLimit = spawnList[spawnIndex].shootLimit;
            enemyLogic.firstWaitTime = spawnList[spawnIndex].firstWaitTime;
            enemyLogic.waitTime = spawnList[spawnIndex].waitTime;


            Debug.Log(enemyLogic.moveVec);
            Debug.Log(new Vector2(
                spawnList[spawnIndex].movX, spawnList[spawnIndex].movY).normalized);
            spawnIndex++;
            if (spawnAmount == spawnIndex) spawnEnd = true;
            else nextSpawnDelay = spawnList[spawnIndex].delay;
            currentSpawnTime = 0;

            EnemyList.Add(enemy);
            enemyLogic.Init();
        }
    }
}
