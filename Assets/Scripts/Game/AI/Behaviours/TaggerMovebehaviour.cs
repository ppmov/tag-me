using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TaggerMovebehaviour : MoveBehaviour
{
    private Unit Target { get; set; } = null;
    private bool hasEnoughTimePassedFromConstructed = false;
    private bool hasEnoughTimePassedFromInterception = true;
    private float lastAngle;

    public TaggerMovebehaviour(Unit unit, AISettings setup) : base(unit, setup) 
    { 
        Unit.StartCoroutine(Wait4ConstructorDelay()); 
    }

    protected override Vector2 GetDirection()
    {
        if (!hasEnoughTimePassedFromConstructed)
            return Unit.LastDirection;

        SetClosestTarget();

        if (Target == null)
            return Vector2.zero;

        if ((Target.Position - Unit.Position).magnitude > Setup.StopChangingTargetDistance)
            TrySetClosestTargetInCrowd();

        Vector2 direction = (Target.Position - Unit.Position).normalized;
        return TryGetPredictedDirection(direction);
    }

    private void SetClosestTarget()
    {
        Target = GetClosestRunner();

        if (Target == null)
            Target = GetClosestRunner(false);
    }

    private bool TrySetClosestTargetInCrowd()
    {
        List<Unit> crowd = new List<Unit>();

        foreach (Unit runner in Game.Party.Runners)
        {
            if (runner.State == Unit.Role.Untagable)
                continue;

            if (runner.Abilities[Unit.Bonus.Invisible].IsHandling)
                continue;

            int count = 0;

            foreach (Unit other in Game.Party.Runners)
                if (other.State != Unit.Role.Untagable)
                    if ((other.Position - runner.Position).magnitude <= Setup.OvercrowdDistance)
                        count++;

            if (count >= Setup.OvercrowdCount)
                crowd.Add(runner);
        }

        if (crowd.Count == 0)
            return false;

        Unit closestUnit = crowd[0];

        foreach (Unit runner in crowd)
            if ((runner.Position - Unit.Position).magnitude < (closestUnit.Position - Unit.Position).magnitude)
                closestUnit = runner;

        Target = closestUnit;
        return true;
    }

    private Vector2 TryGetPredictedDirection(Vector2 forward)
    {
        if (CanMoveInDirection(forward, Setup.CeilPredictionRange))
            return forward;

        bool canMoveLeft = CanMoveInDirection(Vector2.Perpendicular(forward), FixedPredictionRange);
        bool canMoveRight = CanMoveInDirection(-Vector2.Perpendicular(forward), FixedPredictionRange);

        if (canMoveLeft == canMoveRight)
            return forward;

        if (hasEnoughTimePassedFromInterception)
        {
            lastAngle = Vector2.SignedAngle(Vector2.right, forward);
            lastAngle += Setup.TaggerInterceptionAngle * (canMoveLeft ? 1 : -1);
            Unit.StartCoroutine(Wait4InterceptionDelay());
        }

        return new Vector2(Mathf.Cos(lastAngle * Mathf.Deg2Rad), Mathf.Sin(lastAngle * Mathf.Deg2Rad)).normalized;
    }

    private IEnumerator Wait4ConstructorDelay()
    {
        hasEnoughTimePassedFromConstructed = false;
        yield return new WaitForSeconds(Setup.BecomeTaggerDelay);
        hasEnoughTimePassedFromConstructed = true;
    }

    private IEnumerator Wait4InterceptionDelay()
    {
        hasEnoughTimePassedFromInterception = false;
        yield return new WaitForSeconds(Setup.InterceptionDelay);
        hasEnoughTimePassedFromInterception = true;
    }
}
