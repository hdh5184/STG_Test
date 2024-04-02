using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Panel : MonoBehaviour
{
    bool isSelected = false;
    float movingTime = 0f;

    public PanelType panelType;
    public Canvas canvas;

    Vector2 PanelPos;
    Vector2 PanelMovingPos;

    public enum PanelType
    {
        Lobby_Menu, Lobby_Stage, Lobby_Setting, Lobby_Credit,
        Stage_Clear, Stage_Continue, Stage_GameOver,
        LoadScene_EffectL, LoadScene_EffectR
    }

    private void Awake()
    {
        canvas = GetComponent<Canvas>();

        PanelInit();
        transform.position = PanelPos;
    }

    void PanelInit()
    {
        switch (panelType)
        {
            case PanelType.Lobby_Menu:
                PanelPos = new Vector2(0, 0);
                PanelMovingPos = new Vector2(0, 0); break;
            case PanelType.Lobby_Stage: 
            case PanelType.Lobby_Setting: 
            case PanelType.Lobby_Credit: PanelPos = new Vector2(5, 0); break;

            case PanelType.Stage_Continue: PanelPos = new Vector2(0, 0); break;
            case PanelType.Stage_Clear:
            case PanelType.Stage_GameOver: PanelPos = new Vector2(0, 0); break;
        }
    }

    void Update()
    {
        if (isSelected)
        {
            movingTime += Time.deltaTime;

            transform.position = Vector2.Lerp(transform.position, PanelMovingPos, 0.3f);

            if (movingTime >= 1f)
            {
                isSelected = false;
                PanelMovingPos = PanelPos;
                PanelPos = transform.position;
            }
        }
    }

    public void MovingPanel()
    {
        isSelected = true;
        movingTime = 0;
    }

}
