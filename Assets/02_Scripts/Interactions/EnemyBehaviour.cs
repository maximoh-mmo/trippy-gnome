using System.Linq;
using UnityEngine;
using UnityEngine.Serialization;

public class EnemyBehaviour : MonoBehaviour
{
    private Transform player = null;
    private bool isTargetted = false;
    private Rigidbody rigidbody;
    [SerializeField] private float forwardMovementSpeed;
    private WayPoint wp;
    private int nearestWp;
    public float ForwardMovementSpeed { get { return forwardMovementSpeed; } set { forwardMovementSpeed = value; } }
    public bool IsTargetted { get => isTargetted; set => isTargetted = value; }
    void Start()
    {
        wp = FindFirstObjectByType<WayPoint>();
        if (forwardMovementSpeed == 0) forwardMovementSpeed = FindFirstObjectByType<MoveWithPath>().Speed;
        rigidbody = GetComponent<Rigidbody>();
        player = GameObject.FindFirstObjectByType<CrosshairMovement>().transform;
        transform.LookAt(player.position);
        nearestWp = GetNearestWp();
    }
    // Update is called once per frame
    void Update()
    {
            transform.LookAt(player.position);
            if (rigidbody != null) TravelInDirection(PathDirection());
    }

    void TravelInDirection(Vector3 pathDirection)
    {
        rigidbody.velocity = pathDirection * (forwardMovementSpeed-2);
    }

    Vector3 PathDirection()
    {
        if (Vector3.Dot(player.forward, transform.position - player.position) <= 0)
            return Vector3.zero;
        if (Vector3.Distance(wp.GetItem(nearestWp), transform.position) < .9f)
        {
            nearestWp = nearestWp > wp.waypoints.Count - 1 ? 0: nearestWp + 1 ;
        }

        var fwd = nearestWp > wp.waypoints.Count - 1
            ? Vector3.Normalize(wp.GetItem(0) - wp.GetItem(nearestWp) )
            : Vector3.Normalize( wp.GetItem(nearestWp + 1) - wp.GetItem(nearestWp));
        return fwd;
    }

    int GetNearestWp()
    {
        var nearestWp = wp.waypoints
            .OrderBy(t => Vector3.Distance(transform.position, t))
            .First();
        for (int i = 0; i < wp.waypoints.Count; i ++)
        {
            if (wp.GetItem(i) != nearestWp) continue;
            return i;
        }
        return -1;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other != null && other.gameObject.name == "Sweeper") { Destroy(gameObject); }            
    }
}
