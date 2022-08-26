using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class AbilityBehaviour : AIBehaviour
{
    public AbilityBehaviour(Unit unit, AISettings setup) : base(unit, setup) { }

    public void Handle()
    {
        if (Setup.HasToUseRandomAbility)
            UseRandomAbility();
        else
            Use();
    }

    protected abstract void Use();

    protected bool TryUseBoostOrDash(float distanceToTarget)
    {
        bool canBoost = false;
        bool canDash = false;

        if (!Unit.Abilities[Unit.Bonus.Boost].IsCooling)
            if (!Unit.Abilities[Unit.Bonus.Dash].IsHandling)
                if (distanceToTarget <= Setup.BoostDistance(Unit.State))
                    canBoost = true;

        if (!Unit.Abilities[Unit.Bonus.Dash].IsCooling)
            if (!Unit.Abilities[Unit.Bonus.Boost].IsHandling)
                if (distanceToTarget <= Setup.DashDistance(Unit.State))
                    canDash = true;

        if (canBoost && canDash)
        {
            if (Random.Range(0, 2) == 0)
                canBoost = false;
            else
                canDash = false;
        }

        if (canBoost)
            return Unit.Abilities[Unit.Bonus.Boost].TryUse();

        if (canDash)
            return Unit.Abilities[Unit.Bonus.Dash].TryUse();

        return false;
    }

    private void UseRandomAbility()
    {
        foreach (Unit.Bonus bonus in Unit.Abilities.Keys)
            if (Random.Range(0, 4) == 0)
            {
                Unit.Abilities[bonus].TryUse();
                return;
            }
    }
}
