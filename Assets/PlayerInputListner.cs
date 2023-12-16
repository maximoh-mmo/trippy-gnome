using UnityEngine;

public class PlayerInputListner : MonoBehaviour
{
    private Vector2 movementInput;
    private bool isShootPressed, isShieldPressed, isBigBoomPressed;
    public Vector2 MovementInput => movementInput;
    public bool IsShootPressed => isShootPressed;
    public bool IsShieldPressed => isShieldPressed;
    public bool IsBigBoomPressed => isBigBoomPressed;
    void Update()
    {
        movementInput = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));
        isShieldPressed = Input.GetKey(KeyCode.Alpha1);
        isShootPressed = Input.GetKeyDown(KeyCode.Mouse0);
        isBigBoomPressed = Input.GetKeyDown(KeyCode.Alpha2);
    }
}
