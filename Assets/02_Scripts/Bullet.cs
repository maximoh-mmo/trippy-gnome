using UnityEngine;

public class Bullet : MonoBehaviour
{
    float range = 0;
    float speed = 1f;
    int damage = 0;
    Vector3 startPos = Vector3.zero;
    string targetTag = string.Empty;
    bool hit = false;
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
            float currentRange = Vector3.Distance(startPos, transform.position);
            if (currentRange > range)
            {
                Destroy(gameObject);
            }
        }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision != null && hit ==false)
        {
            hit = true;
            Debug.Log(" I hit a thing!!!! woooooo: " + collision.gameObject.name);
            Debug.Log((collision.gameObject.CompareTag(targetTag)));
            if (collision.gameObject.CompareTag(targetTag))
            {
                collision.gameObject.GetComponent<HealthManager>().TakeDamage(damage);
            }
            Destroy(gameObject);
        }
    }
}
