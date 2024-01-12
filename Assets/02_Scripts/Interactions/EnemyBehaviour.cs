using System.Linq;
using UnityEngine;
public class EnemyBehaviour : MonoBehaviour
{
    private bool isTargetted = false;
    [SerializeField] private float forwardMovementSpeed;
    private int nearestWp;
    private Vector3 startPosition;
    private float previousGround;
    private MoveWithPath moveWithPath;
    private Rigidbody rigidbody;
    private GameObject player = null;
    private WayPoint wp;
    public float ForwardMovementSpeed => forwardMovementSpeed;
    public bool IsTargetted { get => isTargetted; set => isTargetted = value; }
    
    void Start()
    {
        wp = FindFirstObjectByType<WayPoint>();
        if (forwardMovementSpeed == 0) forwardMovementSpeed = FindFirstObjectByType<MoveWithPath>().Speed - 2;
        rigidbody = GetComponent<Rigidbody>();
        player = FindFirstObjectByType<ComboCounter>().gameObject;
        transform.LookAt(player.transform);
        nearestWp = GetNearestWpIndex();
        startPosition = transform.position;
        moveWithPath = FindObjectOfType<MoveWithPath>();
        previousGround = moveWithPath.MapheightAtPos(startPosition);
    }
    void Update()
    {
            if (rigidbody != null && Vector3.Dot(player.transform.forward, transform.position - player.transform.position) > 0) TravelInDirection(PathDirection());
            MoveWithGround();
    }
    private void FixedUpdate()
    {
        transform.LookAt(player.transform);
    }
    private void MoveWithGround()
    {
        var currentHeight = moveWithPath.MapheightAtPos(transform.position);
        var heightChange = currentHeight- previousGround;
        if (heightChange != 0) transform.position += new Vector3(0, heightChange, 0);
        previousGround = currentHeight;
    }
    void TravelInDirection(Vector3 pathDirection)
    {
        transform.position += pathDirection * (forwardMovementSpeed*Time.deltaTime);
    }
    Vector3 PathDirection()
    {
        if (Vector3.Distance(wp.GetItem(nearestWp), transform.position) < .9f) nearestWp = nearestWp > wp.waypoints.Count - 1 ? 0: nearestWp + 1 ;
        var fwd = nearestWp > wp.waypoints.Count - 1
            ? Vector3.Normalize(wp.GetItem(0) - wp.GetItem(nearestWp) )
            : Vector3.Normalize( wp.GetItem(nearestWp + 1) - wp.GetItem(nearestWp));
        return fwd;
    }
    int GetNearestWpIndex()
    {
        var nearestWayPoint = wp.waypoints
            .OrderBy(t => Vector3.Distance(transform.position, t))
            .First();
        return wp.waypoints.IndexOf(nearestWayPoint);
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other != null && other.gameObject.name == "Sweeper") { Destroy(gameObject); }            
    }
}