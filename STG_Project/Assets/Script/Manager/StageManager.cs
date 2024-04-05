using System.Collections;
using System.Collections.Generic;
using System.IO;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SocialPlatforms.Impl;
using UnityEngine.UIElements;
using UnityEngine.SceneManagement;
using TMPro;
using static LobbyManager;

public class StageManager : MonoBehaviour
{
    public static StageManager instance;
    public PoolManager pool;    // 오브젝트 Pool
    public AudioManager audioManager;

    public GameObject InGameUI;

    public TextMeshProUGUI scoreText, bossFieldLimitText;
    public TextMeshProUGUI
        Result_Score, Result_BossTime, Result_Remaining, Result_TotalScore;
    public GameObject Back_Button;

    public static List<GameObject> EnemyList;

    public enum StageState { Lobby, Ready, Play, End }

    public static StageState stageState = StageState.Lobby;

    public List<SpawnLogic> spawnList;
    public int spawnIndex;
    public int spawnAmount;
    public bool spawnEnd;

    public float currentSpawnTime;
    public float nextSpawnDelay;
    
    public GameObject debugObj;
    public GameObject Ui_Stage;
    public GameObject background;

    public Material[] backgroundSprite;

    public GameObject gameResultPanel;
    public GameObject gameOverPanel;

    public GameObject playerRemain1, playerRemain2;

    public Renderer background_ren;
    float background_offset = 0;

    public GameObject player;
    public static Vector3 playerPos;
    public Vector3 playerMovingVec;
    public static int playerLevel = 1;
    public static int playerHealth = 2;

    public static int Score = 0;

    public int getStageNum = 0;
    public char getPlayerType;

    public float BossFieldLimitTime = 0;
    public static bool bossExist;

    float ShowResultTime = 0;
    int ShowResultCount = 0;
    int ResultScore, BossTimeScore, RemainingScore;

    private void Awake()
    {
        instance = this;
        /*
        if (instance != this && instance != null)
        {
            Destroy(gameObject); return;
        }
        else
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        */
    }

    private void OnEnable()
    {
        pool = PoolManager.instance;
        audioManager = AudioManager.instance;
        Debug.Log("게임 시작");
        StageInit();
    }

    public void StageInit()
    {
        InGameUI.SetActive(true);

        getPlayerType = 'A';

        getStageNum = LobbyManager.instance.setStageNum;
        stageState = StageState.Ready;

        EnemyList = new List<GameObject>();
        spawnList = new List<SpawnLogic>();

        playerLevel = 1;
        playerHealth = 2;
        Score = 0;

        currentSpawnTime = 0;
        nextSpawnDelay = 0;

        switch (getPlayerType)
        {
            case 'A':   player = pool.MakeObject("Player_A"); break;
            case 'B':   player = pool.MakeObject("Player_B"); break;
            case 'C':   player = pool.MakeObject("Player_C"); break;
        }

        player.transform.position = new Vector2(0, -3);
        playerPos = player.transform.position;
        player.GetComponent<Player>().pool = pool;
        player.GetComponent<Player>().audioManager = audioManager;
        player.GetComponent<Player>().stageManager = instance;

        Ui_Stage.SetActive(true);
        gameResultPanel.SetActive(false);
        Result_Score.text = Result_BossTime.text =
            Result_Remaining.text = Result_TotalScore.text = "";
        Back_Button.SetActive(false);
        gameOverPanel.SetActive(false);

        background_ren = background.GetComponent<Renderer>();
        background_offset = 0.65f;

        /*
        DebugTest debugtest = debugObj.GetComponent<DebugTest>();
        debugtest.pool = pool;
        */

        BossFieldLimitTime = 60;
        bossExist = false;
        scoreText.text = "0";
        bossFieldLimitText.text = "";


        ReadSpawnFile();
        SpawnEnemy();

        Invoke("GameStart", 2f);
    }

    public void GameStart() => stageState = StageState.Play;

    void Update()
    {
        if (playerHealth == 1) playerRemain2.SetActive(false);
        if (playerHealth == 0) playerRemain1.SetActive(false);

        if (stageState == StageState.Ready || stageState == StageState.Play)
        {
            background_offset += 0.02f * Time.deltaTime;
            background_ren.material.mainTextureOffset = new Vector2(0, background_offset);
        }


        if (stageState == StageState.Play)
        {
            // 플레이어 이동 벡터 및 위치 저장
            playerMovingVec = player.transform.position - playerPos;
            playerPos = player.transform.position;

            currentSpawnTime += Time.deltaTime;
            if (!spawnEnd) SpawnEnemy();
        }

        if (bossExist)
        {
            BossFieldLimitTime -= Time.deltaTime;
            if (BossFieldLimitTime <= 0)
            {
                bossExist = false;
                bossFieldLimitText.text =
                $"0<size=80>.00</size>";
            }
            else
            {
                bossFieldLimitText.text =
                    $"{Mathf.FloorToInt(BossFieldLimitTime)}<size=80>.{BossFieldLimitTime * 100 % 100:#,#00}</size>";
            }
        }

        scoreText.text = Score.ToString();

        if (stageState == StageState.End && ShowResultCount < 5)
        {
            ShowResultTime += Time.deltaTime;
            ShowResult();
        }
    }

