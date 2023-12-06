using UnityEngine;

public class CraftFollowCrosshair : MonoBehaviour
{
    [SerializeField] private Transform targetToFollow;
    [SerializeField] private float moveSpeed, clampMultiplier;
    [SerializeField] private CrosshairMovement crosshairMovement;
    [SerializeField] private CameraScriptv2 cameraScriptv2;
    private Vector2 shipSize = Vector2.zero;
    private Vector2 clampLimit = Vector2.zero;
    private float offset;
    public Vector2 ShipSize { get { return shipSize; } }

    private void Awake()
    {
        shipSize = GetShipSize();
    }
    
    private void Start()
    {
        offset = transform.position.z;
        clampLimit = CalculateLimitBasedOnDistance(crosshairMovement.Boundry, cameraScriptv2.Limits);
        clampLimit -= shipSize * clampMultiplier;
    }

    private void Update()
    {
        HandleMovement();
    }

    void HandleMovement()
    {
        Vector3 moveGoal = Vector3.Lerp(transform.position, new Vector3(targetToFollow.position.x, targetToFollow.position.y, targetToFollow.position.z+offset), moveSpeed * Time.deltaTime);
        transform.position = new Vector3(Mathf.Clamp(moveGoal.x, -clampLimit.x, clampLimit.x), Mathf.Clamp(moveGoal.y, -clampLimit.y, clampLimit.y), targetToFollow.position.z+offset);
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
        if (Camera.main != null)
        {
            Vector2 returnVector;
            var camPosZ = Camera.main.transform.position.z;
            var adjacent = GetLengthBetween(camPosZ,crosshairMovement.gameObject.transform.position.z);
            var oppositeX = GetLengthBetween(a.x, b.x);
            var oppositeY = GetLengthBetween(a.y, b.y);
            var angleX = Mathf.Atan2(oppositeX, adjacent); 
            var angleY = Mathf.Atan2(oppositeY, adjacent);
            var cameraToShipAdj = GetLengthBetween(camPosZ, this.transform.position.z);
            returnVector.x = Mathf.Tan(angleX) * cameraToShipAdj + Mathf.Abs(b.x);
            returnVector.y = Mathf.Tan(angleY) * cameraToShipAdj + Mathf.Abs(b.y);
            return returnVector;
        }

        return Vector2.zero;
        
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
        foreach (Renderer renderer in renderers)
        {
            Debug.Log(renderer.gameObject);
            if (renderer.bounds.size.x > maxSizeX) maxSizeX = renderer.bounds.size.x;
            if (renderer.bounds.size.y > maxSizeY) maxSizeY = renderer.bounds.size.y;
        }
        return new Vector2(maxSizeX, maxSizeY)/2;
    }
    float GetLengthBetween(float a, float b)
    {
        return Mathf.Abs(a) - Mathf.Abs(b);
    }
    private void OnDrawGizmos()
    {
        var position = gameObject.transform.position;
        Gizmos.color = Color.cyan;
        
        Gizmos.DrawLine(new Vector3(clampLimit.x, clampLimit.y, position.z), new Vector3(clampLimit.x, -clampLimit.y, position.z));
        Gizmos.DrawLine(new Vector3(-clampLimit.x, clampLimit.y, position.z), new Vector3(clampLimit.x, clampLimit.y, position.z));
        Gizmos.DrawLine(new Vector3(-clampLimit.x, clampLimit.y, position.z), new Vector3(-clampLimit.x, -clampLimit.y, position.z));
        Gizmos.DrawLine(new Vector3(-clampLimit.x, -clampLimit.y, position.z), new Vector3(clampLimit.x, -clampLimit.y, position.z));
    }
}
