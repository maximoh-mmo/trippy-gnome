using UnityEngine;

public class CameraScriptv2 : MonoBehaviour
{
    [SerializeField] Transform _transform;
    [SerializeField] float smoothTime = 1f;
    Vector3 velocity = Vector3.zero;
    [SerializeField]float distanceBehindFactor = 1f;

    private void LateUpdate()
    {
        Vector3 relativePos = _transform.position - transform.position;
        Quaternion rotation = Quaternion.LookRotation(relativePos, Vector3.up);

        transform.rotation = Quaternion.Slerp(transform.rotation, rotation, smoothTime);
        transform.position = Vector3.SmoothDamp(transform.position, (_transform.position - _transform.forward * distanceBehindFactor), ref velocity, smoothTime);
    }
}