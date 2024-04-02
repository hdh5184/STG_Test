using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.IO;
using UnityEditor.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    public PoolManager pool;
    public LobbyManager lobbyManager;
    public AudioManager audioManager;
    //public StageManager stageManager;

    


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

    

    void Start()
    {
        
    }

    void Update()
    {
        
    }

    

    
}
