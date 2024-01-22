using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.InputSystem;

public class CrosshairMovement : MonoBehaviour
{
    [SerializeField] private float xySpeedMultiple;
    [SerializeField] private Vector2 boundary;
    [SerializeField] private float barrelRollReuseTime;
    [SerializeField] private float barrelRollSpeed;
    private bool isRolling;
    private Vector2 inputVector, rollDirection;
    private Vector3 rollStart;
    private PlayerInputSystem playerInputSystem;
    private float BarrelRollReuseTime { get { return barrelRollReuseTime; } set { barrelRollReuseTime = value; } }
    public Vector2 Boundry => boundary;
    public Vector2 InputVector => inputVector;
    private void Awake()
    {
        playerInputSystem = new PlayerInputSystem();
        playerInputSystem.InGame.Enable();
        playerInputSystem.InGame.BarrelRoll.performed += BarrelRoll;
        if (xySpeedMultiple==0) { xySpeedMultiple = 1; }
    }
    private void Update()
    {
        inputVector = playerInputSystem.InGame.Move.ReadValue<Vector2>();
        var startPosition = transform.localPosition;
        if (isRolling)
        {
            DoRoll(startPosition);
        }
        var multiple = (Time.deltaTime * xySpeedMultiple);
        var endPosition = new Vector3( inputVector.x * multiple,inputVector.y * multiple) + startPosition; 
        transform.localPosition = new Vector3(Mathf.Clamp(endPosition.x, -boundary.x, boundary.x),
            Mathf.Clamp(endPosition.y, -boundary.y, boundary.y), endPosition.z);
    }

    private void BarrelRoll(InputAction.CallbackContext context)
    {
        if (Time.time < BarrelRollReuseTime) return;
        rollDirection = inputVector.normalized;
        rollStart = transform.position;
        isRolling = true;
    }

    private void DoRoll(Vector3 position);
    {
        var endPosition = new Vector3(rollDirection.x, rollDirection.y); 
        transform.localPosition = new Vector3(Mathf.Clamp(endPosition.x, -boundary.x, boundary.x),
            Mathf.Clamp(endPosition.y, -boundary.y, boundary.y), endPosition.z);
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
