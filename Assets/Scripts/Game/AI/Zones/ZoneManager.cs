using System.Collections.Generic;
using UnityEngine.Events;

public abstract class ZoneManager
{
    public delegate void OnEnterHandler(Unit unit, Zone zone);
    public event OnEnterHandler OnEnter;

    //public UnityEvent<Unit, Zone> OnEnter;
    protected Dictionary<Zone, List<Unit>> zones = new Dictionary<Zone, List<Unit>>();

    public ZoneManager(Zone[] zones)
    {
        foreach (Zone zone in zones)
        {
            this.zones.Add(zone, new List<Unit>());

            zone.OnUnitEnter.AddListener(OnUnitEnter);
            zone.OnUnitExit.AddListener(OnUnitExit);
        }
    }

    public bool IsRunnerReachedZone(Unit runner, Zone zone)
    {
        if (zone == null)
            return true;

        foreach (Unit unit in zones[zone])
            if (unit == runner)
                return true;

        return false;
    }

    public int GetZoneUnitCount(Zone zone) => zones[zone].Count;

    private void OnUnitEnter(Unit unit, Zone zone)
    {
        zones[zone].Add(unit);
        OnEnter?.Invoke(unit, zone);
    }

    private void OnUnitExit(Unit unit, Zone zone) => zones[zone].Remove(unit);
}
