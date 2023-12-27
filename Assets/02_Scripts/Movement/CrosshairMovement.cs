using UnityEngine;

public class CrosshairMovement : MonoBehaviour
{
    private float xMovement, yMovement;
    [SerializeField] private float xySpeedMultiple;
    [SerializeField] private Vector2 boundary;
    CraftFollowCrosshair craftFollowCrosshair;
    private Vector2 shipSize;
    private PlayerInputSystem playerInputSystem;
    public Vector2 Boundry => boundary;
    public float XMovement => xMovement;

    void Awake()
    {
        playerInputSystem = new PlayerInputSystem();
        playerInputSystem.InGame.Enable();
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
        Vector2 inputVector = playerInputSystem.InGame.Move.ReadValue<Vector2>();
        var targetPosition = transform.localPosition;
        var multiple = (Time.deltaTime * xySpeedMultiple);
        targetPosition += new Vector3( inputVector.x * multiple,inputVector.y * multiple);
        transform.localPosition = new Vector3(Mathf.Clamp(targetPosition.x, -boundary.x, boundary.x),
            Mathf.Clamp(targetPosition.y, -boundary.y, boundary.y), targetPosition.z);
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
