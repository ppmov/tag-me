using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LocalParty : Party
{
    public override Player Myself => playerIndex == -1 ? null : players[playerIndex];

    private int playerIndex = -1;
    private readonly AISettings[] aiSettings;

    public LocalParty(AISettings[] ais)
    {
        if (ais == null || ais.Length == 0)
            throw new UnityException("no ai settings");

        aiSettings = ais;
        int count = 5;
        List<string> names = AINamesGenerator.Utils.GetRandomNames(5);

        players = new List<Player>(count);
        playerIndex = Settings.HasAIOnly ? -1 : Random.Range(0, count);

        for (int i = 0; i < count; i++)
        {
            if (playerIndex == i)
                players.Add(new Player("myself", Settings.PlayerSkin));
            else
            {
                AISettings setup = aiSettings[Random.Range(0, aiSettings.Length)];
                players.Add(new Player(names[i], setup));
            }
        }
    }

    public Player AddPlayer()
    {
        if (players.Count == 5)
            return null;

        AISettings setup = aiSettings[Random.Range(0, aiSettings.Length)];
        players.Add(new Player(AINamesGenerator.Utils.GetRandomName(), setup));
        return players[Count - 1];
    }

    private void CreateRoom()
    {
        playerIndex = -1;
        int count = Random.Range(0, Settings.HasAIOnly ? 6 : 5);
        List<string> names = AINamesGenerator.Utils.GetRandomNames(count);

        if (!Settings.HasAIOnly)
        {
            playerIndex = count;
            count++;
        }

        players = new List<Player>(count);

        for (int i = 0; i < count; i++)
        {
            if (playerIndex == i)
                players.Add(new Player("myself", Settings.PlayerSkin));
            else
            {
                AISettings setup = aiSettings[Random.Range(0, aiSettings.Length)];
                players.Add(new Player(names[i], setup));
            }
        }
    }

    public override void SelectBasicBattlePassForUndecided()
    {
        BattlePass pass = Resources.Load<BattlePass>("Setup/Battlepass/Normal");

        foreach (Player player in players)
            if (!player.IsReady)
                player.SelectBattlePass(pass);
    }
}