using UnityEngine;
using static StageManager;
using static Logic_Bullet;

/* <Bullet 플레이어 전용> */
public class Bullet_PlayerDefault : Bullet
{
    public enum PBullet_Lv { Lv1 = 3, Lv2 = 4, Lv3 = 5 };
    public PBullet_Lv pBullet_Lv;

    private void Awake()
    {
        moveData.shootVec = Vector2.up;
        moveData.moveVec = Vector2.up * 12f;
        moveData.isEnemyBullet = false;
        power = (byte)pBullet_Lv;

        set_Mov = Move_Str;
    }

    protected override void Update()
    {
        if (!isGamePlay) return;
        base.Update();
    }
}
