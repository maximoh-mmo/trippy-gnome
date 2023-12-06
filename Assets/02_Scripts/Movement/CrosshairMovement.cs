using UnityEngine;

public class CrosshairMovement : MonoBehaviour
{
    private float xMovement, yMovement;
    [SerializeField] private float xySpeedMultiple;
    [SerializeField] private Vector2 boundary;
    CraftFollowCrosshair CraftFollowCrosshair;
    private Vector2 screenBounds;
    private Vector2 shipSize;
    public Vector2 Boundry { get { return boundary; } }
    void Awake()
    {
        if (xySpeedMultiple==0) { xySpeedMultiple = 1; }
        CraftFollowCrosshair = GameObject.FindFirstObjectByType<CraftFollowCrosshair>();
    }
    private void Start()
    {
        shipSize = CraftFollowCrosshair.ShipSize;
    }
    // Update is called once per frame
    void Update()
    {
        screenBounds =  Camera.main.ScreenToWorldPoint(new Vector3(Screen.width, Screen.height, Camera.main.transform.position.z));
        xMovement = Input.GetAxis("Horizontal");
        yMovement = Input.GetAxis("Vertical");
        ProcessMovement();
    }
    private void ProcessMovement()
    {
        Vector3 targetPosition = transform.position + new Vector3(xMovement,yMovement) * Time.deltaTime * xySpeedMultiple;
        Vector3 clampedTargetPosition = ClampPosition(targetPosition, boundary.x, boundary.y);
        //Vector3 clampedTargetPosition = ScreenClampPosition(targetPosition);
        transform.position = clampedTargetPosition;
    }
    private Vector3 ScreenClampPosition(Vector3 position)
    {
        Vector3 viewPos = transform.position;
        viewPos.x = Mathf.Clamp(viewPos.x, screenBounds.x + shipSize.x, screenBounds.x * -1);
        viewPos.x = Mathf.Clamp(viewPos.y, screenBounds.y + shipSize.y, screenBounds.y * -1);
        return viewPos;
    }

    private Vector3 ClampPosition(Vector3 targetPosition, float xClamp, float yClamp)
    {
        Vector3 clampedTargetPosition = new Vector3(Mathf.Clamp(targetPosition.x, -xClamp + shipSize.x, xClamp - shipSize.x),Mathf.Clamp(targetPosition.y, -yClamp + shipSize.y,yClamp - shipSize.y),targetPosition.z);
        return clampedTargetPosition;
    }
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawLine(new Vector3(boundary.x, boundary.y,transform.position.z), new Vector3(boundary.x, -boundary.y, transform.position.z));
        Gizmos.DrawLine(new Vector3(-boundary.x, boundary.y, transform.position.z), new Vector3(boundary.x, boundary.y, transform.position.z));
        Gizmos.DrawLine(new Vector3(-boundary.x, boundary.y, transform.position.z), new Vector3(-boundary.x, -boundary.y, transform.position.z));
        Gizmos.DrawLine(new Vector3(-boundary.x, -boundary.y, transform.position.z), new Vector3(boundary.x, -boundary.y, transform.position.z));
    }
}
