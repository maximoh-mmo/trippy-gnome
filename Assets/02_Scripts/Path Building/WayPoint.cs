using System.Collections.Generic;
using UnityEngine;

public class WayPoint : MonoBehaviour
{
    public List<Vector3> waypoints = new List<Vector3>();
    public int lastElement { get { return waypoints.Count-1; } }
    public Vector3 GetItem(int i) { return waypoints[i]; }
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