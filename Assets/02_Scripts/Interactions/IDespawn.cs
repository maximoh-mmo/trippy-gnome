using UnityEngine;

public class IDespawn : MonoBehaviour
{
    private Camera mainCam;
    private void Start()
    {
        mainCam = Camera.main;
    }

    private void Update()
    {
        if (Vector3.Dot(mainCam.transform.forward, transform.position - mainCam.transform.position) <= 0) Destroy(gameObject);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other != null && other.gameObject.name == "Sweeper") { Destroy(gameObject); } 
    }
}