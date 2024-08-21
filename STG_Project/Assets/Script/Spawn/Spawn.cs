using System.Collections.Generic;
using UnityEngine;
using static Init_Enemy;

/* <Enemy 데이터> */
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





/* <Enemy 생성 클래스> */
public class Spawn
{
    /// <summary> Enemy 데이터 불러오기 </summary>
    public static void SetSpawnLogicData(string stageName, List<EnemyData> spawnList)
    {
        // Enemy 컴포넌트 불러오기 및 초기화
        spawnList.Clear();

        // Enemy 데이터 불러오기
        List<Dictionary<string, object>> getData = CSVReader.Read(stageName);

        // Enemy 데이터 적용
        for (int i = 0; i < getData.Count; i++)
        {
            EnemyData listData = new EnemyData();

            listData.spawnCode = getData[i]["spawnCode"].ToString();

            listData.delay = float.Parse(getData[i]["delay"].ToString());
            listData.enemyType = getData[i]["enemyType"].ToString();
            listData.posX = float.Parse(getData[i]["posX"].ToString());
            listData.posY = float.Parse(getData[i]["posY"].ToString());
            listData.dropItemName = getData[i]["dropItemName"].ToString();

            listData.movX = float.Parse(getData[i]["movX"].ToString());
            listData.movY = float.Parse(getData[i]["movY"].ToString());
            listData.degreeZ = float.Parse(getData[i]["degreeZ"].ToString());
            listData.movSpeed = float.Parse(getData[i]["movSpeed"].ToString());
            listData.movingType = getData[i]["movingType"].ToString();
            listData.movDesX = float.Parse(getData[i]["movDesX"].ToString());
            listData.movDesY = float.Parse(getData[i]["movDesY"].ToString());
            listData.movExitX = float.Parse(getData[i]["movExitX"].ToString());
            listData.movExitY = float.Parse(getData[i]["movExitY"].ToString());
            listData.fieldTimeLimit = float.Parse(getData[i]["fieldTimeLimit"].ToString());

            listData.bulletType = getData[i]["bulletType"].ToString();
            listData.bulletName = getData[i]["bulletName"].ToString();
            listData.patternType = getData[i]["patternType"].ToString();
            listData.bulletSpeed = float.Parse(getData[i]["bulletSpeed"].ToString());
            listData.shootLimit = int.Parse(getData[i]["shootLimit"].ToString());
            listData.firstWaitTime = float.Parse(getData[i]["firstWaitTime"].ToString());
            listData.waitTime = float.Parse(getData[i]["waitTime"].ToString());

            spawnList.Add(listData);
        }
    }

    /// <summary> Enemy 데이터 적용 </summary>
    public static void SetEnemyData(Enemy enemy, EnemyData data)
    {
        // 1. Item
        enemy.getDropItemName = data.dropItemName;

        // 2. Vector
        enemy.moveVec = new Vector2(data.movX, data.movY).normalized;
        enemy.moveDesVec = new Vector2(data.movDesX, data.movDesY);
        enemy.moveExitVec = new Vector2(data.movExitX, data.movExitY).normalized;

        // 3. Enemy Attribute
        enemy.degreeZ = data.degreeZ;
        enemy.movSpeed = data.movSpeed;
        enemy.getMovingType = data.movingType;
        enemy.fieldTimeLimit = data.fieldTimeLimit;

        // 4. Bullet Attribute
        enemy.getBulletType = data.bulletType;
        enemy.getBulletName = data.bulletName;
        enemy.getPatternType = data.patternType;
        enemy.bulletSpeed = data.bulletSpeed;
        enemy.shootLimit = data.shootLimit;

        // 5. Time
        enemy.firstWaitTime = data.firstWaitTime;
        enemy.waitTime = data.waitTime;

        Init_Pattern(enemy, enemy.getPatternType);
        Init_Moving(enemy, enemy.getMovingType);

        enemy.transform.rotation = Quaternion.Euler(0, 0, enemy.degreeZ);
    }
}