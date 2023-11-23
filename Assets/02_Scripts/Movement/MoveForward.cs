using UnityEngine;

public class MoveForward : MonoBehaviour
{
    [SerializeField] Terrain terrain; 
    [SerializeField] float speed=10f;
    [SerializeField] float minHeight = 3f;
    [SerializeField] float rotationSpeed =10f;
    [SerializeField] float slerpDistance = 100f;
    Vector3 aimPoint = Vector3.zero;
    WayPoint wayPoint = null;
    int wayPointNumber = 0;
    public float MapheightAtPos(Vector3 position) { return terrain.SampleHeight(position); }
    public Vector3 GetAimPoint(int wpNum)
    { 
        Vector3 ap = wayPoint.GetItem(wpNum);
        ap.y = 0;
        return ap;
    }
    private void Start()
    {
        wayPoint = GetComponent<WayPoint>();
        aimPoint = GetAimPoint(wayPointNumber);
        transform.position = new Vector3(aimPoint.x, MapheightAtPos(aimPoint), aimPoint.z);
        wayPointNumber++;
        aimPoint = GetAimPoint(wayPointNumber);
    }
    void Update()
    {
        if (NextWP()) { wayPointNumber++; aimPoint = GetAimPoint(wayPointNumber); }
        Aim();
        Vector3 target = transform.position + transform.forward * Time.deltaTime * speed;
        target.y = MapheightAtPos(transform.position) + minHeight;
        transform.position = Vector3.Slerp(transform.position, target, speed);
    }
    void Aim()
    {
        Vector3 relativePos = aimPoint - transform.position;
        Quaternion rotation = Quaternion.LookRotation(relativePos, Vector3.up);
        rotation.z = 0;
        rotation.x = 0;
        transform.rotation = Quaternion.Slerp(transform.rotation, rotation, Time.deltaTime * rotationSpeed * Quaternion.Angle(transform.rotation, rotation));        
    }
    float GetHorizontalDistanceToWP()
    {
        Debug.Log(wayPointNumber);
        if (wayPointNumber < 1) { return 0; }
        else
        {
            float distanceBetweenLastAndNextAimPoint = Vector2.Distance(new Vector2(GetAimPoint(wayPointNumber).x, GetAimPoint(wayPointNumber).z), new Vector2(GetAimPoint(wayPointNumber - 1).x, GetAimPoint(wayPointNumber - 1).z));
            float currentDistance = Vector2.Distance(new Vector2(aimPoint.x, aimPoint.z), new Vector2(transform.position.x, transform.position.z));
            return (distanceBetweenLastAndNextAimPoint - currentDistance) / distanceBetweenLastAndNextAimPoint;
        }
    }
    bool NextWP() 
    {
        return GetHorizontalDistanceToWP() > .9f; 
    }
}
