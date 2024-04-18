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

    public enum StageState { Lobby, Ready, Play, End, Pause }

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

    public Sprite[] backgroundSprite;

    public GameObject gameResultPanel;
    public GameObject gameOverPanel;
    public GameObject gamePausePanel;
    public GameObject gameQuitPanel;

    public GameObject playerRemain1, playerRemain2;

    public SpriteRenderer background_sr;
    float background_offset = 0;

    public GameObject player;
    public static Vector3 playerPos;
    public Vector3 playerMovingVec;
    public static int playerLevel = 1;
    public static int playerHealth = 2;

    public static int Score = 0;

    public int getStageNum = 0;
    public int getPlayerType;

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


        getPlayerType = GameManager.instance.setPlayerUnit;

        getStageNum = LobbyManager.instance.setStageNum;
        stageState = StageState.Ready;

        EnemyList = new List<GameObject>();
        spawnList = new List<SpawnLogic>();

        playerLevel = 1;
        playerHealth = 2;
        Score = 0;

        currentSpawnTime = 0;
        nextSpawnDelay = 0;

        if (player != null) player.SetActive(false);

        switch (getPlayerType)
        {
            case 0:   player = pool.MakeObject("Player_A"); break;
            case 1:   player = pool.MakeObject("Player_B"); break;
            case 2:   player = pool.MakeObject("Player_C"); break;
        }

        player.GetComponent<Player>().pool = pool;
        player.GetComponent<Player>().audioManager = audioManager;
        player.GetComponent<Player>().stageManager = instance;
        player.GetComponent<Player>().PlayerInit();
        player.transform.position = new Vector2(0, -3);
        playerPos = player.transform.position;

        Ui_Stage.SetActive(true);
        gameResultPanel.SetActive(false);
        Result_Score.text = Result_BossTime.text =
            Result_Remaining.text = Result_TotalScore.text = "";
        Back_Button.SetActive(false);
        gamePausePanel.SetActive(false);
        gameOverPanel.SetActive(false);

        background_sr = background.GetComponent<SpriteRenderer>();
        background_sr.sprite = backgroundSprite[getStageNum - 1];

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
        if (stageState == StageState.Pause) return;

        if (playerHealth == 1) playerRemain2.SetActive(false);
        if (playerHealth == 0) playerRemain1.SetActive(false);

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
                BossFieldLimitTime = 0;
                bossExist = false;
                bossFieldLimitText.text =
                $"0<size=96>.00</size>";
            }
            else
            {
                bossFieldLimitText.text =
                    $"{Mathf.FloorToInt(BossFieldLimitTime):D2}<size=96>.{(int)(BossFieldLimitTime * 100 % 100):D2}</size>";
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

        string textFile;

        switch (getStageNum)
        {
            case 1: textFile = "Spawn_Stage1"; break;
            case 2: textFile = "Spawn_Stage2"; break;
            default:
                Debug.Log("스테이지 정보를 불러올 수 없습니다.");
                textFile = null; break;
        }

        List<Dictionary<string, object>> data_Dialog = CSVReader.Read(textFile);

        for (int i = 0; i < data_Dialog.Count; i++)
        {
            SpawnLogic spawnData = new SpawnLogic();

            spawnData.spawnCode =       data_Dialog[i]["spawnCode"].ToString();

            spawnData.delay =           float.Parse(data_Dialog[i]["delay"].ToString());
            spawnData.enemyType =       data_Dialog[i]["enemyType"].ToString();
            spawnData.posX =            float.Parse(data_Dialog[i]["posX"].ToString());
            spawnData.posY =            float.Parse(data_Dialog[i]["posY"].ToString());
            spawnData.dropItemName =    data_Dialog[i]["dropItemName"].ToString();

            spawnData.movX =            float.Parse(data_Dialog[i]["movX"].ToString());
            spawnData.movY =            float.Parse(data_Dialog[i]["movY"].ToString());
            spawnData.degreeZ =         float.Parse(data_Dialog[i]["degreeZ"].ToString());
            spawnData.movSpeed =        float.Parse(data_Dialog[i]["movSpeed"].ToString());
            spawnData.movingType =      data_Dialog[i]["movingType"].ToString();
            spawnData.movDesX =         float.Parse(data_Dialog[i]["movDesX"].ToString());
            spawnData.movDesY =         float.Parse(data_Dialog[i]["movDesY"].ToString());
            spawnData.movExitX =        float.Parse(data_Dialog[i]["movExitX"].ToString());
            spawnData.movExitY =        float.Parse(data_Dialog[i]["movExitY"].ToString());
            spawnData.fieldTimeLimit =  float.Parse(data_Dialog[i]["fieldTimeLimit"].ToString());

            spawnData.bulletType =      data_Dialog[i]["bulletType"].ToString();
            spawnData.bulletName =      data_Dialog[i]["bulletName"].ToString();
            spawnData.patternType =     data_Dialog[i]["patternType"].ToString();
            spawnData.bulletSpeed =     float.Parse(data_Dialog[i]["bulletSpeed"].ToString());
            spawnData.shootLimit =      int.Parse(data_Dialog[i]["shootLimit"].ToString());
            spawnData.firstWaitTime =   float.Parse(data_Dialog[i]["firstWaitTime"].ToString());
            spawnData.waitTime =        float.Parse(data_Dialog[i]["waitTime"].ToString());

            spawnList.Add(spawnData);
        }
        spawnAmount = spawnList.Count;
        nextSpawnDelay = spawnList[0].delay;
    }

    public void BossInit(GameObject boss)
    {
        Enemy bossLogic = boss.GetComponent<Enemy>();
        bossLogic.bossLogics.Clear();
        bossLogic.bossLogicsFinal.Clear();

        string textFile;

        switch (getStageNum)
        {
            case 1: textFile = "BossLogic_A"; break;
            case 2: textFile = "BossLogic_B"; break;
            default:
                Debug.Log("스테이지 정보를 불러올 수 없습니다.");
                textFile = null; break;
        }

        List<Dictionary<string, object>> data_Dialog = CSVReader.Read(textFile);

        for (int i = 0; i < data_Dialog.Count; i++)
        {
            BossLogic bossLogicData = new BossLogic();

            bossLogicData.BossLogicCode = data_Dialog[i]["BossLogicCode"].ToString();

            bossLogicData.delay =       float.Parse(data_Dialog[i]["delay"].ToString());
            bossLogicData.posX =        float.Parse(data_Dialog[i]["posX"].ToString());
            bossLogicData.posY =        float.Parse(data_Dialog[i]["posY"].ToString());

            bossLogicData.movX =        float.Parse(data_Dialog[i]["movX"].ToString());
            bossLogicData.movY =        float.Parse(data_Dialog[i]["movY"].ToString());
            bossLogicData.degreeZ =     float.Parse(data_Dialog[i]["degreeZ"].ToString());
            bossLogicData.movSpeed =    float.Parse(data_Dialog[i]["movSpeed"].ToString());
            bossLogicData.movingType =  data_Dialog[i]["movingType"].ToString();
            bossLogicData.movDesX =     float.Parse(data_Dialog[i]["movDesX"].ToString());
            bossLogicData.movDesY =     float.Parse(data_Dialog[i]["movDesY"].ToString());
            bossLogicData.movExitX =    float.Parse(data_Dialog[i]["movExitX"].ToString());
            bossLogicData.movExitY =    float.Parse(data_Dialog[i]["movExitY"].ToString());

            bossLogicData.shootPos1 =   data_Dialog[i]["pos1"].ToString();
            bossLogicData.shootPos2 =   data_Dialog[i]["pos2"].ToString();
            bossLogicData.shootPos3 =   data_Dialog[i]["pos3"].ToString();
            bossLogicData.shootPos4 =   data_Dialog[i]["pos4"].ToString();
            bossLogicData.shootPos5 =   data_Dialog[i]["pos5"].ToString();

            bossLogicData.bulletType =  data_Dialog[i]["bulletType"].ToString();
            bossLogicData.bulletName =  data_Dialog[i]["bulletName"].ToString();
            bossLogicData.patternType = data_Dialog[i]["patternType"].ToString();
            bossLogicData.bulletSpeed = float.Parse(data_Dialog[i]["bulletSpeed"].ToString());
            bossLogicData.shootLimit =  int.Parse(data_Dialog[i]["shootLimit"].ToString());
            bossLogicData.firstWaitTime = float.Parse(data_Dialog[i]["firstWaitTime"].ToString());
            bossLogicData.waitTime =    float.Parse(data_Dialog[i]["waitTime"].ToString());

            if (data_Dialog[i]["BossLogicCode"].ToString() == "Final")
            {
                bossLogic.bossLogicsFinal.Enqueue(bossLogicData); break;
            }
            bossLogic.bossLogics.Enqueue(bossLogicData);
        }

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
                bossExist = true;
            }
            enemyLogic.Init();
        }
    }

    public void GameClear()
    {
        spawnEnd = true;
        bossExist = false;
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
                case 0: Result_Score.text =
                        $"<size=56>Score</size> {Score.ToString()}"; break;
                case 1: Result_BossTime.text =
                        $"<size=56>Boss-time</size> {BossTimeScore.ToString()}"; break;
                case 2: Result_Remaining.text =
                        $"<size=56>Remaining</size> {RemainingScore.ToString()}"; break;
                case 3: Result_TotalScore.text =
                        $"<size=64>Total Score</size>\n{ResultScore.ToString()}"; break;
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

    public void GamePause()
    {
        if (stageState == StageState.Ready || stageState == StageState.Play)
        {
            Time.timeScale = 0f;
            stageState = StageState.Pause;
            gamePausePanel.SetActive(true);
        }
    }

    public void GameResume()
    {
        Time.timeScale = 1f;
        stageState = StageState.Play;
        gamePausePanel.SetActive(false);
    }

    public void GameQuitYN() { gamePausePanel.SetActive(false); gameQuitPanel.SetActive(true); }

    public void GameQuitN() { gamePausePanel.SetActive(true); gameQuitPanel.SetActive(false); }
    public void GameQuitY() { stageState = StageState.End; GameEnd(); }

}
