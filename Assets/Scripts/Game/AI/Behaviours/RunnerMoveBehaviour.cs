using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class RunnerMoveBehaviour : MoveBehaviour
{
    protected Vector2 FromTaggerDirection => -(Tagger.Position - Unit.Position).normalized;

    protected override float PredictionRange => Vector2.Distance(Tagger != null ? Tagger.Position : Unit.Position, Unit.Position) < Setup.UnlockPredictionDistance ? Setup.FloorPredictionRange : Setup.PredictionRange;

    private readonly ActionDelay<Vector2> sharpDelay;
    private readonly ActionDelay<Vector2> inverseDelay;
     
    public RunnerMoveBehaviour(Unit unit, AISettings setup) : base(unit, setup)
    {
        sharpDelay = new ActionDelay<Vector2>(Setup.SharpDecisionDelay, Vector2.zero);
        inverseDelay = new ActionDelay<Vector2>(Setup.ReversalCooldown, Vector2.zero);
    }

    protected Vector2 GetOutOfTaggerDirection()
    {
        float distance = (Tagger.Position - Unit.Position).magnitude;

        if (distance < Setup.StraightRunDistance)
        {
            Debug.DrawLine(Unit.Position, Tagger.Position, Color.black, 0.2f);
            return GetFarthestDirection(FromTaggerDirection);
        }
        else
            return GetFarthestFromTaggerSharpDirection();
    }

    protected Vector2 GetBypassDirectionToTarget(Vector2 targetPosition)
    {
        if (!CanChangeSharpDirection())
            return sharpDelay.Value;

        Vector2 toZone = targetPosition - Unit.Position;
        Vector2 toTagger = Game.Party.Tagger.Position - Unit.Position;
        float angle = Vector2.Angle(toZone, toTagger);

        if (CanMoveForward())
            sharpDelay.Push((targetPosition - Unit.Position).normalized);
        else
        if (IsTargetTooClose())
            return GetOutOfTaggerDirection();
        else
        if (CanBypassInDirection(toZone.normalized - toTagger.normalized))
            sharpDelay.Push((toZone.normalized - toTagger.normalized).normalized);
        else
        if (CanBypassInDirection(toTagger.normalized - toZone.normalized))
            sharpDelay.Push((toTagger.normalized - toZone.normalized).normalized);
        else
            return GetOutOfTaggerDirection();

        Debug.DrawLine(Unit.Position, Unit.Position + sharpDelay.Value * FixedPredictionRange, Color.cyan, 0.2f);
        return sharpDelay.Value;

        bool CanMoveForward() => angle > Setup.BypassAngles.maxValue || toTagger.magnitude * Mathf.Sin(angle * Mathf.Deg2Rad) > Setup.BypassDistance;
        bool IsTargetTooClose() => angle < Setup.BypassAngles.minValue && toTagger.magnitude < Setup.BypassDistance;
        bool CanBypassInDirection(Vector2 bypass) => CanMoveInDirection(bypass, FixedPredictionRange);
    }

    private Vector2 GetFarthestDirection(Vector2 forward, float angleFrom = 0, float angleTo = 360) =>
        SelectFarthestDirectionFromTagger(GetAvailableDirections(forward, angleFrom, angleTo));

    private Vector2 GetFarthestFromTaggerSharpDirection()
    {
        if (!CanChangeSharpDirection())
            return sharpDelay.Value;

        // get forward vector depends on distance to corner
        RangedFloat angles;

        Vector2 forward = IsInCorner()
            ? GetWidestWindowDirection(out angles.minValue, out angles.maxValue)
            : GetSharpMoveDirection(out angles.minValue, out angles.maxValue);

        Vector2 result = GetFarthestDirection(forward, angles.minValue, angles.maxValue);

        // reverse unit if needed and possible
        if (CanInverseDirection(result))
            result = inverseDelay.Value;

        // move
        Debug.DrawLine(Unit.Position, Unit.Position + result * FixedPredictionRange, result == inverseDelay.Value ? Color.red : Color.blue, 0.3f);
        sharpDelay.Push(result);
        return result;
    }

    private bool IsInCorner()
    {
        if (Vector2.Distance(Unit.Position, Game.Party.Tagger.Position) > Setup.CornerTaggerDistance)
            return false;

        if (CanMoveInDirection(Vector2.down, Setup.CornerWallsDistance) && CanMoveInDirection(Vector2.up, Setup.CornerWallsDistance))
            return false;

        if (CanMoveInDirection(Vector2.right, Setup.CornerWallsDistance) && CanMoveInDirection(Vector2.left, Setup.CornerWallsDistance))
            return false;

        return Mathf.Abs(Unit.Position.x) > Mathf.Abs(Game.Party.Tagger.Position.x) &&
               Mathf.Abs(Unit.Position.y) > Mathf.Abs(Game.Party.Tagger.Position.y);
    }

    private Vector2 SelectFarthestDirectionFromTagger(Vector2[] directions)
    {
        if (directions.Length <= 0)
            return Vector2.zero;

        Vector2 target = Game.Party.Tagger.Position + Game.Party.Tagger.LastDirection;
        List<Vector2> filteredDirections = new List<Vector2>();

        if (Unit.State != Unit.Role.Untagable)
            foreach (Vector2 direction in directions)
            {
                if (Unit.LastDirection.magnitude > 0f)
                    if (Setup.JerkAvoidanceFactor < (direction - Unit.LastDirection).magnitude)
                        continue;

                filteredDirections.Add(direction);
            }

        if (filteredDirections.Count == 0)
            filteredDirections = new List<Vector2>(directions);

        Vector2 bestDirection = Vector2.zero;

        foreach (Vector2 direction in filteredDirections)
        {
            if (bestDirection == Vector2.zero)
            {
                if (CanMoveInDirection(direction, FixedPredictionRange))
                    bestDirection = direction;

                continue;
            }

            float distanceToTarget = (Unit.Position + direction * Setup.ComparingDistancesScale - target).magnitude;
            float lastBestDistanceToTarget = (Unit.Position + bestDirection * Setup.ComparingDistancesScale - target).magnitude;

            if (distanceToTarget > lastBestDistanceToTarget)
                if (CanMoveInDirection(direction, FixedPredictionRange))
                    bestDirection = direction;
        }

        return bestDirection.normalized;
    }

    private bool CanMoveSharply(Vector2 forward, out float angleFrom, out float angleTo)
    {
        angleFrom = 0f;
        angleTo = 360f;

        bool canMoveLeft = CanMoveInDirection(Vector2.Perpendicular(forward), FixedPredictionRange);
        bool canMoveRight = CanMoveInDirection(-Vector2.Perpendicular(forward), FixedPredictionRange);

        if (canMoveLeft && sharpDelay.Value.x < 0f)
            canMoveRight = false;
        else
        if (canMoveRight && sharpDelay.Value.x > 0f)
            canMoveLeft = false;

        if (canMoveLeft && canMoveRight)
        {
            if (Random.Range(0, 2) == 0)
                canMoveLeft = false;
            else
                canMoveRight = false;
        }

        if (canMoveLeft)
        {
            angleFrom = Setup.SharpMoveAngles.minValue;//10f;
            angleTo = Setup.SharpMoveAngles.maxValue;//90f;
            return true;
        }
        else
        if (canMoveRight)
        {
            angleFrom = -Setup.SharpMoveAngles.maxValue;//-90f;
            angleTo = -Setup.SharpMoveAngles.minValue;//-10f;
            return true;
        }

        return false;
    }

    private bool CanChangeSharpDirection() => sharpDelay.IsReady || !CanMoveInDirection(sharpDelay.Value, FixedPredictionRange);

    private Vector2 GetSharpMoveDirection(out float angleFrom, out float angleTo)
    {
        CanMoveSharply(FromTaggerDirection, out angleFrom, out angleTo);
        return FromTaggerDirection;
    }

    private Vector2 GetWidestWindowDirection(out float angleFrom, out float angleTo)
    {
        Unit tagger = Game.Party.Tagger;
        Vector2 horizontal = CanMoveInDirection(Vector2.right, Setup.CornerWallsDistance) ? Vector2.left : Vector2.right;
        Vector2 vertical = CanMoveInDirection(Vector2.up, Setup.CornerWallsDistance) ? Vector2.down : Vector2.up;

        RaycastHit2D horizontalHit = Physics2D.Raycast(tagger.Position, horizontal, Mathf.Infinity, 1 >> 0);
        RaycastHit2D verticalHit = Physics2D.Raycast(tagger.Position, vertical, Mathf.Infinity, 1 >> 0);

        if (horizontalHit.distance > verticalHit.distance)
        {
            angleTo = Vector2.Angle(tagger.Position - Unit.Position, horizontalHit.point - Unit.Position);
            angleTo /= 2f;
            angleFrom = -angleTo;
            Debug.DrawLine(horizontalHit.point, tagger.Position, Color.yellow, 0.3f);
            return (tagger.Position + horizontalHit.point) / 2f - Unit.Position; //-vertical;
        }
        else
        {
            angleTo = Vector2.Angle(tagger.Position - Unit.Position, verticalHit.point - Unit.Position);
            angleTo /= 2f;
            angleFrom = -angleTo;
            Debug.DrawLine(verticalHit.point, tagger.Position, Color.yellow, 0.3f);
            return (tagger.Position + verticalHit.point) / 2f - Unit.Position; //-horizontal;
        }
    }

    private bool CanInverseDirection(Vector2 nextDirection)
    {
        Vector2 taggerPosition = Game.Party.Tagger.Position;

        if (!inverseDelay.IsReady)
        {
            if (inverseDelay.Value == Vector2.zero)
                return false;

            if (IsInAllowedAngles(inverseDelay.Value))
                if (CanMoveInDirection(inverseDelay.Value, FixedPredictionRange))
                    return true;

            inverseDelay.Stop();
            return false;
        }

        if (Vector2.Distance(nextDirection, Unit.LastDirection) > Setup.ReversalStartError)
            return false;

        Vector2 nearest = GetNearestToTargetAxis();

        if (TryPushAxisDirection(Vector2.up))
            return true;

        if (TryPushAxisDirection(Vector2.down))
            return true;

        if (TryPushAxisDirection(Vector2.right))
            return true;

        if (TryPushAxisDirection(Vector2.left))
            return true;

        return false;

        bool TryPushAxisDirection(Vector2 axis)
        {
            if (axis != nearest)
                if (Vector2.Angle(-axis, Unit.LastDirection) < Setup.ReversalAngleError)
                    if (IsInAllowedAngles(axis))
                        if (CanMoveInDirection(axis, FixedPredictionRange))
                            if (!CanMoveInDirection(Vector2.Perpendicular(axis), Setup.FloorPredictionRange) ||
                                !CanMoveInDirection(-Vector2.Perpendicular(axis), Setup.FloorPredictionRange))
                            {
                                inverseDelay.Push(axis);
                                return true;
                            }

            return false;
        }

        bool IsInAllowedAngles(Vector2 axis)
        {
            if (Vector2.Angle(-axis, taggerPosition - Unit.Position) < Setup.ReversalMaxAngleToTarget)
                if (!Physics2D.Raycast(taggerPosition, Vector2.Perpendicular(axis), Setup.ReversalWindowWidth, 1 >> 0) &&
                    !Physics2D.Raycast(taggerPosition, -Vector2.Perpendicular(axis), Setup.ReversalWindowWidth, 1 >> 0))
                    return true;

            return false;
        }

        Vector2 GetNearestToTargetAxis()
        {
            Vector2 nearestAxis = Vector2.up;
            Vector2[] axis = new Vector2[4];
            axis[0] = Vector2.up;
            axis[1] = Vector2.down;
            axis[2] = Vector2.right;
            axis[3] = Vector2.left;

            foreach (Vector2 ax in axis)
                if (Vector2.Distance(ax, taggerPosition) < Vector2.Distance(nearestAxis, taggerPosition))
                    nearestAxis = ax;

            return nearestAxis;
        }
    }
}

