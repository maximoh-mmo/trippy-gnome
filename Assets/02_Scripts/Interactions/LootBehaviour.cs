using UnityEngine;

public class LootBehaviour : MonoBehaviour
{
    private int type;
    private SphereCollider sc;
    private bool flyToPlayer = false;
    private Transform player;
    private HUDController hud;
    [SerializeField] private float initialRadius, flyingRadius, speed;
    
    public int Type { set => type = value; }
    
    void Start()
    {
        hud = FindObjectOfType<HUDController>();
        sc = gameObject.AddComponent(typeof(SphereCollider)) as SphereCollider;
        if (sc != null)
        {
            sc.transform.parent = transform;
            sc.center = Vector3.zero;
            sc.radius = initialRadius;
            sc.isTrigger = true;
        }
        else Destroy(gameObject);
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
        if (other.gameObject.name == "Sweeper") Destroy(gameObject);
        if (!other.CompareTag("Player")) return;
        if (sc.radius == flyingRadius) PickUpItem();
        else if (sc.radius == initialRadius) FlyToPlayer(other.gameObject.transform);
    }

    private void PickUpItem()
    {
        hud.AddPowerUp(type);
        Destroy(gameObject);
    }

    private void FlyToPlayer(Transform target)
    {
        sc.radius = flyingRadius;
        flyToPlayer = true;
        player = target;
    }
}

