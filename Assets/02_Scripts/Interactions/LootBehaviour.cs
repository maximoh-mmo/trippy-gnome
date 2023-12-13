using UnityEngine;

public class LootBehaviour : MonoBehaviour
{
    private int type;
    private SphereCollider sc;
    private bool flyToPlayer = false;
    private Transform player;
    
    [SerializeField] private float initialRadius, flyingRadius, speed;
    
    public int Type { set => type = value; }
    // Start is called before the first frame update
    void Start()
    {
        // add a sphere collider that's big so that the player can collect from a distance
        sc = gameObject.AddComponent(typeof(SphereCollider)) as SphereCollider;
        if (sc != null)
        {
            sc.transform.parent = transform;
            sc.center = Vector3.zero;
            sc.radius = initialRadius;
            sc.isTrigger = true;
        }
        else Destroy(this);
    }

    private void Update()
    {
        if (flyToPlayer!=true) return;
        var transform1 = transform;
        transform1.LookAt(player);
        transform1.localPosition += transform1.forward * (speed * Time.deltaTime);
        transform.position = transform1.position;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other != null && other.gameObject.name == "Sweeper") { Debug.Log(other.name); Destroy(this.gameObject); }
        if (!other.CompareTag("Player")) return;
        if (sc.radius == flyingRadius)
        {
            Debug.Log(other.name);
            PickUpItem();
        }
        else if (sc.radius == initialRadius)
        {
            FlyToPlayer(other.gameObject.transform);
        }
    }

    private void PickUpItem()
    {
        Debug.Log("wooohoo you got me");
        Destroy(gameObject);
    }

    private void FlyToPlayer(Transform target)
    {
        Debug.Log("I'm Flying to you");
        sc.radius = flyingRadius;
        flyToPlayer = true;
        player = target;
    }
}

