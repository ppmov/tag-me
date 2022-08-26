using UnityEngine;

public class RoundedMoveBehaviour : RunnerMoveBehaviour
{
    private Zone targetZone = null;
    private RoundedZoneManager ZoneManager { get => ZoneContainer.Rounded; }

    public RoundedMoveBehaviour(Unit unit, AISettings setup) : base(unit, setup) { }

    protected override Vector2 GetDirection()
    {
        if (Tagger.Abilities[Unit.Bonus.Invisible].IsHandling)
            return GetIdlingDirection();

        if ((Tagger.Position - Unit.Position).magnitude <= Setup.SharpRunDistance)
            return GetOutOfTaggerDirection();
        else
        {
            if (HasToSelectZone())
                SelectZone();

            if (!ZoneManager.IsRunnerReachedZone(Unit, targetZone))
                return GetTargetZoneDirection();
            else
                return GetIdlingDirection();
        }

        //        default:
        //            if (IsMyUnitClosestToTagger() || !IsMyUnitInCrowd(out _))
        //                return GetFarthestFromTaggerDirection();
        //            else
        //                return GetFarthestFromTaggerSharpDirection();
        //    }
        //}
    }

    private bool HasToSelectZone()
    {
        if (targetZone == null)
            return true;

        if (!IsTargetZoneSafe())
            return true;

        if (IsRunnerZoneSafe())
            if (!ZoneManager.IsFarthestFromTagger(targetZone) || ZoneManager.GetZoneUnitCount(targetZone) >= Setup.OvercrowdCount) // drop last cond
                return true;

        return false;
    }

    private void SelectZone()
    {
        if (IsRunnerZoneSafe())
            targetZone = ZoneManager.GetFarthestFromTaggerFreeZoneIfPossible(Setup.OvercrowdCount); // GetFarthestFromTaggerZone
        else
            targetZone = ZoneManager.GetNearestSafeZone(Unit.Position);

        if (targetZone == null)
            return;

        if (IsRunnerZoneSafe())
            Debug.DrawLine(Unit.Position, targetZone.Position, Color.white, 0.5f);
        else
            Debug.DrawLine(Unit.Position, targetZone.Position, Color.yellow, 0.5f);
    }

    private bool IsRunnerZoneSafe() => ZoneManager.IsRunnerZoneSafe(Unit);
    private bool IsTargetZoneSafe() => ZoneManager.IsZoneSafe(targetZone);
    private Vector2 GetTargetZoneDirection() => (targetZone.RandomPositionInCircle - Unit.Position).normalized;
}
