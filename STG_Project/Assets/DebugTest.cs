using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class DebugTest : MonoBehaviour
{
    public PoolManager pool;
    public TextMeshProUGUI scoreText;
    float onMouseTime = 0f; // 마우스 누르는 시간 카운트

    private void Start()
    {
        scoreText.text = "0";
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.LeftBracket)) LevelUp();       // [ : 플레이어 레벨 증가
        if (Input.GetKeyDown(KeyCode.RightBracket)) LevelDown();    // ] : 플레이어 레벨 감소

        // 마우스 누를 시 0.1초마다 필드 내 점수 아이템 생성
        if (Input.GetMouseButton(0))
        {
            onMouseTime += Time.deltaTime;
            if (onMouseTime >= 0.1f) SummonCoin();
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
        MousePos = Input.mousePosition;
        MousePos = Camera.main.ScreenToWorldPoint(MousePos);

        for (int j = 0; j < pool.poolItem_SilverCoin.Length; j++)
        {
            GameObject Coin = pool.poolItem_SilverCoin[j];
            if (Coin.activeSelf == false)
            {
                Coin.transform.position = MousePos;
                Coin.SetActive(true);
                break;
            }
        }

        onMouseTime = 0f;
        
        Debug.Log(MousePos);
    }

}
