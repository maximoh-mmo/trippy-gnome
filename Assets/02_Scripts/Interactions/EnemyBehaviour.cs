using UnityEngine;

public class EnemyBehaviour : MonoBehaviour
{
    private Transform player = null;
    private bool isTargetted = false;
    private Rigidbody rigidbody;
    private float speed;    
    public float Speed { get { return speed; } set { speed = value; } }
    public bool IsTargetted { get => isTargetted; set => isTargetted = value; }
    void Start()
    {
        speed = FindFirstObjectByType<MoveWithPath>().Speed;
        rigidbody = GetComponent<Rigidbody>();
        player = GameObject.FindFirstObjectByType<CrosshairMovement>().transform;
        transform.LookAt(player.position);
    }
    // Update is called once per frame
    void Update()
    {
            transform.LookAt(player.position);
            //if (rigidbody != null) TravelInDirection(pathDirection());
    }

    void TravelInDirection(Vector3 pathDirection)
    {
        rigidbody.velocity = pathDirection * speed;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other != null && other.gameObject.name == "Sweeper") { Destroy(gameObject); }            
    }
}
