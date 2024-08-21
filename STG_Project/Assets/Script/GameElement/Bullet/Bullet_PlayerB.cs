using UnityEngine;
using static StageManager;
using static Logic_Bullet;

/* <Bullet 플레이어 B타입 최고 레벨 전용> */
public class Bullet_PlayerB : Bullet
{
    private void Awake()
    {
        moveData.shootVec = Vector2.up;
        moveData.moveVec = Vector2.up * 12f;
        moveData.isEnemyBullet = false;
        power = 4;

        set_Mov = Move_Str;
    }

    protected override void Update()
    {
        if (!isGamePlay) return;
        base.Update();
    }
}