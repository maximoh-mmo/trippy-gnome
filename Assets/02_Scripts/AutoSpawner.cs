using UnityEngine;

public class AutoSpawner : MonoBehaviour
{
    private SpawnBubble spawnBubble;
    // Start is called before the first frame update
    void Start()
    {
        spawnBubble = GameObject.FindFirstObjectByType<SpawnBubble>();
    }
    void Update()
    {
        if (spawnBubble.CountSpawns() < 1)
        {
            spawnBubble.SpawnEnemies(3);
        }
    }
}
