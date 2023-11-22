using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class MoveForward : MonoBehaviour
{
    [SerializeField] Terrain terrain; 
    [SerializeField] float speed=10f;
    [SerializeField] float minHeight = 3f;
    [SerializeField] Vector2 cameraLimits = new Vector2(5, 3);

    Vector3 aimPoint = Vector3.zero;
    WayPoint wayPoint = null;
    int wayPointNumber = 0;
    public float GetMinLevel(Vector3 position)
    {
        return terrain.SampleHeight(position) + minHeight;
    }
    public float GetMaxLevel(Vector3 position)
    {
        return terrain.SampleHeight(position) + minHeight + cameraLimits.y * 2;
    }
    public float GetCameraMidPoint(Vector3 position)
    {
        return GetMinLevel(position) + GetMaxLevel(position) / 2;
    }
    public void SetAimPoint(int wpNum) { 
        aimPoint = wayPoint.GetItem(wpNum); 
        aimPoint.y = 0;
    }
    private void Start()
    {
        wayPoint = GetComponent<WayPoint>();
        SetAimPoint(wayPointNumber);
        transform.position = new Vector3(aimPoint.x, GetCameraMidPoint(aimPoint), aimPoint.z);
        transform.LookAt(new Vector3(aimPoint.x, GetCameraMidPoint(aimPoint), aimPoint.z));
    }
    void Update()
    {
        if (transform.position.z > aimPoint.z) {
            wayPointNumber++; SetAimPoint(wayPointNumber);
            transform.LookAt(new Vector3(aimPoint.x, GetCameraMidPoint(aimPoint), aimPoint.z));
        }
        transform.position += transform.forward * Time.deltaTime * speed;
        transform.position = (new Vector3(transform.position.x, Mathf.Clamp(transform.position.y, GetMinLevel(transform.position), GetMaxLevel(transform.position)), transform.position.z));
    }
}
