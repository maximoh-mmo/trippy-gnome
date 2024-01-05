using UnityEngine;

public class EnemyBehaviour : MonoBehaviour
{
    private Transform player = null;
    private bool isTargetted = false;
    public bool IsTargetted { get => isTargetted; set => isTargetted = value; }

    void Start()
    {
        player = GameObject.FindFirstObjectByType<CrosshairMovement>().transform;
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
