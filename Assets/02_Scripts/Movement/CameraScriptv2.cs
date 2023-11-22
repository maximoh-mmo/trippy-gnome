using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraScriptv2 : MonoBehaviour
{
    private Vector3 velocity = Vector3.zero;
    public Vector2 limits = new Vector2(5, 3);
    float smoothTime = .5f;
    [SerializeField] Transform _transform;
    [SerializeField] Terrain _terrain;
    float offset = -5f;
    [SerializeField]float minHeight = 3f;
    float groundHeight;
    Vector3 camCentre = Vector3.zero;

    private void Update()
    {
        groundHeight = GetGroundLevel();
        camCentre = new Vector3(_transform.position.x, groundHeight, _transform.position.z);
        FollowTarget(_transform);
    }

    void LateUpdate()
    {
        Vector3 localPos = transform.localPosition;
        transform.localPosition = new Vector3(Mathf.Clamp(localPos.x, -limits.x, limits.x),Mathf.Clamp(localPos.y, groundHeight + minHeight, groundHeight + minHeight + limits.y), localPos.z);
    }

    public void FollowTarget(Transform t)
    {
        Vector3 localPos = transform.localPosition;
        Vector3 targetLocalPos = t.transform.localPosition;
        transform.localPosition = Vector3.SmoothDamp(localPos, new Vector3(targetLocalPos.x, targetLocalPos.y, targetLocalPos.z + offset), ref velocity, smoothTime);
    }

    public float GetGroundLevel()
    {
        return _terrain.SampleHeight(_transform.position);
    }
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawLine(new Vector3(transform.position.x -limits.x, groundHeight, transform.position.z), new Vector3(transform.position.x + limits.x, groundHeight, transform.position.z));
        Gizmos.DrawLine(new Vector3(transform.position.x -limits.x, groundHeight + 2*limits.y, transform.position.z), new Vector3(transform.position.x  + limits.x, groundHeight + 2*limits.y, transform.position.z));
        Gizmos.DrawLine(new Vector3(transform.position.x -limits.x, groundHeight, transform.position.z), new Vector3(transform.position.x - limits.x, groundHeight + 2*limits.y, transform.position.z));
        Gizmos.DrawLine(new Vector3(transform.position.x +limits.x, groundHeight, transform.position.z), new Vector3(transform.position.x + limits.x, transform.position.y + 2*limits.y, transform.position.z));
    }
}
