using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;
using Unity.VisualScripting;

public class LobbyManager : MonoBehaviour
{
    public static LobbyManager instance;

    public AudioSource audio;

    public GameObject Lobby_UI;
    public GameObject GameTitle;

    public GameObject selectMenuPanel;
    public GameObject selectStagePanel;
    public GameObject selectSettingPanel;
    public GameObject selectCreditPanel;
    public GameObject selectExitPanel;

    public GameObject selectPanelNow;

    public Sprite[] PlayerUnitSprite;
    public Image SelectedPlayerUnit;

    //public GameObject LoadScene_EffectL;
    //public GameObject LoadScene_EffectR;

    public RectTransform LoadScene_EffectL;
    public RectTransform LoadScene_EffectR;

    public TextMeshProUGUI Text_Credit;

    public string setPlayerType;
    public int setStageNum;

    float LoadSceneTime = 10;
    bool isLoadScene = false;

    int FontColorCode = 0xFF8000; // 0xFF2400
    byte FontColorGreen = 220;
    float FontEffectTime = 0;
    bool FontEffectTrigger = true;

    public static MenuSelected menuSelected;

    public enum MenuSelected
    {
        Main, Stage, Setting, Credit, Exit
    }

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

        audio = GetComponent<AudioSource>();

        //LoadScene_EffectL = GetComponent<RectTransform>();
        //LoadScene_EffectR = GetComponent<RectTransform>();

        //LoadScene_EffectL.transform.position = new Vector2(-5, 0);
        //LoadScene_EffectR.transform.position = new Vector2(5, 0);

        LoadScene_EffectL.anchoredPosition = new Vector3(-1000, 0);
        LoadScene_EffectR.anchoredPosition = new Vector3(1000, 0);

        LobbyInit();
    }

    public void LobbyInit()
    {
        menuSelected = MenuSelected.Main;
        selectPanelNow = selectMenuPanel;
        SelectedPlayerUnit.sprite = PlayerUnitSprite[0];
        selectMenuPanel.SetActive(true);
    }

    public void Select_Back() { menuSelected = MenuSelected.Main; ShowPanel(); }
    public void SelectStart() { menuSelected = MenuSelected.Stage; ShowPanel(); }
    public void SelectSetting() { menuSelected = MenuSelected.Setting; ShowPanel(); }
    public void SelectCredit() { menuSelected = MenuSelected.Credit; ShowPanel(); }
    public void SelectExit() { menuSelected = MenuSelected.Exit; ShowPanel(); }

    public void ShowPanel()
    {
        audio.clip = AudioManager.instance.getAudioClip("Button");
        audio.Play();

        selectPanelNow.SetActive(false);

        switch (menuSelected)
        {
            case MenuSelected.Main: selectPanelNow = selectMenuPanel; break;
            case MenuSelected.Stage: selectPanelNow = selectStagePanel; break;
            case MenuSelected.Setting: selectPanelNow = selectSettingPanel; break;
            case MenuSelected.Credit: selectPanelNow = selectCreditPanel; break;
            case MenuSelected.Exit: selectPanelNow = selectExitPanel; break;
        }

        selectPanelNow.SetActive(true);
    }


    public void SelectStage1() => StartCoroutine("LoadScene", 1);
    public void SelectStage2() => StartCoroutine("LoadScene", 2);
    public void SelectStage3() => StartCoroutine("LoadScene", 3);
    public void SelectStage4() => StartCoroutine("LoadScene", 4);

    public void EndStage() => StartCoroutine("LoadScene", 0);

    public IEnumerator LoadScene(int StageNum)
    {
        isLoadScene = true;

        selectPanelNow.SetActive(false);


        setStageNum = StageNum;

        audio.clip = AudioManager.instance.getAudioClip("Trigger");
        audio.Play();

        yield return new WaitForSeconds(1.5f);

        if (setStageNum != 0)
        {
            GameTitle.SetActive(false);
            audio.clip = AudioManager.instance.getAudioClip("MoveDown");
            audio.Play();
        }
        else {
            GameTitle.SetActive(true);
            LobbyInit();
        }

        switch (StageNum)
        {
            case 0: SceneManager.LoadScene("Lobby"); break;
            default: SceneManager.LoadScene("InGame_Stage"); break;
        }

        StageManager.stageState = StageManager.StageState.Lobby;
        isLoadScene = false;
    }

    void Update()
    {
        LoadSceneEffect();
        FontEffect();
    }

    void LoadSceneEffect()
    {
        if (isLoadScene)
        {
            LoadScene_EffectL.gameObject.SetActive(true);
            LoadScene_EffectR.gameObject.SetActive(true);

            LoadSceneTime = 0;
            LoadScene_EffectL.anchoredPosition =
                Vector2.Lerp(LoadScene_EffectL.anchoredPosition, new Vector3(-270, 0), 0.1f);
            LoadScene_EffectR.anchoredPosition =
                Vector2.Lerp(LoadScene_EffectR.anchoredPosition, new Vector3(270, 0), 0.1f);
        }
        else if (!isLoadScene && LoadSceneTime <= 0.5f)
        {
            LoadSceneTime += Time.deltaTime;
            LoadScene_EffectL.anchoredPosition =
                Vector2.Lerp(LoadScene_EffectL.anchoredPosition, new Vector3(-1000, 0), 0.05f);
            LoadScene_EffectR.anchoredPosition =
                Vector2.Lerp(LoadScene_EffectR.anchoredPosition, new Vector3(1000, 0), 0.05f);
        }
        else
        {
            LoadScene_EffectL.gameObject.SetActive(false);
            LoadScene_EffectR.gameObject.SetActive(false);
        }
    }

    void FontEffect()
    {
        if (menuSelected != MenuSelected.Credit) return;

        FontEffectTime += Time.deltaTime;
        
        if (FontEffectTime >= 0.001f)
        {
            if (FontEffectTrigger) FontColorGreen -= 2;
            else FontColorGreen += 2;
            FontEffectTime = 0;
        }

        Text_Credit.color = new Color32(255, FontColorGreen, 0, 255);

        if (FontColorGreen == 128) FontEffectTrigger = false;
        if (FontColorGreen == 228) FontEffectTrigger = true;
    }

    public void GameExit()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#elif UNITY_ANDROID
        AndroidJavaObject activity =
            new AndroidJavaClass("com.unity3d.player.UnityPlayer").GetStatic<AndroidJavaObject>("currentActivity");
        activity.Call("finish");
#else
        Application.Quit();
#endif
    }
}
