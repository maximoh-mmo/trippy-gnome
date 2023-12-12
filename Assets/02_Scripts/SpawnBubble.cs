using UnityEngine;

public class SpawnBubble : MonoBehaviour
{
    [SerializeField] int numberToSpawn;
    [SerializeField] Vector3 size, center;
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
        if (spawned == false) { SpawnEnemies(numberToSpawn); }
    }

    public void SpawnEnemies(int number)
    {
        spawned = true;
        for (int i = 0; i < number; i++)
        {
            SpawnEnemy(RandomSpawnPoint());
        }
        //Debug.Log(CountSpawns());
    }

    private Vector3 RandomSpawnPoint()
    {
        var sp = transform.TransformPoint(new Vector3(Random.Range(-size.x / 2, size.x / 2), 0, Random.Range(0, size.z))+center/2);
        var h = moveWithPath.MapheightAtPos(sp);
        sp.y = Random.Range(h + minHeight, h + maxHeight);
        return sp;
    }
    public int CountSpawns()
    {
        int i = 0;
        var healthManagers = GameObject.FindObjectsOfType<HealthManager>();
        foreach (var health in healthManagers)
        {
            i++;
        }

        return i;
    }
    void SpawnEnemy(Vector3 pos)
    {
        GameObject newEnemy = Instantiate(EnemyPrefab, pos, Quaternion.identity);
    }
}