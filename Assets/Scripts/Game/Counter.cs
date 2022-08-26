using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class Counter : MonoBehaviour
{
    [System.Serializable]
    public struct Timer
    {
        public Type type;
        public int initial;
        public int allowableReduction;
        public Text uiObject;
    }

    public enum Type { None, Ready, Steady, Eliminate, Pause, Finish }
    public delegate void AfterTimerAction();

    public UnityEvent OnPush;
    public Type TimerType => timer.type;
    public bool IsFreezed => TimerType != Type.Eliminate;

    public int? TimeLeft(Type type)
    {
        if (type != TimerType)
            return null;

        return initial - (int)Mathf.Floor(timeSpent);
    }

    public bool IsRunning
    {
        get => TimerType != Type.None;

        private set
        {
            timeSpent = 0;
            initial = value ? (timer.initial - Random.Range(0, timer.allowableReduction + 1)) : 0;

            if (timer.uiObject != null)
                timer.uiObject.gameObject.SetActive(value);

            if (!value)
            {
                timer = new Timer();

                AfterTimerAction lastAction = endAction;
                endAction = null;
                lastAction?.Invoke();
            }
        }
    }

    [SerializeField]
    private List<Timer> timers = new List<Timer>();

    private Timer timer = new Timer();
    private float timeSpent = 0;
    private int initial = 0;
    private AfterTimerAction endAction = null;

    public void Push(Type type, AfterTimerAction action = null)
    {
        if (GetTimer(type).type == Type.None)
            return;

        // stop last
        IsRunning = false;
        
        // set references
        timer = GetTimer(type);
        endAction = action;

        // start new
        IsRunning = true;
        OnPush.Invoke();
    }

    private void Update()
    {
        if (!IsRunning)
            return;

        timeSpent += Time.deltaTime;

        if (timer.uiObject != null)
            timer.uiObject.text = TimeLeft(TimerType).ToString();

        if (timeSpent >= initial)
            IsRunning = false;
    }

    private Timer GetTimer(Type type)
    {
        foreach (Timer timer in timers)
            if (timer.type == type)
                return timer;

        return new Timer();
    }
}
