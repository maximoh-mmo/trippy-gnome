using System.Linq;
using UnityEngine;

public class EnemyBehaviour : MonoBehaviour
{
    private Transform player = null;
    private bool isTargetted = false;
    private Rigidbody rigidbody;
    private float speed;
    private WayPoint wp;
    private int nearestWp;
    public float Speed { get { return speed; } set { speed = value; } }
    public bool IsTargetted { get => isTargetted; set => isTargetted = value; }
    void Start()
    {
        wp = FindFirstObjectByType<WayPoint>();
        speed = FindFirstObjectByType<MoveWithPath>().Speed;
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
        rigidbody.velocity = pathDirection * speed;
    }

    Vector3 PathDirection()
    {
        if (Vector3.Dot(player.forward, transform.position - player.position) > 0)
            Debug.Log("I'm in front of player");
        else
        {
            Debug.Log("I'm behind player");
        }
        return Vector3.zero;
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
