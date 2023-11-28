using Unity.VisualScripting;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    private Transform StartTransform;
    float range = 0f;
    int damage = 0;
    Vector3 startPos = Vector3.zero;
    public float Range { set { range = value; } }
    public int Damage { set { range = value; } }

    private void Start()
    {
        StartTransform = transform;
        startPos = StartTransform.position;
    }

    private void Update()
    {
        transform.position += transform.forward;
        float currentRange = Vector3.Distance(startPos, transform.position);
        if (currentRange > range)
        {
            Destroy(gameObject);
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        Debug.Log(" I hit a thing!!!! woooooo: " + collision.gameObject.name);
        if (collision.gameObject.CompareTag("Enemy"))
        {
            collision.gameObject.GetComponent<HealthManager>().TakeDamage(damage);
        }
        Destroy(gameObject);
    }
}