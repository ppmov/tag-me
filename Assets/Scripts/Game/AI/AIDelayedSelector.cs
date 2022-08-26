using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIDelayedSelector : MonoBehaviour
{
    [SerializeField]
    [MinMaxRange(0f, 20f)]
    private RangedFloat enterTime;
    [SerializeField]
    [MinMaxRange(0f, 20f)]
    private RangedFloat selectTime;

    private BattlePass[] passes;

    private void Start()
    {
        passes = Resources.LoadAll<BattlePass>("Setup/Battlepass");

        for (int i = 0; i < Game.Party.Count; i++)
            if (!Game.Party[i].IsPlayer)
                StartCoroutine(WaitAndChoose(Game.Party[i]));

        //WaitAndJoin();
    }

    private IEnumerator WaitAndJoin()
    {
        while (Game.Party.Count < 5)
        {
            yield return new WaitForSeconds(enterTime.Random);
            StartCoroutine(WaitAndChoose(((LocalParty)Game.Party).AddPlayer()));
        }
    }

    private IEnumerator WaitAndChoose(Player bot)
    {
        yield return new WaitForSeconds(selectTime.Random);
        bot.SelectBattlePass(passes[Random.Range(0, passes.Length)]);
    }
}
