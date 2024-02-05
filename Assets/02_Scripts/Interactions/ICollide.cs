
using UnityEngine;

public class ICollide : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.GetComponent<ComboCounter>() != null)
        {
            other.gameObject.GetComponent<ComboCounter>().DeathHandler();
        }
        if (!other.gameObject.CompareTag("Player")){
            Destroy(other.gameObject);
        }
    }
}
