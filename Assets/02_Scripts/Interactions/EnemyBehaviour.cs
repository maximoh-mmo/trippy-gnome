using UnityEngine;

public class EnemyBehaviour : MonoBehaviour
{
    Transform player = null;
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
        transform.LookAt(player.position);
    }

    // Update is called once per frame
    void Update()
    {
            transform.LookAt(player.position);
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other != null && other.gameObject.name == "Sweeper") { Destroy(gameObject); }            
    }
}
