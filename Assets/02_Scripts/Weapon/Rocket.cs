using UnityEngine;

public class Rocket : MonoBehaviour
{
        private float range = 0;
        private float speed = 1f;
        private int damage = 0;
        private Transform target = null;
        private Vector3 startPos = Vector3.zero;
        private string targetTag = string.Empty;
        private Rigidbody rb;
        public Transform Target { set { target = value; } }
        public void Setup(string tg, float rng, float spd, int dmg)
        {
            targetTag = tg;
            range = rng;
            speed = spd; 
            damage = dmg; 
        }
    
        private void Start()
        {
            startPos = transform.position;
            if (target!=null) target.GetComponent<EnemyBehaviour>().IsTargetted = true;
            rb = GetComponent<Rigidbody>();
        }

        private void Update()
        {
            var currentRange = Vector3.Distance(startPos, transform.position);
            if (currentRange > range) Destroy(gameObject);
            if (target!=null) transform.LookAt(target);
            rb.velocity = transform.forward * speed;
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
