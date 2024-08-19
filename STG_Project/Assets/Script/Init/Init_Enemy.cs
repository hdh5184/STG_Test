using UnityEngine;
using static Enemy;

/* <Enemy 초기화 모음> */
public class Init_Enemy : MonoBehaviour
{
    /*************** Enemy 요소 초기화 모음 ***************/

    /// <summary> Enemy 고정 속성 초기화 </summary>
    public static void Init(Enemy enemy, int score, int health, GameObject[] obj = null)
    {
        enemy.setScore = score;
        enemy.setHealth = health;

        // 공격 위치, 공격 각도, 공격 유무 초기화
        if (obj == null)
        {
            enemy.shootPos = new Transform[1];
            enemy.shootPos_Degree = new float[1];
            enemy.shootPos_isShoot = new bool[1];
            enemy.shootPos[0] = Init_ShootPos(enemy);
            enemy.shootPos_isShoot[0] = true;
        }
        else
        {
            enemy.shootPos = new Transform[obj.Length];
            enemy.shootPos_Degree = new float[obj.Length];
            enemy.shootPos_isShoot = new bool[obj.Length];

            for (int i = 0; i < obj.Length; i++)
            {
                enemy.shootPos[i] = Init_ShootPos(enemy, obj[i]);
                enemy.shootPos_Degree[i] = 0;
                enemy.shootPos_isShoot[i] = true;
            }
        }
    }

    /// <summary> Enemy 공격 위치 초기화 </summary>
    public static Transform Init_ShootPos(Enemy enemy, GameObject obj = null)
    {
        if (obj == null) return enemy.transform;
        else return obj.transform;
    }

    /// <summary> Enemy 공격 패턴 지정 </summary>
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

    /// <summary> Enemy 이동 패턴 지정 </summary>
    public static void Init_Moving(Enemy enemy, string name_Moving)
    {
        switch (name_Moving)
        {
            case "str": enemy.moveType = MoveType.Straight; break;
            case "acc": enemy.moveType = MoveType.Accel; break;
            case "slow": enemy.moveType = MoveType.SlowDown; break;
        }
    }





    /*************** 데이터 초기화 모음 ***************/

    /// <summary> 공격 데이터 초기화 </summary>
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

    /// <summary> 이동 데이터 초기화 </summary>
    public static void Init_MoveData(Enemy enemy, ref MoveData data)
    {
        data.transform = enemy.transform;
        data.moveVec = (enemy.moveType == MoveType.SlowDown) ?
             enemy.moveDesVec : enemy.moveVec;
        data.movSpeed = enemy.movSpeed;
        data.fieldTime = enemy.fieldTime;
    }
}
