using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Enemy;
using static UnityEngine.RuleTile.TilingRuleOutput;

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

        //if (enemy.enemyType != EnemyType.Boss) enemy.bulletPatterns.Enqueue(enemy.bulletPattern);
        enemy.transform.rotation = Quaternion.Euler(0, 0, enemy.degreeZ);
    }

    /// <summary> Enemy - 공격, 이동, 기체 회전 속성 초기화 (+ 보스 공격) </summary>
    public static void Init_Pattern(Enemy enemy, string name_Pattern)
    {
        switch (name_Pattern)
        {
            case "str": enemy.attackType = AttackType.Straight; break;
            case "way": enemy.attackType = AttackType.n_Way; break;
            case "cir": enemy.attackType = AttackType.Circle; break;
            case "spr": enemy.attackType = AttackType.Spread; break;
            case "sprR": enemy.attackType = AttackType.Spread_Random; break;
            case "vtx": enemy.attackType = AttackType.Vortex; break;
            case "down": enemy.attackType = AttackType.Down; break;
            case "none": enemy.attackType = AttackType.None; break;
        }
    }

    public static void Init_Moving(Enemy enemy, string name_Moving)
    {
        switch (name_Moving)
        {
            case "str": enemy.moveType = MoveType.Straight; break;
            case "acc": enemy.moveType = MoveType.Accel; break;
            case "slow": enemy.moveType = MoveType.SlowDown; break;
        }
    }

    public static void Init_MoveData(Enemy enemy, ref MoveData data)
    {
        data.transform = enemy.transform;
        data.moveVec = enemy.moveVec;
        data.movSpeed = enemy.movSpeed;
        data.fieldTime = enemy.fieldTime;
    }

    public static void Init_AttackData(Enemy enemy, ref AttackData data)
    {
        data.pool = enemy.pool;
        data.getBulletName = enemy.getBulletName;
        data.getBulletType = enemy.getBulletType;
        data.shootCount = enemy.shootCount;
        data.shootLimit = enemy.shootLimit;
        data.bulletSpeed = enemy.bulletSpeed;
        data.degree = enemy.degree;
        data.degreeLimit = 180;
    }

}