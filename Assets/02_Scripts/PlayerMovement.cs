using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField]float xySpeedMultiplier = 18f;
    float rollSpeed = 200;
    [SerializeField]float forwardSpeed = 1f;
    float leanLimit = 75f;
    [SerializeField] Transform aimTarget;
    [SerializeField] private float minHeight = 1f;

    void Update()
    {
        float x = Input.GetAxis("Horizontal");
        float y = Input.GetAxis("Vertical");
        LocalMove(x, y, forwardSpeed, xySpeedMultiplier);
        CraftRoll(x, y);
        HorizontalLean(transform, x, .1f);
    }

    void LocalMove(float x, float y, float z, float speed)
    {
        transform.localPosition += speed * Time.deltaTime * new Vector3(x, y, z);
        ClampPosition();
    }

    void ClampPosition()
    {
        Vector3 pos = Camera.main.WorldToViewportPoint(transform.position);
        pos.x = Mathf.Clamp01(pos.x);
        pos.y = Mathf.Clamp01(pos.y);
        transform.localPosition = Camera.main.ViewportToWorldPoint(pos);
    }
        void CraftRoll(float h, float v)
    {
        transform.rotation = Quaternion.RotateTowards(transform.rotation, Quaternion.LookRotation(aimTarget.position), Mathf.Deg2Rad * rollSpeed * Time.deltaTime);
    }
    void HorizontalLean(Transform target, float axis, float lerpTime)
    {
        Vector3 targetEulerAngels = target.localEulerAngles;
        target.localEulerAngles = new Vector3(targetEulerAngels.x, targetEulerAngels.y, Mathf.LerpAngle(targetEulerAngels.z, -axis * leanLimit, lerpTime));
    }
    private float GetHeightFromGround()
    {
        Physics.Raycast(transform.position, Vector3.down, out RaycastHit hitInfo);
        if (hitInfo.collider == null)
        {
            return 100f;
        }
        else
        {
            return hitInfo.distance;
        }
    }
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(aimTarget.position, 0.5f);
        Gizmos.DrawSphere(aimTarget.position, 0.15f);
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(aimTarget.position + Vector3.forward * 2, 0.25f);
        Gizmos.DrawSphere(aimTarget.position + Vector3.forward *2, 0.05f);
    }
}