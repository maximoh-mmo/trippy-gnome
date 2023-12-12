using UnityEngine;

public class Bullet : MonoBehaviour
{
    float range = 0;
    float speed = 1f;
    int damage = 0;
    Vector3 startPos = Vector3.zero;
    string targetTag = string.Empty;
    public string TargetTag { set { targetTag = value; } }
    public float Range { set { range = value; } }
    public float Speed { set { speed = value; } }
    public int Damage { set { damage = value; } }

    private void Start()
    {
        startPos = transform.position;
    }

    private void Update()
    {   transform.position += transform.forward * speed * Time.deltaTime;
        var currentRange = Vector3.Distance(startPos, transform.position);
        if (currentRange > range)
        {
            Debug.Log("out of range");
            Destroy(gameObject);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other == null) return;
        if (other.gameObject.name == "Sweeper")
        {
            Destroy(this.gameObject);
        }
        if (other.gameObject.CompareTag(targetTag) && other.gameObject.GetComponent<HealthManager>() != null)
        {
            other.gameObject.GetComponent<HealthManager>().TakeDamage(damage);
        }

        if (other.gameObject.GetComponent<ComboCounter>() != null)
        {
            other.gameObject.GetComponent<ComboCounter>().ImHit();
        }
    }
}
