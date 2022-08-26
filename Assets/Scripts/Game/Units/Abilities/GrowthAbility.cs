using UnityEngine;

public class GrowthAbility : Ability
{
    public GrowthAbility(Unit parent) : base(parent) => FxPrefab = Settings.GrowthFx;

    protected override void Enable()
    {
        Parent.transform.localScale = new Vector3(
            Settings.UnitSize * Settings.GrowthScale * BattlePass.GrowthMultiplier, 
            Settings.UnitSize * Settings.GrowthScale * BattlePass.GrowthMultiplier, 
            1f);
    }

    protected override void Disable()
    {
        Parent.transform.localScale = new Vector3(Settings.UnitSize, Settings.UnitSize, 1f);
    }
}
