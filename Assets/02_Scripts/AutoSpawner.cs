using UnityEngine;

public class AutoSpawner : MonoBehaviour
{
    private SpawnBubble spawnBubble;
    private int minSpawns, numToSpawn;
    public int MinSpawns { set => minSpawns = value; }
    public int NumToSpawn { set => numToSpawn = value; }
    // Start is called before the first frame update
    void Start()
    {
        spawnBubble = GameObject.FindFirstObjectByType<SpawnBubble>();
    }
    void Update()
    {
        if (spawnBubble.CountSpawns() < minSpawns)
        {
            spawnBubble.SpawnEnemies(numToSpawn);
        }
    }
}