    void ReadSpawnFile()
    {
        spawnList.Clear();
        spawnIndex = 0;
        spawnEnd = false;

        TextAsset textFile;

        switch (getStageNum)
        {
            case 1: textFile = Resources.Load("Spawn_Stage1") as TextAsset; break;
            case 2: textFile = Resources.Load("Spawn_Stage2") as TextAsset; break;
            default:
                Debug.Log("스테이지 정보를 불러올 수 없습니다.");
                textFile = null; break;
        }

        StringReader stringReader = new StringReader(textFile.text);

        while (stringReader != null)
        {
            string line = stringReader.ReadLine();
            //Debug.Log(line);

            if (line == null) break;

            SpawnLogic spawnData = new SpawnLogic();
            string[] dataSpilt = line.Split(',');

            spawnData.spawnCode = dataSpilt[0];

            spawnData.delay = float.Parse(dataSpilt[1]);
            spawnData.enemyType = dataSpilt[2];
            spawnData.posX = float.Parse(dataSpilt[3]);
            spawnData.posY = float.Parse(dataSpilt[4]);
            spawnData.dropItemName = dataSpilt[5];

            spawnData.movX = float.Parse(dataSpilt[6]);
            spawnData.movY = float.Parse(dataSpilt[7]);
            spawnData.degreeZ = float.Parse(dataSpilt[8]);
            spawnData.movSpeed = float.Parse(dataSpilt[9]);
            spawnData.movingType = dataSpilt[10];
            spawnData.movDesX = float.Parse(dataSpilt[11]);
            spawnData.movDesY = float.Parse(dataSpilt[12]);
            spawnData.movExitX = float.Parse(dataSpilt[13]);
            spawnData.movExitY = float.Parse(dataSpilt[14]);
            spawnData.fieldTimeLimit = float.Parse(dataSpilt[15]);

            spawnData.bulletType = dataSpilt[16];
            spawnData.bulletName = dataSpilt[17];
            spawnData.patternType = dataSpilt[18];
            spawnData.bulletSpeed = float.Parse(dataSpilt[19]);
            spawnData.shootLimit = int.Parse(dataSpilt[20]);
            spawnData.firstWaitTime = float.Parse(dataSpilt[21]);
            spawnData.waitTime = float.Parse(dataSpilt[22]);

            spawnList.Add(spawnData);
        }
        stringReader.Close();
        spawnAmount = spawnList.Count;
        nextSpawnDelay = spawnList[0].delay;
    }

    public void BossInit(GameObject boss)
    {
        Enemy bossLogic = boss.GetComponent<Enemy>();
        bossLogic.bossLogics.Clear();
        bossLogic.bossLogicsFinal.Clear();

        TextAsset textFile;

        switch (getStageNum)
        {
            case 1: textFile = Resources.Load("BossLogic_A") as TextAsset; break;
            case 2: textFile = Resources.Load("BossLogic_B") as TextAsset; break;
            default:
                Debug.Log("스테이지 정보를 불러올 수 없습니다.");
                textFile = null; break;
        }

        StringReader stringReader = new StringReader(textFile.text);

        while (stringReader != null)
        {
            string line = stringReader.ReadLine();
            if (line == null) break;

            BossLogic bossLogicData = new BossLogic();
            string[] dataSpilt = line.Split(',');

            bossLogicData.BossLogicCode = dataSpilt[0];

            bossLogicData.delay = float.Parse(dataSpilt[1]);
            bossLogicData.posX = float.Parse(dataSpilt[2]);
            bossLogicData.posY = float.Parse(dataSpilt[3]);

            bossLogicData.movX = float.Parse(dataSpilt[4]);
            bossLogicData.movY = float.Parse(dataSpilt[5]);
            bossLogicData.degreeZ = float.Parse(dataSpilt[6]);
            bossLogicData.movSpeed = float.Parse(dataSpilt[7]);
            bossLogicData.movingType = dataSpilt[8];
            bossLogicData.movDesX = float.Parse(dataSpilt[9]);
            bossLogicData.movDesY = float.Parse(dataSpilt[10]);
            bossLogicData.movExitX = float.Parse(dataSpilt[11]);
            bossLogicData.movExitY = float.Parse(dataSpilt[12]);

            bossLogicData.shootPos1 = dataSpilt[13];
            bossLogicData.shootPos2 = dataSpilt[14];
            bossLogicData.shootPos3 = dataSpilt[15];
            bossLogicData.shootPos4 = dataSpilt[16];
            bossLogicData.shootPos5 = dataSpilt[17];

            bossLogicData.bulletType = dataSpilt[18];
            bossLogicData.bulletName = dataSpilt[19];
            bossLogicData.patternType = dataSpilt[20];
            bossLogicData.bulletSpeed = float.Parse(dataSpilt[21]);
            bossLogicData.shootLimit = int.Parse(dataSpilt[22]);
            bossLogicData.firstWaitTime = float.Parse(dataSpilt[23]);
            bossLogicData.waitTime = float.Parse(dataSpilt[24]);

            if (dataSpilt[0] == "Final")
            {
                bossLogic.bossLogicsFinal.Enqueue(bossLogicData);
                break;
            }
            bossLogic.bossLogics.Enqueue(bossLogicData);
        }
        stringReader.Close();
        bossLogic.firstWaitTime = 1.5f;
    }

