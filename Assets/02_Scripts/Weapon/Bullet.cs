using UnityEngine;
using Random = UnityEngine.Random;

public class Bullet : MonoBehaviour, IPlaySoundIfFreeSourceAvailable
{
    private float range;
    private float speed = 1f;
    private int damage;
    Vector3 startPos = Vector3.zero;
    string targetTag = string.Empty;
    private Rigidbody rb;
    [SerializeField]private GameObject projectileHit;
    [Header("Sound Randomizer settings")]
    [SerializeField] private float minPitch = 0.975f;
    [SerializeField] private float maxPitch = 1.125f;
    [SerializeField] [Range(0,1)] private float minVolume = 0;
    [SerializeField] [Range(0,1)] private float maxVolume = 1;
    private AudioSource audioSource;
    public void Setup(string tg, float rng, float spd, int dmg)
    {
        targetTag = tg;
        range = rng;
        speed = spd; 
        damage = dmg; 
    }

    private void Start()
    {
        audioSource = GetComponent<AudioSource>();
        startPos = transform.position;
        rb = GetComponent<Rigidbody>();
        rb.AddForce(transform.forward * speed, ForceMode.Impulse);
        PlayAudioOnFirstFreeAvailable();
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
            if (projectileHit) Instantiate(projectileHit, other.ClosestPointOnBounds(transform.position) , Quaternion.identity);
            Destroy(gameObject); 
        }
        
        if (other.gameObject.GetComponent<ComboCounter>() != null)
        {
            Debug.Log(gameObject.name);
            other.gameObject.GetComponent<ComboCounter>().ImHit();
            Destroy(gameObject);
        }
    }

    public void PlayAudioOnFirstFreeAvailable()
    {
        audioSource.pitch = Random.Range(minPitch, maxPitch);
        audioSource.volume = Random.Range(minVolume, maxVolume);
        audioSource.Play();
    }
}