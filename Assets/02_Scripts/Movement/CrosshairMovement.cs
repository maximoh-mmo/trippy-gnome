using UnityEngine;
using UnityEngine.InputSystem;

public class CrosshairMovement : MonoBehaviour
{
    [SerializeField] private float xySpeedMultiple;
    [SerializeField] private Vector2 boundary;
    [SerializeField] private float barrelRollReuseTime;
    [SerializeField] private float barrelRollSpeed;
    [SerializeField] private float barrelRollDistance;
    private bool isRolling;
    private Animation ani;
    private Vector2 inputVector;
    private Vector2 rollDirection;
    private Vector3 rollStart;
    private PlayerInputSystem playerInputSystem;
    private string left, right;
    private float adjustment = -1f;

    private float BarrelRollReuseTime { get { return barrelRollReuseTime; } set { barrelRollReuseTime = value; } }
    public Vector2 Boundry => boundary;
    public Vector2 InputVector => inputVector;
    private void Awake()
    {
        ani = FindObjectOfType<ComboCounter>().GetComponentInChildren<Animation>();
        left = "BarrelRollLeft";
        right = "BarrelRollRight";
        playerInputSystem = new PlayerInputSystem();
        playerInputSystem.InGame.Enable();
        playerInputSystem.InGame.BarrelRoll.performed += BarrelRoll;
        if (xySpeedMultiple==0) { xySpeedMultiple = 1; }
    }
    private void Update()
    {
        RenderSettings.skybox.SetFloat("_Rotation", adjustment * Time.unscaledTime);
        inputVector = playerInputSystem.InGame.Move.ReadValue<Vector2>();
        var startPosition = transform.localPosition;
        if (isRolling)
        {
            if (!ani.isPlaying)
            {
                if (rollDirection.x < 0)
                {
                    ani.Play(left);
                }
                else
                {
                    ani.Play(right);    
                }
                
            }
            var pos = startPosition;
            var endPos = new Vector3(rollDirection.x, rollDirection.y);
            pos += endPos * barrelRollSpeed;
            if (Vector3.Distance(pos, rollStart) > barrelRollDistance) isRolling = false; 
            transform.localPosition = new Vector3(Mathf.Clamp(pos.x, -boundary.x, boundary.x),
                Mathf.Clamp(pos.y, -boundary.y, boundary.y), pos.z);
            return;
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
