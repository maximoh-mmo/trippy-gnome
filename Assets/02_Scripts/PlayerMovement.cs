using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] float xyspeed = 18f;
    [SerializeField] float lookSpeed = 340;
    [SerializeField] Transform aimTarget;
    [SerializeField] float forwardSpeed =0.5f;
    Transform playerModel;
    private void Start()
    {
        playerModel = transform.GetChild(0);
    }

    void Update()
    {
        float h = Input.GetAxis("Mouse X");
        float v = Input.GetAxis("Mouse Y");
        LocalMove(h, v, forwardSpeed, xyspeed);
        RotationLook(h,v);
        HorizontalLean(aimTarget,h, 80, .1f);
        ClampPosition();

    }

    void LocalMove(float x, float y, float z, float speed)
    {
        transform.localPosition += speed * Time.deltaTime * new Vector3(x, y, z);
    }
    void ClampPosition()
    {
        Vector3 pos = Camera.main.WorldToViewportPoint(transform.position);
        pos.x = Mathf.Clamp01(pos.x);
        pos.y = Mathf.Clamp01(pos.y);
        transform.localPosition = Camera.main.ViewportToWorldPoint(pos);
    }
    void RotationLook(float h, float v)
    {
        aimTarget.localPosition = new Vector3(h, v, 1);
        transform.rotation = Quaternion.RotateTowards(transform.rotation, Quaternion.LookRotation(aimTarget.position), Mathf.Deg2Rad * lookSpeed);
    }
    void HorizontalLean(Transform target, float axis, float leanLimit, float lerpTime)
    {
        Vector3 targetEulerAngels = target.localEulerAngles;
        target.localEulerAngles = new Vector3(targetEulerAngels.x, targetEulerAngels.y, Mathf.LerpAngle(targetEulerAngels.z, -axis * leanLimit, lerpTime));
    }
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(aimTarget.position, 0.5f);
        Gizmos.DrawSphere(aimTarget.position, 0.15f);
    }
}