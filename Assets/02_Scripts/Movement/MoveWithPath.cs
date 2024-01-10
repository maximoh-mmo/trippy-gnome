using UnityEngine;

public class MoveWithPath : MonoBehaviour
{
    [SerializeField] bool loop = false;
    [SerializeField] float speed = 10f;
    [SerializeField] float minHeight = 1f;
    [SerializeField] float maxHeight = 11f;
    [SerializeField] GameObject terrainTiles;
    [SerializeField][Range(0, 1)] float ratio;
    Terrain terrain;
    GameObject NextTile;
    Vector3 aimPoint = Vector3.zero;
    WayPoint wayPoint = null;
    int wayPointNumber, tileNumber = 1;
    public float MinHeight { get { return minHeight; } }
    public float Speed { get { return speed; } set { speed = value; } }
    public float MapheightAtPos(Vector3 position) { return terrain.SampleHeight(position); }
    public Vector3 GetAimPoint(int wpNum) { return wayPoint.GetItem(wpNum); }
    private void Start()
    {
        terrain = terrainTiles.GetComponentInChildren<Terrain>();
        wayPoint = FindObjectOfType<WayPoint>();
        aimPoint = GetAimPoint(wayPointNumber);
        transform.position = new Vector3(aimPoint.x, MapheightAtPos(aimPoint) + (minHeight + maxHeight )/2, aimPoint.z);
        transform.LookAt(new Vector3(GetAimPoint(1).x,MapheightAtPos(GetAimPoint(1)) + (minHeight + maxHeight )/2, GetAimPoint(1).z));
        wayPointNumber++;
        aimPoint = GetAimPoint(wayPointNumber);
    }
    void Update()
    {
        if (NextWP()) {
            if (wayPointNumber < wayPoint.lastElement) { wayPointNumber++; }
            else if (loop) { wayPointNumber = 0; }
            else HandleNextTerrainTile();
            aimPoint = GetAimPoint(wayPointNumber);
        }
        //face aimPoint ignoring y axis as aimpoint always y = 0
        Quaternion aimPointHorizontalRotation = Quaternion.LookRotation(new Vector3(aimPoint.x, transform.position.y, aimPoint.z) - transform.position);
        transform.rotation = Quaternion.Slerp(transform.rotation, aimPointHorizontalRotation, ratio * Time.deltaTime);
        
        //move toward target using newly calculated forward direction.
        Vector3 target = transform.position + transform.forward * (Time.deltaTime * speed);
        target.y = MapheightAtPos(transform.position) + ((minHeight + maxHeight )/2);
        transform.position = Vector3.Slerp(transform.position, target, speed);

        //set tilt? if so normalize terrain gradient to set current tilt
     
        }

    private float GetHorizontalDistanceToNextWayPoint(int wpNumber)
    {
        if (wpNumber == 0) {
            float distanceBetweenLastAndNextAimPoint = Vector2.Distance(
                new Vector2(GetAimPoint(wpNumber).x, GetAimPoint(wpNumber).z), 
                new Vector2(GetAimPoint(wayPoint.lastElement).x, GetAimPoint(wayPoint.lastElement).z));

            float currentDistance = Vector2.Distance(
                new Vector2(aimPoint.x, aimPoint.z), 
                new Vector2(transform.position.x, transform.position.z));

            return (distanceBetweenLastAndNextAimPoint - currentDistance) / distanceBetweenLastAndNextAimPoint;
        }
        else
        {
            float distanceBetweenLastAndNextAimPoint = Vector2.Distance(
                new Vector2(GetAimPoint(wpNumber).x, GetAimPoint(wpNumber).z), 
                new Vector2(GetAimPoint(wpNumber - 1).x, GetAimPoint(wpNumber - 1).z));
            float currentDistance = Vector2.Distance(
                new Vector2(aimPoint.x, aimPoint.z), 
                new Vector2(transform.position.x, transform.position.z));

            return (distanceBetweenLastAndNextAimPoint - currentDistance) / distanceBetweenLastAndNextAimPoint;
        }
    }
    bool NextWP() 
    {
        return GetHorizontalDistanceToNextWayPoint(wayPointNumber) > .9f; 
    }

    void HandleNextTerrainTile() {
        Debug.Log("ChangeScene");
        NextTile = Instantiate(terrainTiles, new Vector3(0, 0, terrain.terrainData.size.z), Quaternion.identity);
        wayPoint = NextTile.GetComponentInChildren<WayPoint>();
        wayPoint.Translate(tileNumber);
        terrain = NextTile.GetComponentInChildren<Terrain>();
        tileNumber++;
        wayPointNumber = 0;
    }
    
}
