using UnityEngine;

public class EnemyBehaviour : MonoBehaviour
{
    //[SerializeField]Dictionary<GameObject,float> Drops;

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
        if (other != null && other.gameObject.name == "Sweeper") { Destroy(this.gameObject); }            
    }
    private void OnDestroy()
    {
        GetComponent<Loot>().GetLoot(this.transform.position);
    }
}
