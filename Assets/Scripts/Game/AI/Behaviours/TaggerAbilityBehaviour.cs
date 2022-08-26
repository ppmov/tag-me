using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TaggerAbilityBehaviour : AbilityBehaviour
{
    public TaggerAbilityBehaviour(Unit unit, AISettings setup) : base(unit, setup) { }

    protected override void Use()
    {
        Unit target = GetClosestRunner();

        if (target == null)
            return;

        float distanceToTarget = (target.Position - Unit.Position).magnitude;

        if (TryUseBoostOrDash(distanceToTarget))
            Unit.Abilities[Unit.Bonus.Growth].TryUse();
        else
        if (distanceToTarget <= Setup.GrowthDistance)
            Unit.Abilities[Unit.Bonus.Growth].TryUse();

        if (Setup.LastChanceInvisibilityTime > Game.Counter.TimeLeft(Counter.Type.Eliminate))
            Unit.Abilities[Unit.Bonus.Invisible].TryUse();
    }
}
