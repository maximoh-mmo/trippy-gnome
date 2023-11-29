using UnityEngine;

public class SpawnBubble : MonoBehaviour
{
    //Make sure there is a BoxCollider component attached to your GameObject
    [SerializeField] int numberToSpawn;
    [SerializeField] Vector3 size;
    [SerializeField] GameObject EnemyPrefab;
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
        gameObject.AddComponent<BoxCollider>();
        spawnArea = GetComponent<BoxCollider>();
    }

    void Update()
    {
        spawnArea.size = size;
        spawnArea.center = new Vector3(0, 0, (size.z/2));
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
    }

    Vector3 RandomSpawnPoint()
    {
        var sp = new Vector3(Random.Range(spawnArea.bounds.min.x, spawnArea.bounds.max.x),0,Random.Range(spawnArea.bounds.min.z, spawnArea.bounds.max.z));
        var h = moveWithPath.MapheightAtPos(sp);
        sp.y = Random.Range(h + minHeight, h + maxHeight);
        return sp;
    }
    int CountSpawns()
    {
        return 0;
    }
    void SpawnEnemy(Vector3 pos)
    {
        GameObject newEnemy = Instantiate(EnemyPrefab, pos,Quaternion.identity);
    }
    public Vector3 GetRandomPointInsideCollider(BoxCollider boxCollider)
    {
        Vector3 extents = boxCollider.size / 2f;
        Vector3 point = new Vector3(
            Random.Range(-extents.x, extents.x),
            Random.Range(-extents.y, extents.y),
            Random.Range(-extents.z, extents.z)
        );

        return boxCollider.transform.TransformPoint(point);
    }
}