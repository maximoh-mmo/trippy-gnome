using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] float xyspeed = 18f;
    [SerializeField] Transform aimTarget;

    void Update()
    {
        float h = Input.GetAxis("Mouse X");
        float v = Input.GetAxis("Mouse Y");
        Debug.Log("Mouse X:"+ h + " Mouse Y:" +  v);
        LocalMove(h, v, xyspeed);
        ClampPosition(); 
    }

    void LocalMove(float x, float y, float speed)
    {
        transform.localPosition += speed * Time.deltaTime * new Vector3(x, y, 0);
    }
    void ClampPosition()
    {
        Vector3 pos = Camera.main.WorldToViewportPoint(transform.position);
        pos.x = Mathf.Clamp01(pos.x);
        pos.y = Mathf.Clamp01(pos.y);
        transform.localPosition = Camera.main.ViewportToWorldPoint(pos);
    }
    void RotationLook(float h, float v, float speed)
    {
        aimTarget.localPosition = new Vector3(h, v, 1);
        transform.rotation = Quaternion.RotateTowards(transform.rotation, Quaternion.LookRotation(aimTarget.position), Mathf.Deg2Rad * speed);
    }
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(aimTarget.position, 0.5f);
        Gizmos.DrawSphere(aimTarget.position, 0.15f);
    }
}