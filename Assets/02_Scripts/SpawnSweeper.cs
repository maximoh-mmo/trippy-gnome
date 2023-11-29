using UnityEngine;

public class SpawnSweeper : MonoBehaviour
{
    [SerializeField] Vector3 size;
    BoxCollider sweepArea;
    void Start()
    {
        GameObject child = new GameObject("Sweeper", typeof(BoxCollider));
        child.transform.SetParent(this.transform);
        child.transform.parent = transform;
        child.transform.position = transform.position;
        child.transform.rotation = transform.rotation;
        child.transform.SetParent(this.transform);
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