    void SpawnEnemy()
    {
        if (currentSpawnTime >= nextSpawnDelay)
        {
            Debug.Log($"편대 {spawnList[spawnIndex].spawnCode}번");

            GameObject enemy = pool.MakeObject(spawnList[spawnIndex].enemyType);
            enemy.GetComponent<Enemy>().audioManager = audioManager;

            enemy.transform.position = new Vector2(
                spawnList[spawnIndex].posX, spawnList[spawnIndex].posY);

            Enemy enemyLogic = enemy.GetComponent<Enemy>();
            enemyLogic.pool = pool;
            enemyLogic.getDropItemName = spawnList[spawnIndex].dropItemName;

            enemyLogic.moveVec = new Vector2(
                spawnList[spawnIndex].movX, spawnList[spawnIndex].movY).normalized;
            enemyLogic.degreeZ = spawnList[spawnIndex].degreeZ;
            enemyLogic.movSpeed = spawnList[spawnIndex].movSpeed;
            enemyLogic.getMovingType = spawnList[spawnIndex].movingType;
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

            spawnIndex++;
            if (spawnAmount == spawnIndex) spawnEnd = true;
            else nextSpawnDelay = spawnList[spawnIndex].delay;
            currentSpawnTime = 0;

            EnemyList.Add(enemy);

            if (enemy.GetComponent<Enemy>().enemyType == Enemy.EnemyType.Boss)
            {
                BossInit(enemy);
                //debugObj.GetComponent<DebugTest>().bossExist = true;
                bossExist = true;
            }
            enemyLogic.Init();
        }
    }

    public void GameClear()
    {
        spawnEnd = true;
        //debugObj.GetComponent<DebugTest>().bossExist = false;
        bossExist = true;
        StartCoroutine("GameResult");
    }

    public void GameDefeat()
    {
        StartCoroutine("GameOver");
    }

    public IEnumerator GameResult()
    {
        yield return new WaitForSeconds(5f);
        InGameUI.SetActive(false);

        BossTimeScore = (int)(BossFieldLimitTime * 100) * 10;
        RemainingScore = playerHealth * 40000;
        ResultScore += Score + BossTimeScore + RemainingScore;

        stageState = StageState.End;
        gameResultPanel.SetActive(true);
    }

    public void ShowResult()
    {
        if (ShowResultTime >= 0.5f)
        {
            switch (ShowResultCount)
            {
                case 0:
                    Result_Score.text =
                $"<size=64>Score</size> {Score.ToString()}"; break;
                case 1:
                    Result_BossTime.text =
                $"<size=64>Boss-time</size> {BossTimeScore.ToString()}"; break;
                case 2:
                    Result_Remaining.text =
                $"<size=64>Remaining</size> {RemainingScore.ToString()}"; break;
                case 3:
                    Result_TotalScore.text =
                $"<size=80>Total Score</size>\n{ResultScore.ToString()}"; break;
                case 4: Back_Button.SetActive(true); break;
            }
            ShowResultTime = 0; ShowResultCount++;
        }
    }

    public IEnumerator GameOver()
    {
        yield return new WaitForSeconds(1.5f);
        InGameUI.SetActive(false);

        stageState = StageState.End;
        Time.timeScale = 0f;
        gameOverPanel.SetActive(true);
    }

    public void GameEnd()
    {
        LobbyManager.menuSelected = MenuSelected.Main;
        gameResultPanel.SetActive(false);
        gameOverPanel.SetActive(false);
        player.SetActive(false);
        Time.timeScale = 1f;

        LobbyManager.instance.EndStage();
    }

}
