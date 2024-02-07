using System.Collections;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;

public class SpawnBubble : MonoBehaviour
{
    [SerializeField] int numberToSpawn;
    [SerializeField] Vector3 spawnBoxSize;
    [SerializeField] Vector3 center;
    [SerializeField] GameObject enemyPrefab;
    private bool spawned = true;
    private bool spawnStarted = false;
    BoxCollider spawnArea;
    private Terrain terrain;
    private MainMenu mainMenu;
    private ComboCounter comboCounter;

    public bool SpawnStarted => spawnStarted;

    public void ManualSpawn(InputAction.CallbackContext context)
    {
        if (comboCounter.IsCheating) spawned = false;
    }

    void Start()
    {
        terrain = Terrain.activeTerrain;
        mainMenu = FindFirstObjectByType<MainMenu>();
        mainMenu.playerInputSystem.Cheater.Enable();
        mainMenu.playerInputSystem.Cheater.SpawnAdds.performed += ManualSpawn;
        comboCounter = FindFirstObjectByType<ComboCounter>();
        GameObject child = new GameObject("SpawnArea", typeof(BoxCollider));
        var t = transform;
        child.transform.parent = t;
        child.transform.position = t.position;
        child.transform.rotation = t.rotation;
        child.transform.SetParent(t);
        spawnArea = child.GetComponent<BoxCollider>();
        spawnArea.isTrigger = true;
        spawnArea.size = spawnBoxSize;
        spawnArea.center = center;
    }

    void Update()
    {
        spawnArea.size = spawnBoxSize;
        spawnArea.center = center;
        if (spawned == false)
        {
            SpawnEnemies(numberToSpawn);
        }
    }

    public void SpawnEnemies(int number)
    {
        spawned = true;
        for (int i = 0; i < number; i++)
        {
            SpawnEnemy(RandomSpawnPoint(GetSize(enemyPrefab).y));
        }
    }

    public void SpawnEnemies(GameObject[] enemies)
    {
        spawnStarted = true;
        foreach (var enemy in enemies)
        {
            StartCoroutine(SpawnEnemyWithDelay(RandomSpawnPoint(GetSize(enemy).y), enemy));
        }
    }

    private Vector3 RandomSpawnPoint(float minHeight)
    {
        var sp = transform.TransformPoint(new Vector3(Random.Range(-spawnBoxSize.x / 2, spawnBoxSize.x / 2), 0,
            Random.Range(0, spawnBoxSize.z)) + center / 2);
        var h = terrain.SampleHeight(sp);
        sp.y = Random.Range(h + minHeight, h + (spawnArea.size.y / 2));
        return sp;
    }

    public int CountSpawns()
    {
        var enemies = GameObject.FindGameObjectsWithTag("Enemy").Distinct();
        return enemies.Count();
    }

    void SpawnEnemy(Vector3 pos)
    {
        Instantiate(enemyPrefab, pos, Quaternion.identity);
    }

    void SpawnEnemy(Vector3 pos, GameObject prefabToSpawn)
    {
        var enemy = Instantiate(prefabToSpawn, pos, Quaternion.identity);
        enemy.transform.LookAt(comboCounter.transform);
    }

    private Vector3 GetSize(GameObject t)
    {
        Vector3 size = Vector3.zero;
        Renderer[] renderers = t.GetComponentsInChildren<Renderer>();
        foreach (Renderer r in renderers)
        {
            if (r.bounds.size.x > size.x) size.x = r.bounds.size.x;
            if (r.bounds.size.y > size.y) size.y = r.bounds.size.y;
            if (r.bounds.size.z > size.z) size.z = r.bounds.size.z;
        }

        return size / 2;
    }

    private IEnumerator SpawnEnemyWithDelay(Vector3 pos, GameObject prefabToSpawn)
    {
        yield return new WaitForSeconds(Random.Range(0, 0.5f));
        var enemy = Instantiate(prefabToSpawn, pos, Quaternion.identity);
        enemy.transform.LookAt(comboCounter.transform);
        spawnStarted = false;

    }
}