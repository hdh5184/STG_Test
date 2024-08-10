using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct EnemyData
{
    // 1. 기본 속성
    public float delay;
    public string enemyType;
    public float posX;
    public float posY;
    public string dropItemName;

    // 2. 이동 속성
    public float movX;
    public float movY;
    public float degreeZ;
    public float movSpeed;
    public string movingType;
    public float movDesX;
    public float movDesY;
    public float movExitX;
    public float movExitY;

    // 3. 필드 출현 시간 (제한)
    public float fieldTimeLimit;

    // 4. 공격 속성
    public string bulletType;
    public string bulletName;
    public string patternType;
    public float bulletSpeed;
    public int shootLimit;
    public float firstWaitTime;
    public float waitTime;

    // 0. 편대 코드
    public string spawnCode;
}





public class Spawn
{
    /// <summary> Enemy 데이터 설정 </summary>
    public static void SetSpawnLogicData(string StageDataName, List<EnemyData> SpawnList)
    {
        // Enemy 컴포넌트 불러오기 및 초기화
        SpawnList.Clear();

        // Enemy 데이터 불러오기
        List<Dictionary<string, object>> data_Dialog = CSVReader.Read(StageDataName);

        // Enemy 데이터 적용
        for (int i = 0; i < data_Dialog.Count; i++)
        {
            EnemyData spawnData = new EnemyData();

            spawnData.spawnCode = data_Dialog[i]["spawnCode"].ToString();

            spawnData.delay = float.Parse(data_Dialog[i]["delay"].ToString());
            spawnData.enemyType = data_Dialog[i]["enemyType"].ToString();
            spawnData.posX = float.Parse(data_Dialog[i]["posX"].ToString());
            spawnData.posY = float.Parse(data_Dialog[i]["posY"].ToString());
            spawnData.dropItemName = data_Dialog[i]["dropItemName"].ToString();

            spawnData.movX = float.Parse(data_Dialog[i]["movX"].ToString());
            spawnData.movY = float.Parse(data_Dialog[i]["movY"].ToString());
            spawnData.degreeZ = float.Parse(data_Dialog[i]["degreeZ"].ToString());
            spawnData.movSpeed = float.Parse(data_Dialog[i]["movSpeed"].ToString());
            spawnData.movingType = data_Dialog[i]["movingType"].ToString();
            spawnData.movDesX = float.Parse(data_Dialog[i]["movDesX"].ToString());
            spawnData.movDesY = float.Parse(data_Dialog[i]["movDesY"].ToString());
            spawnData.movExitX = float.Parse(data_Dialog[i]["movExitX"].ToString());
            spawnData.movExitY = float.Parse(data_Dialog[i]["movExitY"].ToString());
            spawnData.fieldTimeLimit = float.Parse(data_Dialog[i]["fieldTimeLimit"].ToString());

            spawnData.bulletType = data_Dialog[i]["bulletType"].ToString();
            spawnData.bulletName = data_Dialog[i]["bulletName"].ToString();
            spawnData.patternType = data_Dialog[i]["patternType"].ToString();
            spawnData.bulletSpeed = float.Parse(data_Dialog[i]["bulletSpeed"].ToString());
            spawnData.shootLimit = int.Parse(data_Dialog[i]["shootLimit"].ToString());
            spawnData.firstWaitTime = float.Parse(data_Dialog[i]["firstWaitTime"].ToString());
            spawnData.waitTime = float.Parse(data_Dialog[i]["waitTime"].ToString());

            SpawnList.Add(spawnData);
        }
    }
}