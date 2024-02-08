using UnityEngine;

public class IHaveNoMoreChildren : MonoBehaviour
{
    // Start is called before the first frame update
    private HealthManager[] kids;
    void Update()
    {
        kids = GetComponentsInChildren<HealthManager>();
        if (kids.Length == 0)
        {
            Destroy(gameObject);
        } 
    }
}
