using UnityEngine;
using static Bullet;

/* <EnemyBullet 로직 모음> */
public class Logic_Bullet : MonoBehaviour
{
    /*************** 속성 및 매서드 지정 모음 ***************/

    /// <summary> 이동 메서드 반환 </summary>
    public static Set_Move Init_Moving(string name_Moving)
    {
        switch (name_Moving)
        {
            case "acc": return Move_Acc;
            case "str": return Move_Str;
            case "hom": return Move_Hom;

            default:    return Move_None;
        }
    }

    /// <summary> Bullet 초기 속성 지정 </summary>
    public static void Set_Attribute(Bullet bullet, string bulletType, float speed)
    {
        bullet.getBulletType = bulletType;
        bullet.speed = speed;
    }





    /*************** 이동 패턴 모음 ***************/

    /// <summary> acc : 가속 이동 </summary>
    public static void Move_Acc(ref MoveData data)
    {
        data.transform.Translate
        (data.moveVec * Time.deltaTime * data.fieldTime);
    }

    /// <summary> str : 직진 이동 </summary>
    public static void Move_Str(ref MoveData data)
    {
        data.transform.Translate
        (data.moveVec * Time.deltaTime);
    }

    /// <summary> hom : 위치 추적 이동 </summary>
    public static void Move_Hom(ref MoveData data)
    {
        if (data.targetPos == null)
        {
            data.transform.Translate(data.moveVec * Time.deltaTime);
            return;
        }

        Vector2 v1, v2, v3;
        Vector2 dotVec = data.shootVec;

        float radian =
        (data.isEnemyBullet) ? Mathf.PI / 360 : Mathf.PI / 90;

        int degTemp = (data.isEnemyBullet) ? 90 : -90;

        v1 = (data.targetPos - data.transform.position).normalized;

        v2 = new Vector2(
            dotVec.x * Mathf.Cos(radian) - dotVec.y * Mathf.Sin(radian),
            dotVec.x * Mathf.Sin(radian) + dotVec.y * Mathf.Cos(radian));

        if (Vector2.Dot(dotVec, v1) >= Vector2.Dot(dotVec, v2))
        {
            data.degreeZ = Mathf.Atan2(v1.y, v1.x) / Mathf.PI * 180 + degTemp;
            data.shootVec = v1;
        }
        else
        {
            v3 = new Vector2(
                dotVec.x * Mathf.Cos(radian) + dotVec.y * Mathf.Sin(radian),
                -dotVec.x * Mathf.Sin(radian) + dotVec.y * Mathf.Cos(radian));

            Vector3 pv = data.targetPos - data.transform.position;

            if (Vector2.Dot(pv, v2) >= Vector2.Dot(pv, v3))
            {
                data.degreeZ = Mathf.Atan2(v2.y, v2.x) / Mathf.PI * 180 + degTemp;
                data.shootVec = v2;
            }
            else
            {
                data.degreeZ = Mathf.Atan2(v3.y, v3.x) / Mathf.PI * 180 + degTemp;
                data.shootVec = v3;
            }
        }

        data.transform.rotation = Quaternion.Euler(0, 0, data.degreeZ);
        data.transform.Translate(data.moveVec * Time.deltaTime);
    }

    /// <summary> none : 이동 없음 </summary>
    public static void Move_None(ref MoveData data) { }
}
