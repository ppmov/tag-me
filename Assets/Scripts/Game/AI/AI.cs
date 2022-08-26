using System.Collections;
using UnityEngine;

public class AI : MonoBehaviour
{
    public Unit Unit { get; private set; }
    public AISettings Setup { get; set; }

    private TaggerCollider taggerCollider;

    private MoveBehaviour moveBehaviour;
    private AbilityBehaviour abilityBehaviour;

    private int fixedUpdateTick = 0;
    private int fixedUpdateCount = 1;
    private Vector2 moveDirection = Vector2.zero;

    private bool isStopped = false;
    private float stopDuration = 0f;

    private void Start()
    {
        Unit = GetComponent<Unit>();
        
        taggerCollider = GetComponentInChildren<TaggerCollider>();
        taggerCollider.OnStateChange.AddListener(OnChangeState);

        Game.Counter.OnPush.AddListener(OnTimerReset);
        ZoneContainer.TopDown.OnEnter += OnEnterZone;
    }

    private void FixedUpdate()
    {
        if (Setup == null)
            return;

        if (Game.Counter.IsFreezed)
            return;

        if (isStopped)
            return;
        
        if (CanUpdateBehaviors())
        {
            moveDirection = moveBehaviour.GetNextDirection();

            if (abilityBehaviour != null)
                abilityBehaviour.Handle();

            if (Setup.HasToStopSuddenly)
                StartCoroutine(WaitSuddenFreeze());
        }

        Unit.MoveTo(MakeSmooth(moveDirection));
    }

    private bool CanUpdateBehaviors()
    {
        if (fixedUpdateTick < fixedUpdateCount)
        {
            fixedUpdateTick++;
            return false;
        }

        fixedUpdateTick = 0;
        fixedUpdateCount = Setup.FixedUpdateCount;
        return true;
    }

    private void OnChangeState()
    {
        moveBehaviour = Unit.State switch
        {
            Unit.Role.Tagger => new TaggerMovebehaviour(Unit, Setup),
            Unit.Role.Untagable => new RoundedMoveBehaviour(Unit, Setup),
            _ => GetRunnerBehaviour()
        };

        if (Settings.IsBonusesEnabled)
        {
            abilityBehaviour = Unit.State switch
            {
                Unit.Role.Tagger => new TaggerAbilityBehaviour(Unit, Setup),
                Unit.Role.Runner => new RunnerAbilityBehaviour(Unit, Setup),
                _ => null
            };
        }

        MoveBehaviour GetRunnerBehaviour()
        {
            if (Game.Counter.TimeLeft(Counter.Type.Eliminate) <= Setup.SwitchStageTime)
                return new RoundedMoveBehaviour(Unit, Setup);
            else
                return new TopDownMoveBehaviour(Unit, Setup);
        }
    }

    private Vector2 MakeSmooth(Vector2 direction)
    {
        if (Unit.LastDirection == Vector2.zero)
            return direction;

        if ((direction - Unit.LastDirection).magnitude > 2f)
            throw new UnityException(direction + " " + Unit.LastDirection);

        if (Setup.SmoothSpeed >= (direction - Unit.LastDirection).magnitude)
            return direction;

        float scale = Setup.SmoothSpeed / (direction - Unit.LastDirection).magnitude;
        return Unit.LastDirection + (direction - Unit.LastDirection) * scale;
    }

    private IEnumerator WaitSuddenFreeze()
    {
        isStopped = true;
        stopDuration = 0f;
        float targetDuration = Setup.SuddenStopDuration;

        while (stopDuration < targetDuration)
        {
            yield return new WaitForFixedUpdate();
            stopDuration += Time.fixedDeltaTime;
        }

        isStopped = false;
    }

    private void OnEnterZone(Unit entered, Zone zone)
    {
        if (Unit == entered)
            StartCoroutine(WaitZoneVisitedDelay(zone));
    }

    private IEnumerator WaitZoneVisitedDelay(Zone zone)
    {
        yield return new WaitForSeconds(Setup.ZoneVisitedDelayRange.Random);

        if (moveBehaviour is TopDownMoveBehaviour topDown)
            if (ZoneContainer.TopDown.IsRunnerReachedZone(Unit, zone))
                topDown.OnZoneReaching();
    }

    private void OnTimerReset()
    {
        if (Game.Counter.TimerType == Counter.Type.Eliminate)
            StartCoroutine(WaitSwitchStageTime());

        OnChangeState();
    }

    private IEnumerator WaitSwitchStageTime()
    {
        int delay = Game.Counter.TimeLeft(Counter.Type.Eliminate) ?? 0;
        yield return new WaitForSeconds(delay - Setup.SwitchStageTime);

        if (Unit.State == Unit.Role.Runner)
            OnChangeState();
    }
}
