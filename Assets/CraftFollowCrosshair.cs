using Unity.VisualScripting;
using UnityEngine;

public class CraftFollowCrosshair : MonoBehaviour
{
    [SerializeField] Transform targetToFollow;
    [SerializeField] float moveSpeed;
    [SerializeField] CrosshairMovement crosshairMovement;
    [SerializeField] CameraScriptv2 cameraScriptv2;
    Vector2 clampLimit = Vector2.zero;
    

    void Update()
    { 
        clampLimit = ClampCalculation(crosshairMovement.Boundry, cameraScriptv2.Limits);
        Vector3 moveGoal = Vector3.Lerp(transform.position, new Vector3(targetToFollow.position.x, targetToFollow.position.y, transform.position.z), moveSpeed * Time.deltaTime);
        transform.position = new Vector3(Mathf.Clamp(moveGoal.x, -clampLimit.x, clampLimit.x), Mathf.Clamp(moveGoal.y, -clampLimit.y, clampLimit.y),transform.position.z);
    }
    Vector2 ClampCalculation(Vector2 a, Vector2 b)
    {
        a = a / 2;
        b = b / 2;
        Vector2 retVect = Vector2.zero;
        float az = targetToFollow.transform.position.z;
        float bz = cameraScriptv2.transform.position.z;
        float adj = transform.position.z - bz;
        retVect.x = Mathf.Tan(Mathf.Atan2(a.x - b.x, az - bz)) * adj;
        retVect.y = Mathf.Tan(Mathf.Atan2(a.y - b.y, az - bz)) * adj;
        return (retVect+crosshairMovement.ShipSize)*2;
    }
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawLine(new Vector3(clampLimit.x, clampLimit.y, gameObject.transform.position.z), new Vector3(clampLimit.x, -clampLimit.y, gameObject.transform.position.z));
        Gizmos.DrawLine(new Vector3(-clampLimit.x, clampLimit.y, gameObject.transform.position.z), new Vector3(clampLimit.x, clampLimit.y, gameObject.transform.position.z));
        Gizmos.DrawLine(new Vector3(-clampLimit.x, clampLimit.y, gameObject.transform.position.z), new Vector3(-clampLimit.x, -clampLimit.y, gameObject.transform.position.z));
        Gizmos.DrawLine(new Vector3(-clampLimit.x, -clampLimit.y, gameObject.transform.position.z), new Vector3(clampLimit.x, -clampLimit.y, gameObject.transform.position.z));
    }
}
