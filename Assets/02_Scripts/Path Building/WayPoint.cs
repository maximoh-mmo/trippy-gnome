using System.Collections.Generic;
using UnityEngine;

public class WayPoint : MonoBehaviour
{
    public List<Vector3> waypoints = new List<Vector3>();
    public int Count { get { return waypoints.Count; } }
    public Vector3 GetItem(int i) { return waypoints[i]; }
    public void SetItem(int i, Vector3 value) { waypoints[i] = value; }
    public Vector3[] ToArray() { return waypoints.ToArray(); }
}