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
using UnityEngine.SocialPlatforms;

public class StageManager : MonoBehaviour
{
    public static StageManager instance;

    // 1. 매니저
    public PoolManager pool;
    public AudioManager audioManager;

    // 2. UI
    public Sprite[] backgroundSprite;
    public SpriteRenderer background_sr;
    public GameObject background;

    public TextMeshProUGUI scoreText, bossFieldLimitText, remainText;
    public TextMeshProUGUI[] ResultText_Clear, ResultText_Defeat;
    public GameObject InGameUI;
    public GameObject Ui_Stage;
    public GameObject[] playerRemain_UI;
    public GameObject Back_Button_Clear, Back_Button_Defeat;

    public GameObject gameResultPanel;
    public GameObject gameOverPanel;
    public GameObject gamePausePanel;
    public GameObject gameQuitPanel;

    public TextMeshProUGUI debug_StageTime;

    float ShowResultTime = 0;
    int ShowResultCountLimit = 0, ShowResultCount = 0;
    int ResultScore, BossTimeScore, RemainingScore;

    // 3. Stage 속성
    public static StageState stageState = StageState.Lobby;
    public int getStageNum = 0;
    public int getPlayerType;

    public static int Score = 0;
    float StageTime = 0;

    // 4. 플레이어
    public GameObject player;
    public static Vector3 playerPos;
    public Vector3 playerMovingVec;
    public static int playerLevel = 1;
    public static int playerHealth = 2;

    // 5. Enemy 관리
    public static List<GameObject> EnemyList;
    public List<SpawnLogic> spawnList;

    public int spawnIndex;
    public int spawnAmount;
    public float currentSpawnTime;
    public float nextSpawnDelay;
    public bool spawnEnd;

    public float BossFieldLimitTime = 0;
    public static bool bossExist;

    // e. 속성 모음
    public enum StageState { Lobby, Ready, Play, End, Pause }

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

    /// <summary> 스테이지 입장 </summary>
    private void OnEnable()
    {
        pool = PoolManager.instance;
        audioManager = AudioManager.instance;
        StageInit();
        Debug.Log("게임 시작");
    }

    /*************** 스테이지 매커니즘 ***************/

    /// <summary> 스테이지 시작 </summary>
    public void GameStart() => stageState = StageState.Play;

    /// <summary> 스테이지 초기화 </summary>
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
        BossTimeScore = 0;
        RemainingScore = 0;

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
        foreach (var item in ResultText_Clear) item.text = "";
        foreach (var item in ResultText_Defeat) item.text = "";
        foreach (var item in playerRemain_UI) item.SetActive(false);
        playerRemain_UI[GameManager.instance.setPlayerUnit].SetActive(true);
        Back_Button_Clear.SetActive(false);
        Back_Button_Defeat.SetActive(false);
        gamePausePanel.SetActive(false);
        gameOverPanel.SetActive(false);

        background_sr = background.GetComponent<SpriteRenderer>();
        background_sr.sprite = backgroundSprite[getStageNum - 1];

        BossFieldLimitTime = 60;
        bossExist = false;
        scoreText.text = "0";
        bossFieldLimitText.text = "";
        remainText.text = "2";

        ShowResultCountLimit = 0;
        ShowResultCount = 0;
        

        ReadSpawnFile();
        SpawnEnemy();

