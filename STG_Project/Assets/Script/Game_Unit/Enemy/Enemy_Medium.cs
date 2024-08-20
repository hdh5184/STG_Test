using static Init_Enemy;
using static Logic_Enemy;

/* <Enemy 중소형> */
public class Enemy_Medium : Enemy
{
    protected override void Awake()
    {
        base.Awake();
        Init(this, 500, 30);
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
