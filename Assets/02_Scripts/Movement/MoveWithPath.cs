using UnityEngine;

public class MoveWithPath : MonoBehaviour
{
    [SerializeField] bool loop;
    [SerializeField] float speed = 10f;
    [SerializeField] float minHeight = 1f;
    [SerializeField] float maxHeight = 11f;
    [SerializeField] GameObject terrainTiles;
    [SerializeField][Range(0, 1)] float ratio;
    Terrain terrain;
    GameObject nextTile;
    Vector3 aimPoint = Vector3.zero;
    WayPoint wayPoint;
    int wayPointNumber, tileNumber = 1;
    public float Speed { get => speed; set => speed = value; }
    public float MapheightAtPos(Vector3 position) => terrain.SampleHeight(position);

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
        if (NextWp()) {
            if (wayPointNumber < wayPoint.lastElement) { wayPointNumber++; }
            else if (loop) { wayPointNumber = 0; }
            else HandleNextTerrainTile();
            aimPoint = GetAimPoint(wayPointNumber);
        }
        var tr = transform;
        var tPos = tr.position;
        //face aimPoint ignoring y axis as aimpoint always y = 0
        Quaternion aimPointHorizontalRotation = Quaternion.LookRotation(new Vector3(aimPoint.x, tPos.y, aimPoint.z) - tPos);
        transform.rotation = Quaternion.Slerp(transform.rotation, aimPointHorizontalRotation, ratio * Time.deltaTime);
        
        //move toward target using newly calculated forward direction.
        Vector3 target = tPos + tr.forward * (Time.deltaTime * speed);
        target.y = MapheightAtPos(tPos) + ((minHeight + maxHeight )/2);
        transform.position = Vector3.Slerp(transform.position, target, speed);

        //set tilt? if so normalize terrain gradient to set current tilt
     
        }
    private Vector3 GetAimPoint(int wpNum) => wayPoint.GetItem(wpNum);

    private float GetHorizontalDistanceToNextWayPoint(int wpNumber)
    {
        if (wpNumber == 0)
        {
            var t = transform.position;
            float distanceBetweenLastAndNextAimPoint = Vector2.Distance(
                new Vector2(GetAimPoint(wpNumber).x, GetAimPoint(wpNumber).z), 
                new Vector2(GetAimPoint(wayPoint.lastElement).x, GetAimPoint(wayPoint.lastElement).z));
            float currentDistance = Vector2.Distance(
                new Vector2(aimPoint.x, aimPoint.z), 
                new Vector2(t.x, t.z));
            return (distanceBetweenLastAndNextAimPoint - currentDistance) / distanceBetweenLastAndNextAimPoint;
        }
        else
        {
            var t = transform.position;
            float distanceBetweenLastAndNextAimPoint = Vector2.Distance(
                new Vector2(GetAimPoint(wpNumber).x, GetAimPoint(wpNumber).z), 
                new Vector2(GetAimPoint(wpNumber - 1).x, GetAimPoint(wpNumber - 1).z));
            float currentDistance = Vector2.Distance(
                new Vector2(aimPoint.x, aimPoint.z), 
                new Vector2(t.x, t.z));

            return (distanceBetweenLastAndNextAimPoint - currentDistance) / distanceBetweenLastAndNextAimPoint;
        }
    }
    bool NextWp() 
    {
        return GetHorizontalDistanceToNextWayPoint(wayPointNumber) > .9f; 
    }

    void HandleNextTerrainTile() {
        Debug.Log("ChangeScene");
        nextTile = Instantiate(terrainTiles, new Vector3(0, 0, terrain.terrainData.size.z), Quaternion.identity);
        wayPoint = nextTile.GetComponentInChildren<WayPoint>();
        wayPoint.Translate(tileNumber);
        terrain = nextTile.GetComponentInChildren<Terrain>();
        tileNumber++;
        wayPointNumber = 0;
    }
    
}
