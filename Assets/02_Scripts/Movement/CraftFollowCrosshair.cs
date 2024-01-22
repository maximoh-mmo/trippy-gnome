using UnityEngine;

public class CraftFollowCrosshair : MonoBehaviour
{
    [SerializeField] private Transform targetToFollow;
    [SerializeField] private float moveSpeed, clampMultiplier, leanLimit, lerpTime;
    [SerializeField] private CrosshairMovement crosshairMovement;
    [SerializeField] private DynamicChaseCamera dynamicChaseCamera;
    private MoveWithPath moveWithPath;
    private Vector2 clampLimit = Vector2.zero;
    private float offset;
    private bool show = false;
    
    public Vector3 ShipSize { get; private set; } = Vector3.zero;

    private void Awake()
    {
        ShipSize = GetSize(gameObject);
        offset = transform.position.z;
        moveWithPath = GetComponentInParent<MoveWithPath>();
    }
    
    private void Start()
    {
        clampLimit = CalculateLimitBasedOnDistance(crosshairMovement.Boundry, dynamicChaseCamera.Limits);
        clampLimit -= new Vector2(ShipSize.x/2,ShipSize.y/2) * clampMultiplier;
    }

    private void Update()
    {
        transform.localPosition = HandleMovement(targetToFollow.localPosition);
        transform.LookAt(targetToFollow);
    }
    private Vector3 HandleMovement(Vector3 crosshairPosition)
    {
        var moveGoal = Vector3.Lerp(transform.localPosition, new Vector3(crosshairPosition.x, crosshairPosition.y, crosshairPosition.z+offset), moveSpeed * Time.deltaTime);
        var cl = clampLimit;
        if (cl.y + ShipSize.y/2 < moveWithPath.MapheightAtPos(transform.position) + ShipSize.y/2) cl.y =  moveWithPath.MapheightAtPos(transform.position) + ShipSize.y/2;
        return new Vector3(Mathf.Clamp(moveGoal.x, -cl.x, cl.x), Mathf.Clamp(moveGoal.y, -cl.y, cl.y), crosshairPosition.z+offset);
    }
    
    Vector2 CalculateLimitBasedOnDistance(Vector2 a, Vector2 b)
    {
        if (Camera.main == null) return Vector2.zero;
        Vector2 returnVector;
        var camPosZ = Camera.main.transform.position.z;
        var adjacent = GetLengthBetween(camPosZ,crosshairMovement.gameObject.transform.position.z);
        var oppositeX = GetLengthBetween(a.x, b.x);
        var oppositeY = GetLengthBetween(a.y, b.y);
        var angleX = Mathf.Atan2(oppositeX, adjacent); 
        var angleY = Mathf.Atan2(oppositeY, adjacent);
        var cameraToShipAdj = GetLengthBetween(camPosZ, transform.position.z);
        returnVector.x = Mathf.Tan(angleX) * cameraToShipAdj + Mathf.Abs(b.x);
        returnVector.y = Mathf.Tan(angleY) * cameraToShipAdj + Mathf.Abs(b.y);
        return returnVector;
    }
    
    private Vector3 GetSize(GameObject t)
    {
        Vector3 size = Vector3.zero;
        Renderer[] renderers = t.GetComponentsInChildren<Renderer>();
        foreach (Renderer r in renderers)
        {
            if (r.bounds.size.x > size.x) size.x = r.bounds.size.x;
            if (r.bounds.size.y > size.y) size.y = r.bounds.size.y;
            if (r.bounds.size.z > size.z) size.z = r.bounds.size.z;
        }
        return size/2;
    }
    float GetLengthBetween(float a, float b)
    {
        return Mathf.Abs(a) - Mathf.Abs(b);
    }

    private void OnDrawGizmos()
    {
        if (!show){return;}
        var positionZ = gameObject.transform.position.z;
        Gizmos.color = Color.cyan;
        
        Gizmos.DrawLine(new Vector3(clampLimit.x, clampLimit.y, positionZ), new Vector3(clampLimit.x, -clampLimit.y, positionZ));
        Gizmos.DrawLine(new Vector3(-clampLimit.x, clampLimit.y, positionZ), new Vector3(clampLimit.x, clampLimit.y, positionZ));
        Gizmos.DrawLine(new Vector3(-clampLimit.x, clampLimit.y, positionZ), new Vector3(-clampLimit.x, -clampLimit.y, positionZ));
        Gizmos.DrawLine(new Vector3(-clampLimit.x, -clampLimit.y, positionZ), new Vector3(clampLimit.x, -clampLimit.y, positionZ));
        
       
        Gizmos.color = Color.red;
        Gizmos.DrawRay(transform.position, transform.forward * 1000);
    }
}
