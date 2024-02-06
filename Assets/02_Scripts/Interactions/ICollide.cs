
using UnityEngine;

public class ICollide : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.GetComponent<ComboCounter>() != null)
        {
            if (!other.gameObject.GetComponent<ComboCounter>().IsPsychoRushActive) other.gameObject.GetComponent<ComboCounter>().DeathHandler();
        }
        if (!other.gameObject.CompareTag("Player")){
            Destroy(other.gameObject);
        }
    }
}
