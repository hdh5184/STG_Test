using static Init_Enemy;
using static Logic_Enemy;

/* <Enemy 소형> */
public class Enemy_Small : Enemy
{
    protected override void Awake()
    {
        base.Awake();
        Init(this, 100, 3);
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
        Explosion(this, "ExplodeB", "EShotL");
        base.Dead();
    }
}
