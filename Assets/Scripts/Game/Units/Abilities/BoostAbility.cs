public class BoostAbility : Ability
{
    public BoostAbility(Unit parent) : base(parent) => FxPrefab = Settings.BoostFx;

    public float IncreasedSpeed { get; private set; } = 0f;

    protected override void Enable()
    {
        Parent.SpeedVariable = Settings.BoostSpeed * BattlePass.BoostMultiplier;
    }

    protected override void Disable()
    {
        Parent.SpeedVariable = 0f;
    }
}
