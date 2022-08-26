using UnityEngine;

public class UnitContainer : MonoBehaviour
{
    [SerializeField]
    private GameObject unitPrefab;
    [SerializeField]
    private Vector2 initialMaxPosition;

    public void InstantiateUnits()
    {
        for (int i = 0; i < Game.Party.Count; i++)
        {
            Player player = Game.Party[i];
            Unit unit = Instantiate(unitPrefab, GetInitialPosition(i), Quaternion.identity, transform).GetComponent<Unit>();
            player.AssignUnit(unit);
            unit.State = i == 0 ? Unit.Role.Tagger : Unit.Role.Runner;
        }
    }

    private Vector2 GetInitialPosition(int index)
    {
        return index switch
        {
            1 => initialMaxPosition,
            2 => initialMaxPosition * new Vector2(-1, 1),
            3 => initialMaxPosition * new Vector2(1, -1),
            4 => initialMaxPosition * -1,
            _ => Vector2.zero,
        };
    }
}
