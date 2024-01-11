using UnityEngine;

public class CraftFollowCrosshair : MonoBehaviour
{
    [SerializeField] private Transform targetToFollow;
    [SerializeField] private float moveSpeed, clampMultiplier, leanLimit, lerpTime;
    [SerializeField] private CrosshairMovement crosshairMovement;
    [SerializeField] private DynamicChaseCamera dynamicChaseCamera;
    private Vector2 clampLimit = Vector2.zero;
    private float offset;
    private bool show = false;
    public Vector2 ShipSize { get; private set; } = Vector2.zero;
    public Transform Ship { get; private set; }

    private void Awake()
    {
        Ship = transform;
        ShipSize = GetShipSize();
        offset = transform.position.z;
    }
    
    private void Start()
    {
        clampLimit = CalculateLimitBasedOnDistance(crosshairMovement.Boundry, dynamicChaseCamera.Limits);
        clampLimit -= ShipSize * clampMultiplier;
    }

    private void Update()
    {
        transform.localPosition = HandleMovement(targetToFollow.localPosition);
        transform.LookAt(targetToFollow);
        HandleTilt();
    }
    private void HandleTilt()
    {
        var currentRoll = transform.localEulerAngles;
        transform.localEulerAngles = new Vector3( currentRoll.x,  currentRoll.y, Mathf.Lerp(currentRoll.z, -crosshairMovement.XMovement * leanLimit, lerpTime)); 
    }
        

    private Vector3 HandleMovement(Vector3 currentPosition)
    {
        var moveGoal = Vector3.Lerp(transform.localPosition, new Vector3(currentPosition.x, currentPosition.y, currentPosition.z+offset), moveSpeed * Time.deltaTime);
        return new Vector3(Mathf.Clamp(moveGoal.x, -clampLimit.x, clampLimit.x), Mathf.Clamp(moveGoal.y, -clampLimit.y, clampLimit.y), currentPosition.z+offset);
    }
    /// <summary>
    /// Uses Pythagorean Trigonometry to calculate a Vector 2 between two parallel sets of limits (Vector2 A and Vector2 B)
    /// take the lengths, calculate the angle of the triangle then use this to calculate the distance
    /// </summary>
    /// <param name="a"></param>
    /// <param name="b"></param>
    /// <returns>Vector2</returns>
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
    
    /// <summary>
    /// Iterates through renderers to get size of rendered craft in child objects.
    /// </summary>
    /// <returns>Vector2</returns>
    private Vector2 GetShipSize()
    {
        float maxSizeX = 0;
        float maxSizeY = 0;
        Renderer[] renderers = this.gameObject.GetComponentsInChildren<Renderer>();
        foreach (Renderer r in renderers)
        {
            if (r.bounds.size.x > maxSizeX) maxSizeX = r.bounds.size.x;
            if (r.bounds.size.y > maxSizeY) maxSizeY = r.bounds.size.y;
        }
        return new Vector2(maxSizeX, maxSizeY)/2;
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
