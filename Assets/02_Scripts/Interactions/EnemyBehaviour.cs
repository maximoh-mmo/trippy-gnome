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
    private GameObject player = null;
    private WayPoint wp;
    private EnemyMovement movementPattern;
    public float ForwardMovementSpeed => forwardMovementSpeed;
    public bool IsTargetted { get => isTargetted; set => isTargetted = value; }
    void Start()
    {
        wp = FindFirstObjectByType<WayPoint>();
        if (forwardMovementSpeed == 0) forwardMovementSpeed = FindFirstObjectByType<MoveWithPath>().Speed - 2;
        player = FindFirstObjectByType<ComboCounter>().gameObject;
        transform.LookAt(player.transform);
        nearestWp = GetNearestWpIndex();
        startPosition = transform.position;
        moveWithPath = FindObjectOfType<MoveWithPath>();
        previousGround = moveWithPath.MapheightAtPos(startPosition);
        if (GetComponent<EnemyMovement>() != null) movementPattern = GetComponent<EnemyMovement>();
    }
    void Update()
    {
        if (!player) Destroy(gameObject);
        if (Vector3.Dot(player.transform.forward, transform.position - player.transform.position) > 0)
        {
            var forwardMotion = PathDirection() * (forwardMovementSpeed*Time.deltaTime);
            var groundMotion = MoveWithGround();
            if (movementPattern) transform.position += movementPattern.ProcessMove(player.transform);
            transform.position += forwardMotion + groundMotion;
        }
        transform.LookAt(player.transform);
    }
    private Vector3 MoveWithGround()
    {
        var currentHeight = moveWithPath.MapheightAtPos(transform.position);
        var heightChange = currentHeight - previousGround;
        previousGround = currentHeight;
        return new Vector3(0, heightChange, 0);
    }
    private Vector3 PathDirection()
    {
        if (Vector3.Distance(wp.GetItem(nearestWp), transform.position) < .9f) nearestWp = nearestWp > wp.waypoints.Count - 1 ? 0: nearestWp + 1 ;
        var fwd = nearestWp > wp.waypoints.Count - 1
            ? Vector3.Normalize(wp.GetItem(0) - wp.GetItem(nearestWp))
            : Vector3.Normalize(wp.GetItem(nearestWp + 1) - wp.GetItem(nearestWp));
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
        if (other.gameObject.GetComponent<ComboCounter>() != null)
        {
            other.gameObject.GetComponent<ComboCounter>().ImHit();
            Destroy(gameObject);
        }
    }
}