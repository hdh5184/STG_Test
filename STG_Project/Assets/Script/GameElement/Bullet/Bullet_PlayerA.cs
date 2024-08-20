using UnityEngine;
using static StageManager;
using static Logic_Bullet;

public class Bullet_PlayerA : Bullet
{
    private void Awake()
    {
        moveData.shootVec = Vector2.up;
        moveData.moveVec = Vector2.up * 15f;
        moveData.isEnemyBullet = false;
        power = 8;

        set_Mov = Move_Acc;
    }

    protected override void Update()
    {
        if (!isGamePlay) return;
        base.Update();
    }
}
