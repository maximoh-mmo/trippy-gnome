using UnityEngine;

public class Rocket : MonoBehaviour
{
        private float range = 0;
        private float speed = 1f;
        private int damage = 0;
        private Transform target = null;
        private Vector3 startPos = Vector3.zero;
        private string targetTag = string.Empty;
        public string TargetTag { set { targetTag = value; } }
        public float Range { set { range = value; } }
        public float Speed { set { speed = value; } }
        public int Damage { set { damage = value; } }
        public Transform Target { set { target = value; } }

        private void Start()
        {
            startPos = transform.position;
        }

        private void Update()
        {
            var t = transform;
            var tpos = t.position;
            tpos += t.forward * (speed * Time.deltaTime);
            var currentRange = Vector3.Distance(startPos, tpos);
            if (currentRange > range) Destroy(gameObject);
            transform.position = tpos;
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other == null) return;
            if (other.gameObject.name == "Sweeper") Destroy(this.gameObject);
        
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
