using UnityEngine;

public class CrosshairMovement : MonoBehaviour
{
    private float xMovement, yMovement;
    [SerializeField] private float xySpeedMultiple;
    [SerializeField] private Vector2 boundary;
    CraftFollowCrosshair craftFollowCrosshair;
    private Vector2 screenBounds;
    private Vector2 shipSize;
    private Transform relativeZero;
    public Vector2 Boundry => boundary;
    public Transform RelativeZero => relativeZero;
    public float XMovement => xMovement;

    void Awake()
    {
        relativeZero = GetComponentInParent<Transform>();
        if (xySpeedMultiple==0) { xySpeedMultiple = 1; }
        craftFollowCrosshair = GameObject.FindFirstObjectByType<CraftFollowCrosshair>();
    }
    private void Start()
    {
        shipSize = craftFollowCrosshair.ShipSize;
    }
    // Update is called once per frame
    void Update()
    {
        xMovement = Input.GetAxis("Horizontal");
        yMovement = Input.GetAxis("Vertical");
        ProcessMovement();
    }
    private void ProcessMovement()
    {
        var targetPosition = transform.localPosition;
        var multiple = (Time.deltaTime * xySpeedMultiple);
        targetPosition += new Vector3(xMovement * multiple, yMovement * multiple);
        transform.localPosition = new Vector3(Mathf.Clamp(targetPosition.x, -boundary.x + shipSize.x, boundary.x - shipSize.x),
            Mathf.Clamp(targetPosition.y, -boundary.y + shipSize.y, boundary.y - shipSize.y), targetPosition.z);
    }

    private Vector3 ClampPosition(Vector3 targetPosition, float xClamp, float yClamp)
    {
        return new Vector3(Mathf.Clamp(targetPosition.x, -xClamp + shipSize.x, xClamp - shipSize.x),
            Mathf.Clamp(targetPosition.y, -yClamp + shipSize.y,yClamp - shipSize.y),targetPosition.z);
    }
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        var positionZ = transform.position.z;
        Gizmos.DrawLine(new Vector3(boundary.x, boundary.y,positionZ), new Vector3(boundary.x, -boundary.y, positionZ));
        Gizmos.DrawLine(new Vector3(-boundary.x, boundary.y, positionZ), new Vector3(boundary.x, boundary.y, positionZ));
        Gizmos.DrawLine(new Vector3(-boundary.x, boundary.y, positionZ), new Vector3(-boundary.x, -boundary.y, positionZ));
        Gizmos.DrawLine(new Vector3(-boundary.x, -boundary.y, positionZ), new Vector3(boundary.x, -boundary.y, positionZ));
    }
}
