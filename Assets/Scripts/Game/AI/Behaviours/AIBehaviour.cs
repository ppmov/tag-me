using UnityEngine;

public abstract class AIBehaviour
{
    protected Unit Unit { get; set; }
    protected AISettings Setup { get; set; }

    protected virtual float PredictionRange => Setup.PredictionRange;
    protected float FixedPredictionRange
    {
        get
        {
            if (predictionDelay.IsReady)
                predictionDelay.Push(PredictionRange);

            return predictionDelay.Value;
        }
    }

    protected Unit Tagger
    {
        get
        {
            if (Game.Party.Tagger.Abilities[Unit.Bonus.Invisible].IsHandling)
                return null;

            return Game.Party.Tagger;
        }
    }

    private readonly ActionDelay<float> predictionDelay;

    public AIBehaviour(Unit unit, AISettings setup)
    {
        Unit = unit;
        Setup = setup;
        predictionDelay = new ActionDelay<float>(Time.fixedDeltaTime, 0f);
    }

    public Unit GetClosestRunner(bool tagableOnly = true)
    {
        Unit closestUnit = null;

        foreach (Unit runner in Game.Party.Runners)
        {
            if (tagableOnly)
                if (runner.State == Unit.Role.Untagable)
                    continue;

            if (runner.Abilities[Unit.Bonus.Invisible].IsHandling)
                continue;

            if (closestUnit == null)
                closestUnit = runner;

            if ((runner.Position - Unit.Position).magnitude < (closestUnit.Position - Unit.Position).magnitude)
                closestUnit = runner;
        }

        return closestUnit;
    }
}
