using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattlePassContainer : MonoBehaviour
{
    private BattlePassElement[] battlePasses;

    private void Start()
    {
        battlePasses = GetComponentsInChildren<BattlePassElement>();
    }

    public void OnPassSelect(BattlePass pass)
    {
        if (Game.Party.Myself == null)
            return;

        if (Game.Party.Myself.IsReady)
            return;

        Game.Party.Myself?.SelectBattlePass(pass);

        foreach (BattlePassElement element in battlePasses)
            if (element.Title != pass.Title)
                element.Interactable = false;
    }
}
