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
    float minHeight = 1f;
    private void Update()
    {
        FollowTarget(_transform);
    }

    void LateUpdate()
    {
        Vector3 localPos = transform.localPosition;
        float groundHeight = GetGroundLevel();
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
        return _terrain.SampleHeight(transform.position);
    }
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawLine(new Vector3(-limits.x, -limits.y, transform.position.z), new Vector3(limits.x, -limits.y, transform.position.z));
        Gizmos.DrawLine(new Vector3(-limits.x, limits.y, transform.position.z), new Vector3(limits.x, limits.y, transform.position.z));
        Gizmos.DrawLine(new Vector3(-limits.x, -limits.y, transform.position.z), new Vector3(-limits.x, limits.y, transform.position.z));
        Gizmos.DrawLine(new Vector3(limits.x, -limits.y, transform.position.z), new Vector3(limits.x, limits.y, transform.position.z));
    }
}
