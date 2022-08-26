using UnityEngine;

public class TopDownMoveBehaviour : RunnerMoveBehaviour
{
    private TopDownZoneManager ZoneManager { get => ZoneContainer.TopDown; }
    private Zone targetZone = null;
    private AISettings.Distance lastStage;
    
    public TopDownMoveBehaviour(Unit unit, AISettings setup) : base(unit, setup) { }

    protected override Vector2 GetDirection()
    {
        AISettings.Distance newStage = Setup.GetDistanceType((Tagger.Position - Unit.Position).magnitude);

        if (lastStage != newStage)
            targetZone = null;

        lastStage = newStage;

        switch (newStage)
        {
            case AISettings.Distance.Critical:
                return GetOutOfTaggerDirection();

            case AISettings.Distance.Dangerous:
                if (targetZone != null)
                    return GetBypassDirectionToTarget(targetZone.RandomPositionInCircle);
                else
                    return GetOutOfTaggerDirection();

            case AISettings.Distance.Attention:
                return GetNearestZoneDirection();

            default:
                if (ZoneManager.IsUnitZoneTopOrDown(Unit))
                    return GetIdlingDirection();
                else
                    return GetNearestZoneDirection();
        }
    }

    public void OnZoneReaching()
    {
        if (targetZone != null)
            if (ZoneManager.IsRunnerReachedZone(Unit, targetZone))
                targetZone = null;

        if (lastStage != AISettings.Distance.Dangerous)
            return;

        if (Setup.CanBypassAtDangerousDistance)
            targetZone = ZoneManager.GetOppositeToUnitZone(Unit);
        else
            targetZone = null;
        //    targetZone = ZoneManager.GetNearestToUnitZone(Unit);
    }

    private Vector2 GetNearestZoneDirection()
    {
        if (targetZone == null)
            targetZone = ZoneManager.GetNearestToUnitZone(Unit);

        Debug.DrawLine(Unit.Position, Unit.Position + (targetZone.RandomPositionInCircle - Unit.Position).normalized, Color.gray, 0.3f);
        return (targetZone.RandomPositionInCircle - Unit.Position).normalized;
    }
}
