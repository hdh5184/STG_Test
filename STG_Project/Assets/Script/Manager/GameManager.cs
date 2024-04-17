using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.IO;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    public PoolManager pool;
    public LobbyManager lobbyManager;
    public AudioManager audioManager;

    public int setPlayerUnit = 0;

    


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

        Application.targetFrameRate = 60;
    }

    

    void Start()
    {
        
    }

    void Update()
    {
        
    }

    public void SetPlayerUnit()
    {
        setPlayerUnit++;
        if (setPlayerUnit == 3) setPlayerUnit = 0;

        lobbyManager.SelectedPlayerUnit.sprite =
            lobbyManager.PlayerUnitSprite[setPlayerUnit];
    }

    

    
}
