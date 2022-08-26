using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

public class Game : MonoBehaviour
{
    public static Party Party { get; private set; }
    public static Counter Counter { get; private set; }

    public UnityEvent OnGameLoaded;
    public UnityEvent OnGameStarted;
    public UnityEvent OnMyselfEliminated;
    public UnityEvent<Player> OnGameFinished;

    private void Awake()
    {
        Application.targetFrameRate = 60;
        Counter = GetComponent<Counter>();
        Party = new LocalParty(Settings.AIs);
    }

    private void Start()
    {
        Counter.Push(Counter.Type.Ready, Party.SelectBasicBattlePassForUndecided);
        StartCoroutine(WaitUntilPartyIsReady());
    }

    private IEnumerator WaitUntilPartyIsReady()
    {
        while (Counter.TimerType == Counter.Type.Ready)
        {
            yield return new WaitForFixedUpdate();

            if (Party.Count == 5)
                if (Party.IsEveryoneReady())
                    break;
        }

        OnGameLoaded.Invoke();
        Counter.Push(Counter.Type.Steady, Go);
    }

    private void EliminateTagger()
    {
        Eliminate(Party.Tagger);

        if (Party.Runners.Length == 0)
            Finish(Party.Tagger);
        else
            Pause();
    }

    private void Go()
    {
        OnGameStarted.Invoke();
        Continue();
    }

    private void Eliminate(Unit loser)
    {
        if (Party.Myself?.Unit == loser)
            OnMyselfEliminated.Invoke();

        Destroy(loser.gameObject);

        if (Party.Runners.Length > 0)
            Party.Runners[Random.Range(0, Party.Runners.Length)].State = Unit.Role.Tagger;
    }

    private void Finish(Unit winner)
    {
        OnGameFinished.Invoke(Party[winner]);
        Destroy(winner.gameObject);
        Counter.Push(Counter.Type.Finish, Reload);
    }

    private void Pause() => Counter.Push(Counter.Type.Pause, Continue);

    private void Continue() => Counter.Push(Counter.Type.Eliminate, EliminateTagger);

    private void Reload() => SceneManager.LoadScene(0);
}
