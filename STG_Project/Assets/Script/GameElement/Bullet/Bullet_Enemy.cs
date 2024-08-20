using UnityEngine;
using static StageManager;
using static Logic_Bullet;

public class Bullet_Enemy : Bullet
{
    private void Awake()
    {
        moveData.shootVec = Vector2.down;
        moveData.isEnemyBullet = true;
    }

    protected override void OnEnable()
    {
        base.OnEnable();
        set_Mov = Init_Moving(getBulletType);
        moveData.shootVec = Vector2.down;

        switch (getBulletType)
        {
            case "str":
            case "acc": moveData.moveVec = Vector2.down * speed; break;
            case "hom": moveData.moveVec = Vector2.down * 5; break;
        }
    }

    protected override void Update()
    {
        if (!isGamePlay) return;
        base.Update();
    }

    protected override void Move()
    {
        moveData.targetPos = playerPos;
        base.Move();
    }
}
