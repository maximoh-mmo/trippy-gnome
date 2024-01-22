using UnityEngine;

public class IDespawn : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other != null && other.gameObject.name == "Sweeper") { Destroy(gameObject); } 
    }
}