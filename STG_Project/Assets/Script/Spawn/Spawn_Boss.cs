using System.Collections.Generic;
using UnityEngine;
using static Init_Enemy;

/* <Boss 데이터> */
public struct BossData
{
    // 1. 기본 속성
    public float delay;
    public float posX;
    public float posY;

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

    // 3. Boss 파츠 위치
    public bool[] isShoot;

    // 4. 공격 속성
    public string bulletType;
    public string bulletName;
    public string patternType;
    public float bulletSpeed;
    public int shootLimit;
    public float firstWaitTime;
    public float waitTime;

    // 0. Boss 진행 코드
    public string BossLogicCode;
}





/* <Boss 생성 클래스> */
public class Spawn_Boss
{
    /// <summary> Boss 데이터 불러오기 </summary>
    public static void SetBossLogicData(string stageName, Enemy_Boss boss)
    {
        // Enemy 컴포넌트 불러오기 및 초기화
        Queue<BossData> bossAttact = new Queue<BossData>();
        BossData finalAttackData = new BossData();

        // Enemy 데이터 불러오기
        List<Dictionary<string, object>> getData = CSVReader.Read(stageName);

        boss.queue_Attack = new Queue<BossData>();
        boss.finalAttack = new BossData();

        // Boss 데이터 적용
        for (int i = 0; i < getData.Count; i++)
        {
            BossData listData = new BossData();

            listData.BossLogicCode = getData[i]["BossLogicCode"].ToString();

            listData.delay = float.Parse(getData[i]["delay"].ToString());
            listData.posX = float.Parse(getData[i]["posX"].ToString());
            listData.posY = float.Parse(getData[i]["posY"].ToString());

            listData.movX = float.Parse(getData[i]["movX"].ToString());
            listData.movY = float.Parse(getData[i]["movY"].ToString());
            listData.degreeZ = float.Parse(getData[i]["degreeZ"].ToString());
            listData.movSpeed = float.Parse(getData[i]["movSpeed"].ToString());
            listData.movingType = getData[i]["movingType"].ToString();
            listData.movDesX = float.Parse(getData[i]["movDesX"].ToString());
            listData.movDesY = float.Parse(getData[i]["movDesY"].ToString());
            listData.movExitX = float.Parse(getData[i]["movExitX"].ToString());
            listData.movExitY = float.Parse(getData[i]["movExitY"].ToString());

            listData.isShoot = new bool[5];
            listData.isShoot[0] = (getData[i]["pos1"].ToString() == "1") ? true : false;
            listData.isShoot[1] = (getData[i]["pos2"].ToString() == "1") ? true : false;
            listData.isShoot[2] = (getData[i]["pos3"].ToString() == "1") ? true : false;
            listData.isShoot[3] = (getData[i]["pos4"].ToString() == "1") ? true : false;
            listData.isShoot[4] = (getData[i]["pos5"].ToString() == "1") ? true : false;

            listData.bulletType = getData[i]["bulletType"].ToString();
            listData.bulletName = getData[i]["bulletName"].ToString();
            listData.patternType = getData[i]["patternType"].ToString();
            listData.bulletSpeed = float.Parse(getData[i]["bulletSpeed"].ToString());
            listData.shootLimit = int.Parse(getData[i]["shootLimit"].ToString());
            listData.firstWaitTime = float.Parse(getData[i]["firstWaitTime"].ToString());
            listData.waitTime = float.Parse(getData[i]["waitTime"].ToString());


            if (getData[i]["BossLogicCode"].ToString() == "Final")
            finalAttackData = listData;

            else bossAttact.Enqueue(listData);
        }

        boss.queue_Attack = bossAttact;
        boss.finalAttack = finalAttackData;
    }

    /// <summary> Boss 데이터 적용 </summary>
    public static void SetBossData(Enemy_Boss boss, BossData data)
    {
        // 2. Vector
        boss.moveVec = new Vector2(data.movX, data.movY).normalized;
        boss.moveDesVec = new Vector2(data.movDesX, data.movDesY);
        boss.moveExitVec = new Vector2(data.movExitX, data.movExitY).normalized;

        // 3. Enemy Attribute
        boss.degreeZ = data.degreeZ;
        boss.movSpeed = data.movSpeed;
        boss.getMovingType = data.movingType;

        // 4. Bullet Attribute
        boss.getBulletType = data.bulletType;
        boss.getBulletName = data.bulletName;
        boss.getPatternType = data.patternType;
        boss.bulletSpeed = data.bulletSpeed;
        boss.shootLimit = data.shootLimit;

        boss.shootPos_isShoot = data.isShoot;

        // 5. Time
        boss.waitTime = data.delay;

        Init_Pattern(boss, boss.getPatternType);
        Init_Moving(boss, boss.getMovingType);

        boss.queue_Attack.Enqueue(data);
    }
}