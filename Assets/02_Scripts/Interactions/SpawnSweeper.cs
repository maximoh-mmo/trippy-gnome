using UnityEngine;

public class SpawnSweeper : MonoBehaviour
{
    [SerializeField] Vector3 size;
    private BoxCollider sweepArea;
    void Start()
    {
        GameObject child = new GameObject("Sweeper", typeof(BoxCollider));
        var t = transform;
        child.transform.SetParent(t);
        child.transform.parent = transform;
        child.transform.position = t.position;
        child.transform.rotation = t.rotation;
        sweepArea = child.GetComponent<BoxCollider>();
        sweepArea.isTrigger = true;
        sweepArea.size = size;
        sweepArea.center = new Vector3(0, 0, (-size.z / 2));
    }
    void Update()
    {
        sweepArea.size = size;
        sweepArea.center = new Vector3(0, 0, (-size.z / 2));
    }
}
