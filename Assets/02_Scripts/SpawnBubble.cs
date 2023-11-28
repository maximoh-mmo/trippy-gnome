
using Unity.VisualScripting;
using UnityEngine;

public class SpawnBubble : MonoBehaviour
{
    //Make sure there is a BoxCollider component attached to your GameObject
    BoxCollider spawnArea;
    [SerializeField] int numberToSpawn;
    [SerializeField] Vector3 size;
    bool spawned = true;
    void Start()
    {
        spawnArea = GetComponent<BoxCollider>();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space)) { spawned = false; }
        spawnArea.size = size;
        spawnArea.center = new Vector3(0, 0, size.z / 2);
        if (spawned == false) { Test(); }
    }

    void Test()
    {   
        spawned = true;
        for (int i = 0; i < numberToSpawn; i++)
            {
                GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
                cube.transform.position = RandomSpawnPoint();
                cube.tag = "Enemy";
            }
        
    }

    Vector3 RandomSpawnPoint()
    {
        var point = new Vector3(
            Random.Range(spawnArea.bounds.min.x, spawnArea.bounds.max.x),
            Random.Range(spawnArea.bounds.min.y, spawnArea.bounds.max.y),
            Random.Range(spawnArea.bounds.min.z, spawnArea.bounds.max.z)
        );

        if (point != spawnArea.ClosestPoint(point) && point.y > GetComponent<MoveWithPath>().MapheightAtPos(point))
        {
            Debug.Log("Out of the collider! Looking for other point...");
            point = RandomSpawnPoint();
        }
        return point;
    }

    int CountSpawns()
    {
        return 0;
    }
}