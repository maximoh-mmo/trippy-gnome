using UnityEngine;

public class SpawnBubble : MonoBehaviour
{
    [SerializeField] int numberToSpawn;
    [SerializeField] Vector3 size, center;
    [SerializeField] GameObject EnemyPrefab;
    [SerializeField] LayerMask LayerMask;
    bool spawned = true;
    BoxCollider spawnArea;
    MoveWithPath moveWithPath;
    float minHeight;
    float maxHeight = 10f;
    void Start()
    {
        moveWithPath = FindObjectOfType<MoveWithPath>();
        minHeight = moveWithPath.MinHeight;
        maxHeight = moveWithPath.MaxHeight;
        GameObject child = new GameObject("SpawnArea", typeof(BoxCollider));
        child.transform.parent = transform;
        child.transform.position = transform.position;
        child.transform.rotation = transform.rotation;
        child.transform.SetParent(this.transform); 
        spawnArea = child.GetComponent<BoxCollider>();
        spawnArea.isTrigger = true;
        spawnArea.size = size;
        spawnArea.center = center;
    }

    void Update()
    {
        spawnArea.size = size;
        spawnArea.center = center; 
        if (Input.GetKeyDown(KeyCode.Space)) { spawned = false; }
        if (spawned == false) { Test(); }
    }

    void Test()
    {
        spawned = true;
        for (int i = 0; i < numberToSpawn; i++)
        {
            SpawnEnemy(RandomSpawnPoint());
        }
        //Debug.Log(CountSpawns());
    }

    Vector3 RandomSpawnPoint()
    {
        var sp = transform.TransformPoint(new Vector3(Random.Range(-size.x / 2, size.x / 2), 0, Random.Range(0, size.z)));
        var h = moveWithPath.MapheightAtPos(sp);
        sp.y = Random.Range(h + minHeight, h + maxHeight);
        return sp;
    }
    int CountSpawns()
    {
        var count = 0;
        Collider[] hitColliders = Physics.OverlapBox(spawnArea.center, size / 2, Quaternion.identity, LayerMask);
        foreach (Collider collider in hitColliders)
        {
            if (collider.gameObject.CompareTag("Enemy")) {  count++; }
        }
        return count;
    }
    void SpawnEnemy(Vector3 pos)
    {
        GameObject newEnemy = Instantiate(EnemyPrefab, pos, Quaternion.identity);
    }
}