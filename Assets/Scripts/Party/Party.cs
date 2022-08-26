using System.Collections.Generic;

public abstract class Party
{
    protected List<Player> players = new List<Player>();

    public abstract Player Myself { get; }
    public abstract void SelectBasicBattlePassForUndecided();

    public int Count => players.Count;
    public Player this[int index] => players[index];
    public Player this[Unit unit]
    {
        get
        {
            foreach (Player player in players)
                if (player.Unit == unit)
                    return player;

            return null;
        }
    }

    public bool IsEveryoneReady()
    {
        foreach (Player player in players)
            if (!player.IsReady)
                return false;

        return true;
    }

    public Unit Tagger
    {
        get
        {
            if (tagger != null)
                if (tagger.State == Unit.Role.Tagger)
                    return tagger;

            foreach (Player player in players)
                if (player.Unit != null)
                    if (player.Unit.State == Unit.Role.Tagger)
                        return tagger = player.Unit;

            return tagger = null;
        }
    } 
    private Unit tagger = null;

    public Unit[] Runners
    {
        get
        {
            List<Unit> runners = new List<Unit>();
            int i = 0;

            foreach (Player player in players)
                if (player.Unit != null)
                    if (player.Unit.State != Unit.Role.Tagger)
                    {
                        runners.Add(player.Unit);
                        i++;
                    }

            return runners.ToArray();
        }
    }
}
