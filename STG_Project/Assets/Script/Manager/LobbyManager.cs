using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

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

    public GameObject selectPanelNow;

    public GameObject LoadScene_EffectL;
    public GameObject LoadScene_EffectR;

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
        Main, Stage, Setting, Credit
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
        LoadScene_EffectL.transform.position = new Vector2(-5, 0);
        LoadScene_EffectR.transform.position = new Vector2(5, 0);

        LobbyInit();
    }

    public void LobbyInit()
    {
        menuSelected = MenuSelected.Main;
        selectPanelNow = selectMenuPanel;
        selectMenuPanel.SetActive(true);
    }

    public void Select_Back() { menuSelected = MenuSelected.Main; ShowPanel(); }
    public void SelectStart() { menuSelected = MenuSelected.Stage; ShowPanel(); }
    public void SelectSetting() { menuSelected = MenuSelected.Setting; ShowPanel(); }
    public void SelectCredit() { menuSelected = MenuSelected.Credit; ShowPanel(); }

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
        }

        selectPanelNow.SetActive(true);
    }


    public void SelectStage1() => StartCoroutine("LoadScene", "InGame_Stage1");
    public void SelectStage2() => StartCoroutine("LoadScene", "InGame_Stage2");
    public void SelectStage3() => StartCoroutine("LoadScene", "InGame_Stage3");
    public void SelectStage4() => StartCoroutine("LoadScene", "InGame_Stage4");

    public void EndStage() => StartCoroutine("LoadScene", "Lobby");

    public IEnumerator LoadScene(string Stage)
    {
        isLoadScene = true;

        selectPanelNow.SetActive(false);

        switch (Stage)
        {
            case "InGame_Stage1": setStageNum = 1; break;
            //case "InGame_Stage2": setStageNum = 2; break;
            case "Lobby": setStageNum = 0; break;
        }

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

        SceneManager.LoadScene(Stage);
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
            LoadScene_EffectL.SetActive(true); LoadScene_EffectR.SetActive(true);

            LoadSceneTime = 0;
            LoadScene_EffectL.transform.position =
                Vector2.Lerp(LoadScene_EffectL.transform.position, new Vector2(-1.44f, 0), 0.1f);
            LoadScene_EffectR.transform.position =
                Vector2.Lerp(LoadScene_EffectR.transform.position, new Vector2(1.44f, 0), 0.1f);
        }
        else if (!isLoadScene && LoadSceneTime <= 0.5f)
        {
            LoadSceneTime += Time.deltaTime;
            LoadScene_EffectL.transform.position =
                Vector2.Lerp(LoadScene_EffectL.transform.position, new Vector2(-5, 0), 0.05f);
            LoadScene_EffectR.transform.position =
                Vector2.Lerp(LoadScene_EffectR.transform.position, new Vector2(5, 0), 0.05f);
        }
        else
        {
            LoadScene_EffectL.SetActive(false); LoadScene_EffectR.SetActive(false);
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
}
