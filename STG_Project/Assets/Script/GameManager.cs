using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.IO;
using UnityEditor.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    public PoolManager pool;    // 오브젝트 Pool
    public AudioManager audioManager;
    public StageManager stageManager;

    public string setPlayerType;
    public int setStageNum;

    // 적기 코드 저장









    // 플레이어 위치 및 이동 벡터



    private void Awake()
    {
        if (instance != this && instance != null)
        {
            Destroy(gameObject); return;
        }
        else
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }

    }

    

    public void SelectStage1() => StartGame("InGame_Stage1");
    public void SelectStage2() => StartGame("InGame_Stage2");

    public void StartGame(string Stage)
    {
        stageManager.getPlayerType = 'A';

        switch (Stage)
        {
            case "InGame_Stage1": setStageNum = 1; break;
            case "InGame_Stage2": setStageNum = 2; break;
        }


        SceneManager.LoadScene(Stage);
        
        stageManager.StageInit();
    }

    

    
    

    

    void Start()
    {
        
    }

    void Update()
    {
        
    }

    

    
}
