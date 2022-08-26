using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TopDownZoneManager : ZoneManager
{
    public TopDownZoneManager(Zone[] zones) : base(zones) { }

    public bool IsUnitZoneTopOrDown(Unit unit)
    {
        foreach (Zone zone in zones.Keys)
            foreach (Unit other in zones[zone])
                if (other == unit)
                    if (zone.IsTop || zone.IsDown)
                        return true;

        return false;
    }

    public Zone[] GetUnitZones(Unit unit)
    {
        List<Zone> result = new List<Zone>();

        foreach (Zone zone in zones.Keys)
            foreach (Unit other in zones[zone])
                if (other == unit)
                    result.Add(zone);

        return result.ToArray();
    }

    public bool IsUnitZonesChanged(Unit unit, Zone[] old)
    {
        foreach (Zone zone in zones.Keys)
            if (zones[zone].Contains(unit))
            {
                for (int i = 0; i < old.Length; i++)
                {
                    if (zone == old[i])
                        break;

                    if (i == old.Length - 1)
                        return true;
                }
            }

        return false;
    }

    public Zone GetNearestToUnitZone(Unit unit)
    {
        Zone nearestZone = null;

        foreach (Zone zone in zones.Keys)
            if (zone.IsTop || zone.IsDown)
                if (nearestZone == null || Vector2.Distance(zone.Position, unit.Position) < Vector2.Distance(nearestZone.Position, unit.Position))
                    nearestZone = zone;

        List<Zone> nearests = new List<Zone>();

        if (nearestZone.IsTop)
        {
            foreach (Zone zone in zones.Keys)
                if (zone.IsTop)
                    nearests.Add(zone);
        }
        else
        {
            foreach (Zone zone in zones.Keys)
                if (zone.IsDown)
                    nearests.Add(zone);
        }

        return nearests[Random.Range(0, nearests.Count)];
    }

    public Zone GetOppositeToUnitZone(Unit unit)
    {
        foreach (Zone zone in zones.Keys)
            if (zone.IsTop || zone.IsDown)
                foreach (Unit other in zones[zone])
                    if (other == unit)
                    {
                        List<Zone> opposites = new List<Zone>();

                        if (zone.IsTop)
                        {
                            foreach (Zone down in zones.Keys)
                                if (down.IsDown)
                                    opposites.Add(down);
                        }
                        else
                        if (zone.IsDown)
                        {
                            foreach (Zone top in zones.Keys)
                                if (top.IsTop)
                                    opposites.Add(top);
                        }

                        if (opposites.Count > 0)
                            return opposites[Random.Range(0, opposites.Count)];
                    }

        return null;
    }
}
