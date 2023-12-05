using UnityEditor.IMGUI.Controls;
using UnityEngine;
using UnityEngine.UIElements;

public class CameraScriptv2 : MonoBehaviour
{
    [SerializeField] Transform target;
    Vector2 limits;
    private Vector3 velocity = Vector3.zero;
    private Vector3 screenBounds = Vector3.zero;
    float smoothTime = .5f;
    Vector3 offset;
    public Vector2 Limits { get { return limits; } }
    private void Start()
    {
        screenBounds = Camera.main.ScreenToWorldPoint(new Vector3(Screen.width, Screen.height, Camera.main.transform.position.z));
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
        Vector3 localPos = transform.position;
        transform.position = new Vector3(Mathf.Clamp(localPos.x, -limits.x, limits.x), Mathf.Clamp(localPos.y, -limits.y, limits.y), localPos.z);
    }

    public void FollowTarget(Transform t)
    {
        Vector3 localPos = transform.position;
        Vector3 targetLocalPos = t.transform.position;
        transform.position = Vector3.SmoothDamp(localPos, new Vector3(targetLocalPos.x + offset.x, targetLocalPos.y + offset.y, targetLocalPos.z), ref velocity, smoothTime);
        transform.position = new Vector3(transform.position.x,transform.position.y,targetLocalPos.z + offset.z);
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
