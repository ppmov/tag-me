public class DashAbility : Ability
{
    public DashAbility(Unit parent) : base(parent) => FxPrefab = Settings.DashFx;

    protected override void Enable()
    {
        Parent.Rigidbody.AddForce(BattlePass.DashMultiplier * Settings.DashForce * Parent.LastDirection.normalized, UnityEngine.ForceMode2D.Impulse);
    }

    protected override void Disable()
    {
        return;
    }
}
