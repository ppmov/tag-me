using UnityEngine;

public class ActionDelay<T>
{
    public bool IsReady => timeFromPushing + delayTime < Time.time;

    public T Value
    {
        get
        {
            if (IsReady)
                value = initial;

            return value;
        }
    }

    private readonly float delayTime;
    private readonly T initial;

    private float timeFromPushing = 0f;
    private T value;

    public ActionDelay(float delayTime, T initial)
    {
        this.delayTime = delayTime;
        this.initial = initial;
        value = initial;
    }

    public T Push(T value)
    {
        this.value = value;
        timeFromPushing = Time.time;
        return value;
    }

    public void Stop()
    {
        value = initial;
    }
}
