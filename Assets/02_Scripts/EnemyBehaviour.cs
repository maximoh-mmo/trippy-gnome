using UnityEngine;

public class EnemyBehaviour : MonoBehaviour
{
    Transform player = null;
    void Start()
    {
        if (GameObject.Find("Starcraft").transform != null)
        {
            player = GameObject.Find("Starcraft").transform;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (player != null)
        {
            transform.LookAt(player.position);
        }
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other != null)
        {
            if (other.gameObject.name == "Sweeper") { Destroy(this.gameObject); }            
        }
    }
}
