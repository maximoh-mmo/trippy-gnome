using UnityEngine;
public class DynamicChaseCamera : MonoBehaviour
{
    [SerializeField] Transform target;
    Vector2 limits;
    private Vector3 velocity = Vector3.zero;
    private Vector3 screenBounds = Vector3.zero;
    private bool isShaking;
    private float smoothTime = .5f;
    private float magnitude, duration;
    Vector3 offset;
    public Vector2 Limits => limits;
    
    private void Awake()
    {
        if (Camera.main != null)
            screenBounds =
                Camera.main.ScreenToWorldPoint(new Vector3(Screen.width, Screen.height,
                    Camera.main.transform.position.z));
        offset = transform.position;
        limits = target.GetComponent<CrosshairMovement>().Boundry;
        if (limits.x + screenBounds.x < 0) limits.x = 0;
        else
        {
            limits.x += screenBounds.x;
        }
        if (limits.y + screenBounds.y < 0) limits.y = 0;
        else
        {
            limits.y += screenBounds.y;
        }
    }
    private void Update()
    {
        if (!Application.isPlaying)
        {
            offset = Vector3.zero;
        }
        FollowTarget(target);
    }

    void LateUpdate()
    {
        Vector3 localPos = transform.localPosition;
        transform.localPosition = new Vector3(Mathf.Clamp(localPos.x, -limits.x, limits.x), Mathf.Clamp(localPos.y, -limits.y, limits.y), localPos.z);
        if (isShaking) ShakeCam();
    }

    public void FollowTarget(Transform t)
    {
        Vector3 localPos = transform.localPosition;
        Vector3 targetLocalPos = t.transform.localPosition;
        localPos = Vector3.SmoothDamp(localPos,
            new Vector3(targetLocalPos.x + offset.x, targetLocalPos.y + offset.y, targetLocalPos.z), ref velocity,
            smoothTime);
        localPos = new Vector3(localPos.x, localPos.y, targetLocalPos.z + offset.z);
        transform.localPosition = localPos;
    }

    private void ShakeCam()
    {
        transform.localPosition += Vector3.right * (Random.Range(-1, 2) * magnitude) + Vector3.up * (Random.Range(-1, 2) * magnitude);
        if (Time.time > duration) isShaking = false;
    }
    public void NewShake(float d, float m)
    {
        isShaking = true;
        duration = Time.time + d;
        magnitude = m;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        var positionZ = transform.localPosition.z;
        Gizmos.DrawLine(new Vector3(-limits.x, -limits.y, positionZ), new Vector3(limits.x, -limits.y, positionZ));
        Gizmos.DrawLine(new Vector3(-limits.x, limits.y, positionZ), new Vector3(limits.x, limits.y, positionZ));
        Gizmos.DrawLine(new Vector3(-limits.x, -limits.y, positionZ), new Vector3(-limits.x, limits.y, positionZ));
        Gizmos.DrawLine(new Vector3(limits.x, -limits.y, positionZ), new Vector3(limits.x, limits.y, positionZ));
    }
}
