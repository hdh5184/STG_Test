using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using static Spawn;
using static Spawn_Boss;

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

    public Slider MusicMasterSlider;
    public TextMeshProUGUI Text_SetMoveType;
    public TextMeshProUGUI Text_SetMoveTypeDes;

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

    public static int score = 0;
    float StageTime = 0;
    bool isGameClear = false;

    string StageDataName;
    string StageBossDataName;

    // 4. 플레이어
    public GameObject player;
    public static Vector3 playerPos;
    public Vector3 playerMovingVec;
    public static int playerLevel = 1;
    public static int playerHealth = 2;

    // 5. Enemy 관리
    public static List<GameObject> EnemyList;
    public List<EnemyData> spawnList;
    public Queue<BossData> bossAttactQueue;
    public BossData bossFinalAttact;

    public int spawnIndex;
    public int spawnAmount;
    public float currentSpawnTime;
    public float nextSpawnDelay;
    public bool spawnEnd;

    public float BossFieldLimitTime = 0;
    public static bool bossExist;

    // e. 속성 모음
    public enum StageState { Lobby, Ready, Play, End, Pause }




    public static bool isGamePlay = false;



    /*************** 게임 루프 ***************/

    private void Awake() => instance = this;

    /// <summary> 스테이지 입장 </summary>
    private void OnEnable()
    {
        // 1. Load Manager
        pool = PoolManager.instance;
        audioManager = AudioManager.instance;

        // 2. Init
        StageInit();
        UI_Init();
        PlayerInit();

        // 3. Set Data
        SetStageDataName();
        SetSpawnData();

        // 4. Starting
        isGamePlay = true;
        Invoke("GameStart", 2f);
        Debug.Log("게임 시작");
    }

    /// <summary> Stage 로직 </summary>
    void Update()
    {
        if (stageState == StageState.Pause) return;

        Timing();

        switch (stageState)
        {
            case StageState.Play:
                SetPMovingVec();
                SpawnEnemy();

                if (bossExist)
                ShowBossState();

                scoreText.text = score.ToString();
                break;

            case StageState.End:
                ShowResult();
                break;
        }

        if (GameManager.isDebug && !GameManager.isDebug_HideDebug)
            DebugTest(true);
        else DebugTest(false);
    }





    /*************** 스테이지 초기화 매커니즘 ***************/

    /// <summary> 스테이지 시작 </summary>
    public void GameStart() => stageState = StageState.Play;

    /// <summary> 스테이지 초기화 </summary>
    public void StageInit()
    {
        // 1. List Data
        EnemyList = new List<GameObject>();
        spawnList = new List<EnemyData>();

        // 2. Stage Data
        stageState = StageState.Ready;
        getStageNum = LobbyManager.instance.setStageNum;

        // 2-1. Player
        playerLevel = 1;
        playerHealth = 2;

        // 2-2. Score
        score = 0;
        BossTimeScore = 0;
        RemainingScore = 0;

        // 2-3. Time
        currentSpawnTime = 0;
        nextSpawnDelay = 0;
        BossFieldLimitTime = 60;

        // 2-4. Sub Status
        bossExist = false;
        isGameClear = false;
    }

    /// <summary> UI 초기화 </summary>
    void UI_Init()
    {
        // 1. UI
        InGameUI.SetActive(true);

        Ui_Stage.SetActive(true);
        gameResultPanel.SetActive(false);
        foreach (var item in playerRemain_UI) item.SetActive(false);
        playerRemain_UI[GameManager.instance.setPlayerUnit].SetActive(true);
        Back_Button_Clear.SetActive(false);
        Back_Button_Defeat.SetActive(false);
        gamePausePanel.SetActive(false);
        gameOverPanel.SetActive(false);

        // 2. Text
        switch (GameManager.instance.setPlayerMoveType)
        {
            case 0:
                Text_SetMoveType.text = "A";
                Text_SetMoveTypeDes.text = "<조작법 A - 드래그>\n터치한 곳을 기준으로\n기체가 움직입니다.";
                break;
            case 1:
                Text_SetMoveType.text = "B";
                Text_SetMoveTypeDes.text = "<조작법 B - 타겟팅>\n터치한 곳을 따라서\n기체가 움직입니다.";
                break;
        }

        foreach (var item in ResultText_Clear) item.text = "";
        foreach (var item in ResultText_Defeat) item.text = "";
        remainText.text = "2";
        scoreText.text = "0";
        bossFieldLimitText.text = "";

        // 3. Background
        background_sr = background.GetComponent<SpriteRenderer>();
        background_sr.sprite = backgroundSprite[getStageNum - 1];

        // 4. Sound
        MusicMasterSlider.value = PlayerPrefs.GetFloat("MasterVolume");
        MusicMasterSlider.onValueChanged.AddListener(SetMasterVolume);

        // 5. var
        ShowResultCountLimit = 0;
        ShowResultCount = 0;
    }

    /// <summary> Player 초기화 </summary>
    void PlayerInit()
    {
        // Player 완전 비활성화
        if (player != null) player.SetActive(false);

        // Player 타입 가져오기
        getPlayerType = GameManager.instance.setPlayerUnit;

        switch (getPlayerType)
        {
            case 0: player = pool.MakeObject("Player_A"); break;
            case 1: player = pool.MakeObject("Player_B"); break;
            case 2: player = pool.MakeObject("Player_C"); break;
        }

        // Player에 필요한 매니저 연결 및 초기화
        Player PComponent = player.GetComponent<Player>();
        PComponent.stageManager = instance;
        PComponent.PlayerInit();

        player.transform.position = new Vector2(0, -3);
        playerPos = player.transform.position;
    }

    /// <summary> 스테이지 데이터 파일 불러오기 </summary>
    void SetStageDataName()
    {
        // 스테이지에 따른 파일 이름 설정
        switch (getStageNum)
        {
            case 1:
                //StageDataName = "Spawn_Stage1 copy";
                StageDataName = "Spawn_Stage1";
                StageBossDataName = "BossLogic_A"; break;
            case 2:
                StageDataName = "Spawn_Stage2";
                StageBossDataName = "BossLogic_B"; break;
            case 3:
                StageDataName = "Spawn_Stage3";
                StageBossDataName = "BossLogic_C"; break;
            case 4:
                StageDataName = "Spawn_Stage4";
                StageBossDataName = "BossLogic_D"; break;
            default:
                Debug.Log("스테이지 정보를 불러올 수 없습니다.");
                StageDataName = null;
                StageBossDataName = null; break;
        }
    }

    /// <summary> Enemy 생성 데이터 적용 </summary>
    void SetSpawnData()
    {
        // Enemy 생성 데이터 적용
        SetSpawnLogicData(StageDataName, spawnList);

        // Enemy 생성 관련 변수 초기화
        spawnIndex = 0;
        spawnEnd = false;
        spawnAmount = spawnList.Count;
        nextSpawnDelay = spawnList[0].delay;
    }





    /*************** Stage 요소 관련 매커니즘 ***************/

    /// <summary> 시간 측정 </summary>
    void Timing()
    {
        // 스테이지 진행 시간
        StageTime += Time.deltaTime;

        // Enemy 생성 시간
        if (stageState == StageState.Play & !spawnEnd)
        currentSpawnTime += Time.deltaTime;

        // Boss 잔여 시간
        if (bossExist)
        BossFieldLimitTime -= Time.deltaTime;

        // 결과 출력 관련 시간
        if (stageState == StageState.End && ShowResultCount < ShowResultCountLimit)
        ShowResultTime += Time.deltaTime;
    }

    /// <summary> Player 이동 방향 저장 </summary>
    void SetPMovingVec()
    {
        // Player 이동 방향 및 위치 갱신
        playerMovingVec = player.transform.position - playerPos;
        playerPos = player.transform.position;
    }

    /// <summary> Boss 정보 출력 </summary>
    void ShowBossState()
    {
        // Boss 잔여 시간 없음
        if (BossFieldLimitTime <= 0)
        {
            bossExist = false;
            BossFieldLimitTime = 0;
            bossFieldLimitText.text = $"0<size=96>.00</size>";
        }
        // Boss 전투 진행 중 - 잔여 시간 출력
        else
        {
            bossFieldLimitText.text =
                $"{Mathf.FloorToInt(BossFieldLimitTime):D2}<size=96>." +
                $"{(int)(BossFieldLimitTime * 100 % 100):D2}</size>";
        }
    }





    /*************** Enemy 생성 매커니즘 ***************/

    /// <summary> Boss 데이터 적용 </summary>

    /// <summary> Enemy 생성 로직 </summary>
    void SpawnEnemy()
    {
        // 스폰 종료 및 디버그 모드-스폰 정지 시 함수 종료
        if (spawnEnd) return;
        if (GameManager.isDebug_StopSpawn && GameManager.isDebug) return;

        // 생성 시간 도달 시 Enemy 생성
        if (currentSpawnTime >= nextSpawnDelay)
        {
            // 1. 생성 데이터 불러오기
            EnemyData spawnData = spawnList[spawnIndex];
            Debug.Log($"편대 {spawnData.spawnCode}번");

            // 2. Enemy 생성 및 위치 배치
            GameObject enemy = pool.MakeObject(spawnData.enemyType, false);
            enemy.transform.position = new Vector2(spawnData.posX, spawnData.posY);

            // 3. Enemy 데이터 적용 및 초기화
            Enemy enemyLogic = enemy.GetComponent<Enemy>();
            //enemyLogic.audioManager = audioManager;

            SetEnemyData(enemyLogic, spawnData);

            // @. 보스 출현 시 보스 초기화 및 보스 전투 진행 처리
            if (enemyLogic.isBoss)
            {
                bossExist = true;
                Enemy_Boss boss = enemy.GetComponent<Enemy_Boss>();
                SetBossLogicData(StageBossDataName, boss);
            }

            enemy.SetActive(true);

            //enemyLogic.SetEnemyData(spawnData);
            //enemyLogic.EnemyInit();

            // 4. 후속 처리 및 다음 생성 시간 갱신
            // *. Enemy 모두 생성 완료 시 Enemy 생성 로직 정지
            spawnIndex++;
            currentSpawnTime = 0;

            if (spawnAmount == spawnIndex) spawnEnd = true;
            else nextSpawnDelay = spawnList[spawnIndex].delay;

            

            

            // 5. 필드 내에 존재하는 Enemy 목록에 추가
            EnemyList.Add(enemy);
        }
    }





    /*************** 게임 결과 출력 매커니즘 ***************/

    /// <summary> 게임 클리어 성공 </summary>
    public void GameClear()
    {
        spawnEnd = true;
        bossExist = false;
        isGameClear = true;
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
            RemainingScore = playerHealth * 20000;
        }
        ResultScore += score + BossTimeScore + RemainingScore;

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
                            $"<size=72>Total Score</size>\n{score.ToString()}"; break;
                    case 2: Back_Button_Defeat.SetActive(true); break;
                }
            }
            else
            {
                switch (ShowResultCount)
                {
                    case 0:
                        ResultText_Clear[0].text =
                            $"<size=64>Score</size> {score.ToString()}"; break;
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
        //LobbyManager.menuSelected = MenuSelected.Main;
        gameResultPanel.SetActive(false);
        gameOverPanel.SetActive(false);
        player.SetActive(false);
        Time.timeScale = 1f;

        if (isGameClear && getStageNum == 4)
            LobbyManager.instance.EndStage(true);
        else
            LobbyManager.instance.EndStage();
    }

    /// <summary> 게임 일시정지 </summary>
    public void GamePause()
    {
        if (stageState == StageState.Ready || stageState == StageState.Play)
        {
            isGamePlay = false;
            Time.timeScale = 0f;
            stageState = StageState.Pause;
            gamePausePanel.SetActive(true);
        }
    }

    /// <summary> 게임 재실행 </summary>
    public void GameResume()
    {
        isGamePlay = true;
        Time.timeScale = 1f;
        stageState = StageState.Play;
        gamePausePanel.SetActive(false);
    }





    /*************** 게임 일시정지 관련 상호작용 ***************/

    /// <summary> 게임 종료 여부 선택 </summary>
    public void GameQuitYN() { gamePausePanel.SetActive(false); gameQuitPanel.SetActive(true); }

    /// <summary> 게임 종료 </summary>
    public void GameQuitY() { stageState = StageState.End; GameEnd(); }
    /// <summary> 이전 </summary>
    public void GameQuitN() { gamePausePanel.SetActive(true); gameQuitPanel.SetActive(false); }

    /// <summary> 스테이지 내 음량 갱신 </summary>
    public void SetMasterVolume(float volume)
    {
        GameManager.instance.audioMixer.SetFloat("Master", Mathf.Log10(volume) * 20);
        PlayerPrefs.SetFloat("MasterVolume", MusicMasterSlider.value);
    }

    /// <summary> 스테이지 내 Player 이동 타입 설정 </summary>
    public void setPlayerMoveType()
    {
        GameManager.instance.SetPlayerMoveType(false);
        switch (GameManager.instance.setPlayerMoveType)
        {
            case 0:
                Text_SetMoveType.text = "A";
                Text_SetMoveTypeDes.text = "<조작법 A - 드래그>\n터치한 곳을 기준으로\n기체가 움직입니다.";
                break;
            case 1:
                Text_SetMoveType.text = "B";
                Text_SetMoveTypeDes.text = "<조작법 B - 타겟팅>\n터치한 곳을 따라서\n기체가 움직입니다.";
                break;
        }
    }





    /*************** 디버그 관리 ***************/

    void DebugTest(bool isDebug)
    {
        if (isDebug)
        {
            debug_StageTime.gameObject.SetActive(true);
            debug_StageTime.text = $"{StageTime:N2}";

            if (GameManager.isDebug_PlayerNonHit)
                debug_StageTime.text += "\nNon-Hit";
            if (GameManager.isDebug_StopSpawn)
                debug_StageTime.text += "\nNon-Spawn";
        }
        else
        {
            debug_StageTime.gameObject.SetActive(false);
        }
    }
}
