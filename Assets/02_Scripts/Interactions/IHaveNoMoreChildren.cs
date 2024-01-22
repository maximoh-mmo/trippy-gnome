using UnityEngine;

public class IHaveNoMoreChildren : MonoBehaviour
{
    // Start is called before the first frame update
    private HealthManager[] kids;
    void Start()
    {
        kids = GetComponentsInChildren<HealthManager>();
    }

    // Update is called once per frame
    void Update()
    {
        if (kids.Length < 1)
        {
            Destroy(gameObject);
        } 
    }
}
