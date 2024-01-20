using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class WayPoint : MonoBehaviour
{
    public List<Vector3> waypoints = new List<Vector3>();
    public int lastElement { get { return waypoints.Count-1; } }

    public Vector3 TravelDirection(Transform player)
    {
        var nearest = waypoints
            .OrderBy(t => Vector3.Distance(t, player.position))
            .First();
        var nextWp = Vector3.Dot(player.forward, nearest - player.position) > 0 ? nearest : GetItem(GetItem(nearest) + 1);
        var heading = nearest - nextWp != Vector3.zero ? nextWp - nearest : GetItem(GetItem(nextWp) - 1);
        return Vector3.Normalize(heading);
    }

    public Vector3 GetItem(int i)
    {
        if (i > waypoints.Count -1) return Vector3.zero;
        return waypoints[i]; 
    }

    public int GetItem(Vector3 pos)
    {
        for (int i = 0; i < waypoints.Count; i++)
        {
            if (waypoints[i] == pos) return i;
        }
        return -1;
    }
    public void SetItem(int i, Vector3 value) { waypoints[i] = value; }
    public Vector3[] ToArray() { return waypoints.ToArray(); }
    public void Translate(int tileNum)
    {
        for (int i = 0; i < waypoints.Count; i++)
        {
            waypoints[i] = GetItem(i) + (tileNum *new Vector3(0,0,1000));
        }
    }
}