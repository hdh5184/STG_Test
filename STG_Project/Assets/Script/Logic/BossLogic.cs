using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossLogic
{
    public float delay;
    public float posX;
    public float posY;

    public float movX;
    public float movY;
    public float degreeZ;
    public float movSpeed;
    public string movingType;
    public float movDesX;
    public float movDesY;
    public float movExitX;
    public float movExitY;

    public string shootPos1, shootPos2, shootPos3, shootPos4, shootPos5;

    public string bulletType;
    public string bulletName;
    public string patternType;
    public float bulletSpeed;
    public int shootLimit;
    public float firstWaitTime;
    public float waitTime;

    public string BossLogicCode;

    public static void SetBossLogicData(string StageBossDataName, GameObject boss)
    {
        Enemy bossLogic = boss.GetComponent<Enemy>();
        bossLogic.bossLogics.Clear();
        bossLogic.bossLogicsFinal.Clear();

        List<Dictionary<string, object>> bossData = CSVReader.Read(StageBossDataName);

        for (int i = 0; i < bossData.Count; i++)
        {
            BossLogic bossLogicData = new BossLogic();

            bossLogicData.BossLogicCode = bossData[i]["BossLogicCode"].ToString();

            bossLogicData.delay = float.Parse(bossData[i]["delay"].ToString());
            bossLogicData.posX = float.Parse(bossData[i]["posX"].ToString());
            bossLogicData.posY = float.Parse(bossData[i]["posY"].ToString());

            bossLogicData.movX = float.Parse(bossData[i]["movX"].ToString());
            bossLogicData.movY = float.Parse(bossData[i]["movY"].ToString());
            bossLogicData.degreeZ = float.Parse(bossData[i]["degreeZ"].ToString());
            bossLogicData.movSpeed = float.Parse(bossData[i]["movSpeed"].ToString());
            bossLogicData.movingType = bossData[i]["movingType"].ToString();
            bossLogicData.movDesX = float.Parse(bossData[i]["movDesX"].ToString());
            bossLogicData.movDesY = float.Parse(bossData[i]["movDesY"].ToString());
            bossLogicData.movExitX = float.Parse(bossData[i]["movExitX"].ToString());
            bossLogicData.movExitY = float.Parse(bossData[i]["movExitY"].ToString());

            bossLogicData.shootPos1 = bossData[i]["pos1"].ToString();
            bossLogicData.shootPos2 = bossData[i]["pos2"].ToString();
            bossLogicData.shootPos3 = bossData[i]["pos3"].ToString();
            bossLogicData.shootPos4 = bossData[i]["pos4"].ToString();
            bossLogicData.shootPos5 = bossData[i]["pos5"].ToString();

            bossLogicData.bulletType = bossData[i]["bulletType"].ToString();
            bossLogicData.bulletName = bossData[i]["bulletName"].ToString();
            bossLogicData.patternType = bossData[i]["patternType"].ToString();
            bossLogicData.bulletSpeed = float.Parse(bossData[i]["bulletSpeed"].ToString());
            bossLogicData.shootLimit = int.Parse(bossData[i]["shootLimit"].ToString());
            bossLogicData.firstWaitTime = float.Parse(bossData[i]["firstWaitTime"].ToString());
            bossLogicData.waitTime = float.Parse(bossData[i]["waitTime"].ToString());

            if (bossData[i]["BossLogicCode"].ToString() == "Final")
            {
                bossLogic.bossLogicsFinal.Enqueue(bossLogicData); break;
            }
            bossLogic.bossLogics.Enqueue(bossLogicData);
        }

        bossLogic.firstWaitTime = 1.5f;
    }
}