using UnityEngine;

public class PlayerMovementv2 : MonoBehaviour
{
    [SerializeField]float xySpeedMultiplier = 18f;
    float leanLimit = 75f;
    [SerializeField] Transform aimTarget;
    [SerializeField] GameObject menu;

    void Update()
    {
        if (Input.GetKey(KeyCode.Escape) == true)
        {
            Time.timeScale = 0f;
            menu.SetActive(true);
        }
        if (Time.timeScale != 0)
        {
            float x = Input.GetAxis("Horizontal");
            float y = Input.GetAxis("Vertical");
            LocalMove(x, y, xySpeedMultiplier);
            HorizontalLean(transform, x, .1f);
        }
    }

    void LocalMove(float x, float y, float speed)
    {
        transform.localPosition += speed * Time.deltaTime * new Vector3(x, y, transform.localPosition.z);
    }
    void HorizontalLean(Transform target, float axis, float lerpTime)
    {
        Vector3 targetEulerAngels = target.localEulerAngles;
        target.localEulerAngles = new Vector3(targetEulerAngels.x, targetEulerAngels.y, Mathf.LerpAngle(targetEulerAngels.z, -axis * leanLimit, lerpTime));
    }
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(aimTarget.position + aimTarget.forward * 20, 0.5f);
        Gizmos.DrawSphere(aimTarget.position + aimTarget.forward * 20, 0.15f);
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(aimTarget.position + aimTarget.forward * 10, 0.25f);
        Gizmos.DrawSphere(aimTarget.position + aimTarget.forward * 10, 0.05f);
    }
}