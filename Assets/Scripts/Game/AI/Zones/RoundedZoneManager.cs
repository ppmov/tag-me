using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoundedZoneManager : ZoneManager
{
    public RoundedZoneManager(Zone[] zones) : base(zones) { }

    public bool IsZoneSafe(Zone zone, int range = 1)
    {
        if (zone == null)
            return false;

        foreach (Zone other in GetSafeZones(range))
            if (other == zone)
                return true;

        return false;
    }

    public bool IsRunnerZoneSafe(Unit unit, int range = 1)
    {
        Zone[] taggers = GetTaggerZones();

        foreach (Zone zone in zones.Keys)
            foreach (Unit other in zones[zone])
                if (other == unit)
                    foreach (Zone tagg in taggers)
                        if (IsNeighbours(zone, tagg, range))
                            return false;

        return true;
    }

    public bool IsFarthestFromTagger(Zone zone)
    {
        Zone[] taggers = GetTaggerZones();

        foreach (Zone tagg in taggers)
            if (!tagg.IsZoneFarthest(zone))
                return false;

        return true;
    }

    public Zone GetFarthestFromTaggerZone()
    {
        Zone[] result = GetFarthestFromTaggerZones();

        if (result.Length == 0)
            return null;

        return result[Random.Range(0, result.Length)];
    }

    public Zone GetFarthestFromTaggerFreeZoneIfPossible(int overcrowdCount)
    {
        Zone[] result = GetFarthestFromTaggerZones();

        if (result.Length == 0)
            return null;

        List<Zone> sorted = new List<Zone>();

        foreach (Zone zone in result)
            if (GetZoneUnitCount(zone) < overcrowdCount)
                sorted.Add(zone);

        if (sorted.Count == 0)
            return result[Random.Range(0, result.Length)];

        return sorted[Random.Range(0, sorted.Count)];
    }

    private Zone[] GetFarthestFromTaggerZones()
    {
        Zone[] taggers = GetTaggerZones();
        List<Zone> result = new List<Zone>();

        foreach (Zone zone in zones.Keys)
        {
            bool isFarthest = true;

            foreach (Zone tagg in taggers)
                if (!tagg.IsZoneFarthest(zone))
                {
                    isFarthest = false;
                    break;
                }

            if (isFarthest)
                result.Add(zone);
        }

        return result.ToArray();
    }

    //public Zone GetRandomFreeSafeZone()
    //{
    //    Zone[] safes = GetSafeZones();

    //    if (safes.Length == 0)
    //        return null;

    //    List<Zone> frees = new List<Zone>();

    //    foreach (Zone zone in safes)
    //        if (zones[zone].Count == 0)
    //            frees.Add(zone);

    //    if (frees.Count == 0)
    //        return safes[Random.Range(0, safes.Length)];
    //    else
    //        return frees[Random.Range(0, frees.Count)];
    //}

    public Zone GetNearestSafeZone(Vector2 from, int range = 1)
    {
        Zone[] safes = GetSafeZones(range);

        if (safes.Length == 0)
            return null;

        Zone best = safes[0];

        foreach (Zone zone in safes)
            if ((best.Position - from).magnitude > (zone.Position - from).magnitude)
                best = zone;

        return best;
    }

    private Zone[] GetSafeZones(int range = 1)
    {
        Zone[] taggers = GetTaggerZones();
        List<Zone> safes = new List<Zone>();

        foreach (Zone zone in zones.Keys)
        {
            bool safe = true;

            foreach (Zone tagg in taggers)
                if (IsNeighbours(zone, tagg, range))
                {
                    safe = false;
                    break;
                }

            if (safe)
                safes.Add(zone);
        }

        return safes.ToArray();
    }

    private Zone[] GetTaggerZones()
    {
        List<Zone> taggers = new List<Zone>();

        foreach (Zone zone in zones.Keys)
            foreach (Unit unit in zones[zone])
                if (unit.State == Unit.Role.Tagger)
                    taggers.Add(zone);

        return taggers.ToArray();
    }

    private bool IsNeighbours(Zone first, Zone second, int range = 1)
    {
        if (first == second)
            return true;

        int count = 0;
        RaycastHit2D[] hits = Physics2D.RaycastAll(first.Position, second.Position - first.Position);

        foreach (RaycastHit2D hit in hits)
            if (hit.collider.gameObject.layer == 10)
                if (hit.collider.transform != first.transform)
                {
                    count++;

                    if (hit.collider.transform == second.transform)
                        return true;

                    if (count == range)
                        return false;
                }

        return false;
    }

}
