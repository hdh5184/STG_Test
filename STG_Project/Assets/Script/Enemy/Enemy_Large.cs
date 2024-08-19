using static Init_Enemy;
using static Logic_Enemy;

/* <Enemy 중형> */
public class Enemy_Large : Enemy
{
    protected override void Awake()
    {
        base.Awake();
        Init(this, 5000, 120);
    }

    protected override void OnEnable()
    {
        base.OnEnable();
    }

    protected override void Update()
    {
        if (StageManager.isGamePlay)
        base.Update();
    }

    protected override void Dead()
    {
        Explosion(this, "ExplodeC", "Explode");
        base.Dead();
    }
}
