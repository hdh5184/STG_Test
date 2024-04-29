using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.IO;
using UnityEngine.Audio;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    public PoolManager pool;
    public LobbyManager lobbyManager;
    public AudioManager audioManager;

    [SerializeField] private AudioMixer audioMixer;
    [SerializeField] private Slider MusicMasterSlider;

    public int setPlayerUnit = 0;
    public int setPlayerMoveType = 0;

    public bool isDebug = false;


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
        setPlayerUnit = PlayerPrefs.GetInt("PlayerType");
        lobbyManager.SelectedPlayerUnit.sprite =
            lobbyManager.PlayerUnitSprite[setPlayerUnit];

        setPlayerMoveType = PlayerPrefs.GetInt("MoveType");
        switch (setPlayerMoveType)
        {
            case 0: lobbyManager.Text_SetMoveType.text = "A"; break;
            case 1: lobbyManager.Text_SetMoveType.text = "B"; break;
        }

        MusicMasterSlider.value = PlayerPrefs.GetFloat("MasterVolume");
        MusicMasterSlider.onValueChanged.AddListener(SetMasterVolume);
    }

    public void SetPlayerUnit()
    {
        setPlayerUnit++;
        if (setPlayerUnit == 3) setPlayerUnit = 0;
        PlayerPrefs.SetInt("PlayerType", setPlayerUnit);

        lobbyManager.SelectedPlayerUnit.sprite =
            lobbyManager.PlayerUnitSprite[setPlayerUnit];
    }

    public void SetPlayerMoveType()
    {
        setPlayerMoveType++;
        if (setPlayerMoveType == 2) setPlayerMoveType = 0;
        PlayerPrefs.SetInt("MoveType", setPlayerMoveType);

        switch (setPlayerMoveType)
        {
            case 0: lobbyManager.Text_SetMoveType.text = "A"; break;
            case 1: lobbyManager.Text_SetMoveType.text = "B"; break;
        }
    }

    private void Start()
    {
        audioMixer.SetFloat("Master", Mathf.Log10(MusicMasterSlider.value) * 20);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.P))
        {
            isDebug = !isDebug;
            Debug.Log($"디버그 모드 : {isDebug}");
        }
    }


    public void SetMasterVolume(float volume)
    {
        audioMixer.SetFloat("Master", Mathf.Log10(volume) * 20);
        PlayerPrefs.SetFloat("MasterVolume", MusicMasterSlider.value);
    }
}