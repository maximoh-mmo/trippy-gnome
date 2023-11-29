using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    private Transform StartTransform;
    float range, speed = 0f;
    int damage = 0;
    Vector3 startPos = Vector3.zero;
    string targetTag = string.Empty;

    public string TargetTag { set { tag = value; } }
    public float Range { set { range = value; } }
    public float Speed { set { range = value; } }
    public int Damage { set { range = value; } }

    private void Start()
    {
        StartTransform = transform;
        startPos = StartTransform.position; 
        if (speed == 0) { speed = 1; }
    }

    private void Update()
    {        
        transform.position += transform.forward * Time.deltaTime * speed;
        float currentRange = Vector3.Distance(startPos, transform.position);
        if (currentRange > range)
        {
            Destroy(gameObject);
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        Debug.Log(" I hit a thing!!!! woooooo: " + collision.gameObject.name);
        if (collision.gameObject.CompareTag(targetTag))
        {
            collision.gameObject.GetComponent<HealthManager>().TakeDamage(damage);
        }
        Destroy(gameObject);
    }
}