        Invoke("GameStart", 2f);
    }

    /// <summary> Stage 로직 </summary>
    void Update()
    {
        if (stageState == StageState.Pause) return;

        if (stageState == StageState.Play)
        {
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
                    $"{Mathf.FloorToInt(BossFieldLimitTime):D2}<size=96>." +
                    $"{(int)(BossFieldLimitTime * 100 % 100):D2}</size>";
            }
        }

        scoreText.text = Score.ToString();

        if (stageState == StageState.End && ShowResultCount < ShowResultCountLimit)
        {
            ShowResultTime += Time.deltaTime;
            ShowResult();
        }

        StageTime += Time.deltaTime;

        if (GameManager.instance.isDebug) DebugTest(true);
        else DebugTest(false);
    }

    /*************** Enemy 생성 매커니즘 ***************/

    /// <summary> Enemy 생성 데이터 적용 </summary>
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
            case 3: textFile = "Spawn_Stage3"; break;
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
            spawnData.enemyType =                   data_Dialog[i]["enemyType"].ToString();
            spawnData.posX =            float.Parse(data_Dialog[i]["posX"].ToString());
            spawnData.posY =            float.Parse(data_Dialog[i]["posY"].ToString());
            spawnData.dropItemName =                data_Dialog[i]["dropItemName"].ToString();

            spawnData.movX =            float.Parse(data_Dialog[i]["movX"].ToString());
            spawnData.movY =            float.Parse(data_Dialog[i]["movY"].ToString());
            spawnData.degreeZ =         float.Parse(data_Dialog[i]["degreeZ"].ToString());
            spawnData.movSpeed =        float.Parse(data_Dialog[i]["movSpeed"].ToString());
            spawnData.movingType =                  data_Dialog[i]["movingType"].ToString();
            spawnData.movDesX =         float.Parse(data_Dialog[i]["movDesX"].ToString());
            spawnData.movDesY =         float.Parse(data_Dialog[i]["movDesY"].ToString());
            spawnData.movExitX =        float.Parse(data_Dialog[i]["movExitX"].ToString());
            spawnData.movExitY =        float.Parse(data_Dialog[i]["movExitY"].ToString());
            spawnData.fieldTimeLimit =  float.Parse(data_Dialog[i]["fieldTimeLimit"].ToString());

            spawnData.bulletType =                  data_Dialog[i]["bulletType"].ToString();
            spawnData.bulletName =                  data_Dialog[i]["bulletName"].ToString();
            spawnData.patternType =                 data_Dialog[i]["patternType"].ToString();
            spawnData.bulletSpeed =     float.Parse(data_Dialog[i]["bulletSpeed"].ToString());
            spawnData.shootLimit =      int.Parse  (data_Dialog[i]["shootLimit"].ToString());
            spawnData.firstWaitTime =   float.Parse(data_Dialog[i]["firstWaitTime"].ToString());
            spawnData.waitTime =        float.Parse(data_Dialog[i]["waitTime"].ToString());

            spawnList.Add(spawnData);
        }
        spawnAmount = spawnList.Count;
        nextSpawnDelay = spawnList[0].delay;
    }

    /// <summary> Boss 데이터 적용 </summary>
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
            case 3: textFile = "BossLogic_C"; break;
            default:
                Debug.Log("스테이지 정보를 불러올 수 없습니다.");
                textFile = null; break;
        }

        List<Dictionary<string, object>> data_Dialog = CSVReader.Read(textFile);

        for (int i = 0; i < data_Dialog.Count; i++)
        {
            BossLogic bossLogicData = new BossLogic();

            bossLogicData.BossLogicCode =           data_Dialog[i]["BossLogicCode"].ToString();

            bossLogicData.delay =       float.Parse(data_Dialog[i]["delay"].ToString());
            bossLogicData.posX =        float.Parse(data_Dialog[i]["posX"].ToString());
            bossLogicData.posY =        float.Parse(data_Dialog[i]["posY"].ToString());

            bossLogicData.movX =        float.Parse(data_Dialog[i]["movX"].ToString());
            bossLogicData.movY =        float.Parse(data_Dialog[i]["movY"].ToString());
            bossLogicData.degreeZ =     float.Parse(data_Dialog[i]["degreeZ"].ToString());
            bossLogicData.movSpeed =    float.Parse(data_Dialog[i]["movSpeed"].ToString());
            bossLogicData.movingType =              data_Dialog[i]["movingType"].ToString();
            bossLogicData.movDesX =     float.Parse(data_Dialog[i]["movDesX"].ToString());
            bossLogicData.movDesY =     float.Parse(data_Dialog[i]["movDesY"].ToString());
            bossLogicData.movExitX =    float.Parse(data_Dialog[i]["movExitX"].ToString());
            bossLogicData.movExitY =    float.Parse(data_Dialog[i]["movExitY"].ToString());

            bossLogicData.shootPos1 =               data_Dialog[i]["pos1"].ToString();
            bossLogicData.shootPos2 =               data_Dialog[i]["pos2"].ToString();
            bossLogicData.shootPos3 =               data_Dialog[i]["pos3"].ToString();
            bossLogicData.shootPos4 =               data_Dialog[i]["pos4"].ToString();
            bossLogicData.shootPos5 =               data_Dialog[i]["pos5"].ToString();

            bossLogicData.bulletType =              data_Dialog[i]["bulletType"].ToString();
            bossLogicData.bulletName =              data_Dialog[i]["bulletName"].ToString();
            bossLogicData.patternType =             data_Dialog[i]["patternType"].ToString();
            bossLogicData.bulletSpeed = float.Parse(data_Dialog[i]["bulletSpeed"].ToString());
            bossLogicData.shootLimit =  int.Parse  (data_Dialog[i]["shootLimit"].ToString());
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

    /// <summary> Enemy 생성 로직 </summary>
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

            enemyLogic.getBulletType =  spawnList[spawnIndex].bulletType;
            enemyLogic.getBulletName =  spawnList[spawnIndex].bulletName;
            enemyLogic.getPatternType = spawnList[spawnIndex].patternType;
            enemyLogic.bulletSpeed =    spawnList[spawnIndex].bulletSpeed;
            enemyLogic.shootLimit =     spawnList[spawnIndex].shootLimit;
            enemyLogic.firstWaitTime =  spawnList[spawnIndex].firstWaitTime;
            enemyLogic.waitTime =       spawnList[spawnIndex].waitTime;

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
            enemyLogic.EnemyInit();
        }
    }

    /*************** 게임 결과 ***************/

    /// <summary> 게임 클리어 성공 </summary>
    public void GameClear()
    {
        spawnEnd = true;
        bossExist = false;
        StartCoroutine("GameResult");
    }

    /// <summary> 게임 클리어 실패 </summary>
    public void GameDefeat()
    {
        StartCoroutine("GameOver");
    }

    /// <summary> 게임 결과 정산 </summary>
    public IEnumerator GameResult()
    {
        yield return new WaitForSeconds(5f);
        InGameUI.SetActive(false);

        if (playerHealth >= 0)
        {
            BossTimeScore = (int)(BossFieldLimitTime * 100) * 10;
            RemainingScore = playerHealth * 40000;
        }
        ResultScore += Score + BossTimeScore + RemainingScore;

        stageState = StageState.End;
        ShowResultCountLimit = 5;
        gameResultPanel.SetActive(true);
    }

    /// <summary> 게임 결과 출력 </summary>
    public void ShowResult()
    {
        if (ShowResultTime >= 0.5f)
        {
            if (playerHealth < 0)
            {
                switch (ShowResultCount)
                {
                    case 0:
                        ResultText_Defeat[0].text = "Defeat"; break;
                    case 1:
                        ResultText_Defeat[1].text =
                            $"<size=72>Total Score</size>\n{Score.ToString()}"; break;
                    case 2: Back_Button_Defeat.SetActive(true); break;
                }
            }
            else
            {
                switch (ShowResultCount)
                {
                    case 0:
                        ResultText_Clear[0].text =
                            $"<size=64>Score</size> {Score.ToString()}"; break;
                    case 1:
                        ResultText_Clear[1].text =
                            $"<size=64>Boss-time</size> {BossTimeScore.ToString()}"; break;
                    case 2:
                        ResultText_Clear[2].text =
                            $"<size=64>Remaining</size> {RemainingScore.ToString()}"; break;
                    case 3:
                        ResultText_Clear[3].text =
                            $"<size=72>Total Score</size>\n{ResultScore.ToString()}"; break;
                    case 4: Back_Button_Clear.SetActive(true); break;
                }
            }
            ShowResultTime = 0; ShowResultCount++;
        }
    }

    /// <summary> 게임 오버 </summary>
    public IEnumerator GameOver()
    {
        yield return new WaitForSeconds(1.5f);
        InGameUI.SetActive(false);

        stageState = StageState.End;
        Time.timeScale = 1f;
        ShowResultCountLimit = 3;
        gameOverPanel.SetActive(true);
    }

    /// <summary> 게임 종료 - Stage 나가기 </summary>
    public void GameEnd()
    {
        LobbyManager.menuSelected = MenuSelected.Main;
        gameResultPanel.SetActive(false);
        gameOverPanel.SetActive(false);
        player.SetActive(false);
        Time.timeScale = 1f;

        LobbyManager.instance.EndStage();
    }

    /// <summary> 게임 일시정지 </summary>
    public void GamePause()
    {
        if (stageState == StageState.Ready || stageState == StageState.Play)
        {
            Time.timeScale = 0f;
            stageState = StageState.Pause;
            gamePausePanel.SetActive(true);
        }
    }

    /// <summary> 게임 재실행 </summary>
    public void GameResume()
    {
        Time.timeScale = 1f;
        stageState = StageState.Play;
        gamePausePanel.SetActive(false);
    }

    /*************** 게임 종료 여부 선택 모음 ***************/

    /// <summary> 게임 종료 여부 선택 </summary>
    public void GameQuitYN() { gamePausePanel.SetActive(false); gameQuitPanel.SetActive(true); }

    public void GameQuitN() { gamePausePanel.SetActive(true); gameQuitPanel.SetActive(false); }
    public void GameQuitY() { stageState = StageState.End; GameEnd(); }

    /*************** 디버그 관리 ***************/

    void DebugTest(bool isDebug)
    {
        if (isDebug)
        {
            debug_StageTime.gameObject.SetActive(true);
            debug_StageTime.text = $"{StageTime:N2}";
        }
        else
        {
            debug_StageTime.gameObject.SetActive(false);
        }
    }
}