//private bool isStopped = false;
//private float stopDuration = 0f;
//private IEnumerator SuddenFreeze()
//{
//    isStopped = true;
//    stopDuration = 0f;
//    float targetDuration = Setup.SuddenStopDuration;

//    while (stopDuration < targetDuration)
//    {
//        yield return new WaitForFixedUpdate();
//        stopDuration += Time.fixedDeltaTime;
//    }

//    isStopped = false;
//}

//protected bool IsMyUnitClosestToTagger()
//{
//    Unit tagger = Game.Party.Tagger;
//    float myDistance = (tagger.Position - Unit.Position).magnitude;

//    foreach (Unit runner in Game.Party.Runners)
//        if (runner != Unit)
//            if (myDistance >= (tagger.Position - runner.Position).magnitude)
//                return false;

//    return true;
//}

//protected bool IsMyUnitInCrowd(out Vector2 crowdCenterPosition)
//{
//    int count = 1;
//    List<Vector2> positions = new List<Vector2>();
//    positions.Add(Unit.Position);

//    foreach (Unit runner in Game.Party.Runners)
//    {
//        if (runner == Unit)
//            continue;

//        if ((runner.Position - Unit.Position).magnitude <= Setup.OvercrowdDistance)
//        {
//            positions.Add(runner.Position);
//            count++;
//        }
//    }

//    crowdCenterPosition = Vector2.zero;

//    foreach (Vector2 position in positions)
//    {
//        crowdCenterPosition.x += position.x;
//        crowdCenterPosition.y += position.y;
//    }

//    crowdCenterPosition /= count;
//    return count >= Setup.OvercrowdCount;
//}
//protected Vector2 GetFarthestFromTaggerLimitedDirection(Vector2 direction, float angleFrom, float angleTo)
//{
//    Unit tagger = Game.Party.Tagger;
//    Vector2[] directions = GetAvailableDirections(FixedPredictionRange, direction, angleFrom, angleTo);

//    Vector2 result = SelectFarthestDirectionFromTarget(tagger.Position + tagger.LastDirection, directions);
//    return result;
//}
