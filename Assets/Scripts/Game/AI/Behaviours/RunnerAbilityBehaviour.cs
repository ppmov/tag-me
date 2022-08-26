using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RunnerAbilityBehaviour : AbilityBehaviour
{
    public RunnerAbilityBehaviour(Unit unit, AISettings setup) : base(unit, setup) { }

    protected override void Use()
    {
        if (Tagger == null)
            return;

        if (Tagger.Abilities[Unit.Bonus.Invisible].IsHandling)
            return;

        TryUseBoostOrDash((Tagger.Position - Unit.Position).magnitude);
    }
}
