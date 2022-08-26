using UnityEngine;

public class Player
{
    public string Name { get; private set; }
    public Sprite Skin { get; private set; }
    public Unit Unit { get; private set; }
    public bool IsPlayer { get; private set; }
    public BattlePass BattlePass { get; private set; }
    public bool IsReady => BattlePass != null;

    private readonly AISettings setup;

    public Player(string name, Sprite skin)
    {
        Name = name;
        IsPlayer = true;
        Skin = skin;
    }

    public Player(string name, AISettings setup)
    {
        Name = name;
        IsPlayer = false;
        this.setup = setup;
        Skin = setup.Skin;
    }

    public void SelectBattlePass(BattlePass pass) => BattlePass = pass;

    public void AssignUnit(Unit unit)
    {
        if (Unit != null)
            return;

        Unit = unit;
        Unit.Construct();

        if (!IsPlayer)
            Unit.gameObject.AddComponent<AI>().Setup = setup;
    }
}
