using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class DebugTest : MonoBehaviour
{
    public PoolManager pool;
    public TextMeshProUGUI scoreText;
    public TextMeshProUGUI bossFieldLimitText;
    float onMouseTime = 0f; // 마우스 누르는 시간 카운트

    public MouseClickType mouseClickType;

    public float BossFieldLimitTime = 0;
    public bool bossExist;

    public enum MouseClickType
    {
        None, Coin, HomingFire
    }

    private void Start()
    {
        BossFieldLimitTime = 60;
        bossExist = false;
        scoreText.text = "0";
        bossFieldLimitText.text = "";
        mouseClickType = MouseClickType.None;
    }

    void Update()
    {
        if (bossExist)
        {
            BossFieldLimitTime -= Time.deltaTime;
            bossFieldLimitText.text =
                $"{Mathf.FloorToInt(BossFieldLimitTime)}<size=80>.{BossFieldLimitTime * 100 % 100:#,#00}</size>";
        }

        if (Input.GetKeyDown(KeyCode.LeftBracket)) LevelUp();       // [ : 플레이어 레벨 증가
        if (Input.GetKeyDown(KeyCode.RightBracket)) LevelDown();    // ] : 플레이어 레벨 감소

        // 마우스 누를 시 0.1초마다 필드 내 점수 아이템 생성
        if (Input.GetMouseButton(0))
        {
            onMouseTime += Time.deltaTime;
            if (onMouseTime >= 0.1f)
            {
                MousePos = Input.mousePosition;
                MousePos = Camera.main.ScreenToWorldPoint(MousePos);

                switch (mouseClickType)
                {
                    case MouseClickType.Coin: SummonCoin(); break;
                    case MouseClickType.HomingFire: SummonHomingFire(); break;
                }

                onMouseTime = 0f;

                Debug.Log(MousePos);
            }
        }
        else onMouseTime = 0f;

        scoreText.text = GameManager.Score.ToString();
    }

    public void LevelUp() =>
        GameManager.playerLevel = (GameManager.playerLevel == 4) ? 4 : GameManager.playerLevel + 1;
    public void LevelDown() =>
        GameManager.playerLevel = (GameManager.playerLevel == 1) ? 1 : GameManager.playerLevel - 1;


    Vector2 MousePos;   // 마우스 위치 저장

    public void SummonCoin()
    {
        GameObject Coin = pool.MakeObject("SilverCoin");
        Coin.transform.position = MousePos;
    }


    public void SummonHomingFire()
    {
        GameObject Coin = pool.MakeObject("Bullet_SG1_Homing");
        Coin.transform.position = MousePos;
    }
}
