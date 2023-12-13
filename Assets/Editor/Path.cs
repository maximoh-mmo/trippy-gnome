using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(WayPoint))]

public class Path : Editor
{
    WayPoint wayPoints; 
    private const float handleSize = 0.05f;
    private const float pickSize = 0.1f;

    private int selectedIndex = -1;

    void OnEnable()
    {
        wayPoints = target as WayPoint;
    }

    void OnSceneGUI()
    {
        DrawLine();
        for (int i = 0; i < wayPoints.ToArray().Length; i++)
        {
            DrawPoint(i);
        }        
    }
    private Vector3 DrawPoint(int index)
    {
        Vector3 point = wayPoints.GetItem(index);
        float size = HandleUtility.GetHandleSize(point);
        Handles.color = Color.cyan;
        if (Handles.Button(point, Quaternion.identity, size * handleSize, size * pickSize, Handles.DotHandleCap))
        {
            selectedIndex = index;
            Repaint();
        }
        if (selectedIndex == index)
        {
            EditorGUI.BeginChangeCheck();
            point = Handles.DoPositionHandle(point, Quaternion.identity);
            if (EditorGUI.EndChangeCheck())
            {
                Undo.RecordObject(wayPoints, "Moved Waypoint");
                EditorUtility.SetDirty(wayPoints);
                wayPoints.SetItem(index, new Vector3(point.x,0,point.z));
            }
        }
        return point;
    }
    private void DrawLine()
    {
        Handles.color = Color.green;
        Handles.DrawAAPolyLine(3f, wayPoints.ToArray());
    }
}
