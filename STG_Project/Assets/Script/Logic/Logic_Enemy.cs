using UnityEngine;
using static Enemy;
using static Logic_Bullet;

/* <Enemy 로직 모음> */
public class Logic_Enemy : MonoBehaviour
{
    /*************** 메서드 반환 모음 ***************/

    /// <summary> 이동 메서드 반환 </summary>
    public static Set_Move SetSwitch_Move(MoveType move)
    {
        switch (move)
        {
            case MoveType.Accel:    return Move_Acc;
            case MoveType.SlowDown: return Move_Slow;
            case MoveType.Straight: return Move_Str;

            default:                return Move_None;
        }
    }

    /// <summary> 공격 메서드 반환 </summary>
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

            default:                        return Attack_None;
        }
    }





    /*************** 이동 패턴 모음 ***************/

    /// <summary> acc : 가속 이동 </summary>
    public static void Move_Acc(ref MoveData data)
    {
        data.transform.Translate
        (data.moveVec * data.movSpeed * Time.deltaTime * data.fieldTime * 2);
    }

    /// <summary> slow : 지정 위치까지 감쇠 이동 </summary>
    public static void Move_Slow(ref MoveData data)
    {
        data.transform.position =
        Vector2.Lerp(data.transform.position, data.moveVec, 0.07f);
    }

    /// <summary> str : 직진 이동 </summary>
    public static void Move_Str(ref MoveData data)
    {
        data.transform.Translate
        (data.moveVec * data.movSpeed * Time.deltaTime);
    }

    /// <summary> none : 이동 없음 </summary>
    public static void Move_None(ref MoveData data) { }





    /*************** 공격 패턴 모음 ***************/

    /// <summary> str : 플레이어 조준 공격 </summary>
    public static void Attack_Str(ref AttackData data) => Fire(ref data);

    /// <summary> way : n개 탄 방사 공격 </summary>
    public static void Attack_n_Way(ref AttackData data)
    {
        int n_Count = data.shootLimit;
        float degEach = 20f;
        data.degree -= degEach * (n_Count - 1) / 2;

        for (int i = 0; i < n_Count; i++)
        {
            Fire(ref data);
            data.degree += degEach;
        }
    }

    /// <summary> cir : 원형 공격 </summary>
    public static void Attack_Cir(ref AttackData data)
    {
        int n_Count = 20;
        float degEach = 360 / n_Count;

        for (int i = 0; i < n_Count; i++)
        {
            Fire(ref data);
            data.degree += degEach;
        }
    }

    /// <summary> spr : 역삼각형 모양 방사 반복 공격 </summary>
    public static void Attack_Spr(ref AttackData data)
    {
        if (data.shootCount % 4 == 0) Fire(ref data);
        else
        {
            float degOrigin = data.degree;
            float degEach = 10f * (data.shootCount % 4);

            for (int i = 0; i < 2; i++)
            {
                data.degree = (i == 0) ? degOrigin - degEach : degOrigin + degEach;
                Fire(ref data);
            }
        }
    }

    /// <summary> sprR : 플레이어 기준 전방 무작위 각도 방사 공격 </summary>
    public static void Attack_SprR(ref AttackData data)
    {
        int n_Count = 4;
        float degOrigin = data.degree;
        float degLimit = data.degreeLimit / 2;

        for (int i = 0; i < n_Count; i++)
        {
            data.degree = degOrigin + Random.Range(-degLimit, degLimit);
            Fire(ref data);
        }
    }

    public static void Attack_Vtx(ref AttackData data)
    {

    }

    /// <summary> down : 전방 고정 공격 </summary>
    public static void Attack_Down(ref AttackData data)
    {
        data.degree = 0; Fire(ref data);
    }

    /// <summary> none : 공격 없음 </summary>
    public static void Attack_None(ref AttackData data)
    {
        return;
    }





    /*************** 공격 로직 모음 ***************/

    /// <summary> 공격 기준 각도 반환 </summary>
    public static float SetDegree(Vector2 shootPos)
    {
        Vector3 playerPos = StageManager.playerPos;

        Vector2 shootVec = new Vector2(
            playerPos.x - shootPos.x,
            playerPos.y - shootPos.y);

        return Mathf.Atan2(shootVec.y, shootVec.x) / Mathf.PI * 180 + 90;
    }

    /// <summary> 탄 발사 </summary>
    private static void Fire(ref AttackData data)
    {
        // 1. 탄 오브젝트 및 속성 불러오기
        GameObject bullet = data.pool.MakeObject(data.getBulletName, false);
        Bullet bulletCom = bullet.GetComponent<Bullet>();

        // 2. Transform 속성 지정
        Quaternion rotate = Quaternion.Euler(0, 0, data.degree);

        bullet.transform.position = data.firePos;
        bullet.transform.rotation = rotate;

        // 3. 탄 속성 지정 및 초기화
        Set_Attribute(bulletCom, data.getBulletType, data.bulletSpeed);
        //bulletCom.Init();
        bullet.SetActive(true);
    }





    /*************** 기체 파괴 ***************/

    /// <summary> 폭발 이펙트 출현 </summary>
    public static GameObject Explosion(Enemy enemy, string obj, string audio, bool randomExplode = false)
    {
        GameObject explosion = PoolManager.instance.MakeObject(obj, false);

        // 랜덤 위치 폭발 유무 확인
        if (!randomExplode) explosion.transform.position = enemy.transform.position;
        else
        {
            Vector2 Pos = enemy.transform.position;
            Vector2 ExplosionPos = new Vector2(
                Random.Range(Pos.x - 2, Pos.x + 2),
                Random.Range(Pos.y - 1, Pos.y + 1));

            explosion.transform.position = ExplosionPos;
        }

        explosion.GetComponent<Effect>().audio.clip =
            AudioManager.instance.getAudioClip(audio);

        explosion.SetActive(true);

        return explosion;
    }
}
