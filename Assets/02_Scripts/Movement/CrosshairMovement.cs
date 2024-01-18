using UnityEngine;
using UnityEngine.InputSystem;

public class CrosshairMovement : MonoBehaviour
{
    [SerializeField] private float xySpeedMultiple;
    [SerializeField] private Vector2 boundary;
    private Vector2 inputVector;
    private PlayerInputSystem playerInputSystem;
    public Vector2 Boundry => boundary;
    public Vector2 InputVector => inputVector;
    void Awake()
    {
        playerInputSystem = new PlayerInputSystem();
        playerInputSystem.InGame.Enable();
        playerInputSystem.InGame.BarrelRoll.performed+=BarrelRoll;
        if (xySpeedMultiple==0) { xySpeedMultiple = 1; }
    }
    // Update is called once per frame
    void Update()
    {
        inputVector = playerInputSystem.InGame.Move.ReadValue<Vector2>();
        var targetPosition = transform.localPosition;
        var multiple = (Time.deltaTime * xySpeedMultiple);
        targetPosition += new Vector3( inputVector.x * multiple,inputVector.y * multiple);
        transform.localPosition = new Vector3(Mathf.Clamp(targetPosition.x, -boundary.x, boundary.x),
            Mathf.Clamp(targetPosition.y, -boundary.y, boundary.y), targetPosition.z);
    }

    public void BarrelRoll(InputAction.CallbackContext context)
    {
        Debug.Log("BarrelRoll");
        
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
