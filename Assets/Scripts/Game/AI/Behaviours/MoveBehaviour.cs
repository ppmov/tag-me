using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class MoveBehaviour : AIBehaviour
{
    public MoveBehaviour(Unit unit, AISettings setup) : base(unit, setup) { }

    public Vector2 GetNextDirection()
    {
        if (Tagger == null)
            return GetIdlingDirection();
        else
            return TryMessUpAngleOfDirection(GetDirection());
    }

    protected abstract Vector2 GetDirection();

    protected bool CanMoveInDirection(Vector2 direction, float distance)
    {
        Vector2 perpendicular = Vector2.Perpendicular(direction).normalized * (Unit.Radius - 0.1f);

        if (Physics2D.Raycast(Unit.Position + perpendicular, direction, distance, 1 >> 0))
            return false;

        if (Physics2D.Raycast(Unit.Position - perpendicular, direction, distance, 1 >> 0))
            return false;

        return true;
    }

    protected Vector2 GetIdlingDirection()
    {
        if (Random.Range(0f, 0.99f) < Setup.SafeSleepChance)
            return Vector2.zero;
        else
            return GetRandomDirection();
    }

    protected Vector2[] GetAvailableDirections(Vector2 forward, float angleFrom = 0f, float angleTo = 360f)
    {
        // define angles by forward direction
        float shiftAngle = Vector2.SignedAngle(Vector2.right, forward);
        angleFrom += shiftAngle;
        angleTo += shiftAngle;

        List<Vector2> directions = new List<Vector2>();

        // find all available directions between angles
        for (float a = angleFrom; a <= angleTo; a += 2 * Mathf.Rad2Deg * Mathf.PI / Setup.RaycastAccuracy)
        {
            Vector2 direction = new Vector2(Mathf.Cos(a * Mathf.Deg2Rad), Mathf.Sin(a * Mathf.Deg2Rad)).normalized;

            if (CanMoveInDirection(direction, FixedPredictionRange))
                directions.Add(direction);
        }

        return directions.ToArray();
    }

    private Vector2 GetRandomDirection()
    {
        //if (!CanChangeSharpDirection())
        //    return sharpDelay.Value;

        Vector2[] directions = GetAvailableDirections(Vector2.up);

        if (directions.Length > 0)
            return directions[Random.Range(0, directions.Length)].normalized * Random.Range(0f, 1f); // sharpDelay.Push(

        return Vector2.zero;
    }

    private Vector2 TryMessUpAngleOfDirection(Vector2 direction)
    {
        if (!Setup.HasToMessUpAngle)
            return direction;

        float angle = Vector2.SignedAngle(Vector2.right, direction) + Setup.MessingAngle;
        return new Vector2(Mathf.Cos(angle * Mathf.Deg2Rad), Mathf.Sin(angle * Mathf.Deg2Rad)).normalized * direction.magnitude;
    }
}
