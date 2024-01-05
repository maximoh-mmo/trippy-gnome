using UnityEngine;

public class Bullet : MonoBehaviour
{
    private float range = 0;
    private float speed = 1f;
    private int damage = 0;
    Vector3 startPos = Vector3.zero;
    string targetTag = string.Empty;
    private Rigidbody rb;

    public string TargetTag { set => targetTag = value; }
    public float Range { set => range = value; }
    public float Speed { set => speed = value; }
    public int Damage { set => damage = value; }

    private void Start()
    {
        startPos = transform.position;
        rb = GetComponent<Rigidbody>();
        rb.AddForce(transform.forward * speed, ForceMode.Impulse);
    }

    private void Update()
    {
        var currentRange = Vector3.Distance(startPos, transform.position);
        if (currentRange > range) Destroy(gameObject);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other == null) return;
        if (other.gameObject.name == "Sweeper") Destroy(gameObject);
        if (other.gameObject.CompareTag(targetTag) && other.gameObject.GetComponent<HealthManager>() != null)
        {
            other.gameObject.GetComponent<HealthManager>().TakeDamage(damage);
            Destroy(gameObject); 
        }

        if (other.gameObject.GetComponent<ComboCounter>() != null)
        {
            other.gameObject.GetComponent<ComboCounter>().ImHit();
            Destroy(gameObject);
        }
    }
}
