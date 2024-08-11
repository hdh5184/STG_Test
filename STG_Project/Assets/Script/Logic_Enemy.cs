using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Enemy;

public class Logic_Enemy : MonoBehaviour
{
    public static Logic_Enemy instance;

    int shootCount;

    // 구조체로 매개변수를 넘겨줄까
    public static void Move_Acc(ref MoveData data)
    {
        data.transform.Translate
            (data.moveVec * data.movSpeed * Time.deltaTime * data.fieldTime * 2);
    }

    public static void Move_Str(ref MoveData data)
    {
        data.transform.Translate
            (data.moveVec * data.movSpeed * Time.deltaTime);
    }

    public static void Move_Slow(ref MoveData data)
    {
        data.transform.position =
            Vector2.Lerp(data.transform.position, data.moveVec, 0.07f);
    }


    /*
    public static void Moving()
    {
        switch (movingType)
        {
            case MovingType.Straight:
                transform.Translate(moveVec * movSpeed * Time.deltaTime); break;
            case MovingType.Accel:
                if (enemyState == EnemyState.Exit)
                    transform.Translate(moveExitVec * movSpeed * Time.deltaTime * fieldTime * 2);
                else transform.Translate(moveVec * movSpeed * Time.deltaTime * fieldTime * 2); break;
            case MovingType.SlowDown:
                if (fieldTime < 1)
                    transform.position = Vector2.Lerp(transform.position, moveDesVec, 0.07f);
                else movingType = MovingType.Straight; break;
        }
    }
    */

    
    public static Set_Attack SetSwitch_Attack(AttackType patten)
    {
        switch (patten)
        {
            case AttackType.Circle:         return Attack_Cir;
            case AttackType.Down:           return Attack_Down;
            case AttackType.n_Way:          return Attack_n_Way;
            case AttackType.Spread:         return Attack_Spr;
            case AttackType.Spread_Random:  return Attack_SprR;
            case AttackType.Straight:       return Attack_Str;
            case AttackType.Vortex:         return Attack_Vtx;

            case AttackType.None:           return Attack_None;

            default:                        return null;
        }
    }

    public static Set_Move SetSwitch_Move(MoveType move)
    {
        switch (move)
        {
            case MoveType.Accel:    return Move_Acc;
            case MoveType.SlowDown: return Move_Slow;
            case MoveType.Straight: return Move_Str;

            default:                return null;
        }
    }




    public static float SetDegree(Vector2 shootPos)
    {
        Vector3 playerPos = StageManager.playerPos;

        Vector2 shootVec = new Vector2(
            playerPos.x - shootPos.x,
            playerPos.y - shootPos.y);

        return Mathf.Atan2(shootVec.y, shootVec.x) / Mathf.PI * 180 + 90;
    }



    /// <summary> str : 플레이어 조준 공격 </summary>
    public static void Attack_Str(ref AttackData data) => Fire(ref data);

    /// <summary> way : n개 탄 방사 공격 </summary>
    public static void Attack_n_Way(ref AttackData data)
    {

        int n_Count = data.shootLimit;
        float degEach = 20f;
        float setDeg = data.degree - degEach * (n_Count - 1) / 2;

        for (int i = 0; i < n_Count; i++)
        {
            Fire(ref data);
            setDeg += degEach;
        }
    }

    /// <summary> cir : 원형 공격 </summary>
    public static void Attack_Cir(ref AttackData data)
    {

        int n_Count = 20;
        float degEach = 360 / n_Count;
        float setDeg = data.degree;

        for (int i = 0; i < n_Count; i++)
        {
            Fire(ref data);
            setDeg += degEach;
        }
    }

    /// <summary> spr : 역삼각형 모양 방사 반복 공격 </summary>
    public static void Attack_Spr(ref AttackData data)
    {
        float degEach = 10f;

        for (int i = 0; i < 2; i++)
        {
            float setDeg = (i == 0) ?
                data.degree - degEach * (data.shootCount % 4) : data.degree + degEach * (data.shootCount % 4);
            Fire(ref data);
            if (data.shootCount % 4 == 0) break;
        }
    }

    /// <summary> sprR : 플레이어 기준 전방 무작위 각도 방사 공격 </summary>
    public static void Attack_SprR(ref AttackData data)
    {
        int n_Count = 4;


        for (int i = 0; i < n_Count; i++)
        {
            float setDeg = data.degree + Random.Range(-data.degreeLimit / 2, data.degreeLimit / 2);
            Fire(ref data);
        }
    }

    public static void Attack_Vtx(ref AttackData data)
    {

    }

    /// <summary> down : 전방 고정 공격 </summary>
    public static void Attack_Down(ref AttackData data) => Fire(ref data);

    public static void Attack_None(ref AttackData data)
    {
        return;
    }



    /// <summary> 탄 발사 로직 </summary>
    private static void Fire(ref AttackData data)
    {
        // 1. 탄 오브젝트 및 속성 불러오기
        GameObject bullet = data.pool.MakeObject(data.getBulletName);
        EnemyBullet bulletCom = bullet.GetComponent<EnemyBullet>();

        // 2. Transform 속성 지정
        Quaternion rotate = Quaternion.Euler(0, 0, data.degree);

        bullet.transform.position = data.firePos;
        bullet.transform.rotation = rotate;

        // 3. 탄 속성 지정 및 초기화
        bulletCom.Set_Attribute(data.getBulletType, data.bulletSpeed);
        bulletCom.Init();
    }
    
}
