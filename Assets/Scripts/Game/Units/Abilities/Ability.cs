using System.Collections;
using UnityEngine;

public abstract class Ability
{
    protected Unit Parent { get; set; }
    protected GameObject FxPrefab { get; set; }
    protected BattlePass BattlePass { get => Parent.BattlePass; }

    public bool IsCooling { get; protected set; } = false;
    public bool IsHandling { get; protected set; } = false;

    public Ability(Unit parent) => Parent = parent;

    public bool TryUse()
    {
        if (Game.Counter.IsFreezed)
            return false;

        if (!Settings.IsBonusesEnabled)
            return false;

        if (IsCooling)
            return false;

        Parent.StartCoroutine(Handling());
        Parent.StartCoroutine(Cooldown());
        return true;
    }

    protected abstract void Enable();
    protected abstract void Disable();

    private IEnumerator Handling()
    {
        IsHandling = true;
        Enable();

        if (FxPrefab != null)
            Object.Instantiate(FxPrefab, Parent.transform);

        yield return new WaitForSeconds(Settings.AbilityDuration);

        IsHandling = false;
        Disable();
    }

    private IEnumerator Cooldown()
    {
        IsCooling = true;
        yield return new WaitForSeconds(Settings.AbilityCooldown);
        IsCooling = false;
    }
}
