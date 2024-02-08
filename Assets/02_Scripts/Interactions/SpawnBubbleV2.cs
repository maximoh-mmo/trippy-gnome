using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;

public class SpawnBubbleV2 : MonoBehaviour
{
    #region exposed variables
        [Header("Cheat Mode Spawn Options")]
        [SerializeField] private int numberToSpawn;
        [SerializeField] private GameObject enemyPrefab;

        [Header("Auto Spawn Settings")]
        [SerializeField] private float maxSpawnDistance;
        [SerializeField] private float minSpawnDistance;
        [SerializeField] private float minHeightFromTerrain;
        [SerializeField] private float maxHeightFromTerrain;
        [SerializeField] private float maxDistanceFromPath;
    #endregion
    #region variables
        private bool spawned = true;
        private bool spawnStarted = false;
    #endregion
    #region class references
        private MainMenu mainMenu;
        private ComboCounter comboCounter;
        private Terrain terrain;
        private List<Vector3> waypoints;
    #endregion
    #region getters and setters
        public bool SpawnStarted => spawnStarted;
    #endregion
    #region unity game loop

        void Start()
        {
            waypoints = FindFirstObjectByType<WayPoint>().waypoints;
            terrain = Terrain.activeTerrain;
            mainMenu = FindFirstObjectByType<MainMenu>();
            mainMenu.playerInputSystem.Cheater.Enable();
            mainMenu.playerInputSystem.Cheater.SpawnAdds.performed += ManualSpawn;
            comboCounter = FindFirstObjectByType<ComboCounter>();
        }
    
        void Update()
        {
            if (spawned == false)
            {
                SpawnEnemies(numberToSpawn);
            }
        }

    #endregion
    #region public methods

        public void SpawnEnemies(int number)
        {
            spawned = true;
            for (int i = 0; i < number; i++)
            {
                SpawnEnemy(GenerateRandomSpawnPoint(GetSize(enemyPrefab).y));
            }
        }
        public void SpawnEnemies(GameObject[] enemies)
        {
            spawnStarted = true;
            foreach (var enemy in enemies)
            {
                StartCoroutine(SpawnEnemyWithDelay(GenerateRandomSpawnPoint(GetSize(enemy).y), enemy));
            }
        }
        public int CountSpawns()
        {
            var enemies = GameObject.FindGameObjectsWithTag("Enemy").Distinct();
            return enemies.Count();
        }

    #endregion
    #region private methods

    private Vector3 GenerateRandomSpawnPoint(float minHeight)
    {
        var player = comboCounter.transform;
        var playerPosition = player.position;
        var returnVector = Vector3.zero;
        var playerPositionZeroY = new Vector3(playerPosition.x, 0, playerPosition.z);
        var spawnPoint = Random.insideUnitCircle * maxDistanceFromPath;
        var spawnDistanceOnPath = Random.Range(minSpawnDistance, maxSpawnDistance);
        var closestWp = waypoints
            .OrderBy(t => Vector3.Distance(t, playerPositionZeroY))
            .First();
        Vector3 nearestWpAhead;
        if (Vector3.Dot(player.forward, closestWp - playerPositionZeroY) > 0) nearestWpAhead = closestWp;
        else
        {
            var closestWpIndex = waypoints.IndexOf(closestWp);
            nearestWpAhead = closestWpIndex < waypoints.Count - 1 ? waypoints[closestWpIndex + 1] : waypoints[0];
        }
        var distToFirstWp = Vector3.Distance(playerPositionZeroY, nearestWpAhead);
        if (distToFirstWp > spawnDistanceOnPath)
        {
            var direction = (nearestWpAhead - playerPositionZeroY).normalized * spawnDistanceOnPath;
            returnVector = playerPositionZeroY + direction + new Vector3(spawnPoint.x, 0, spawnPoint.y);
        }
        var remainingDistance = spawnDistanceOnPath - distToFirstWp;
        while (remainingDistance > 0)
        {
            var nextWaypoint = NextWaypoint(nearestWpAhead);
            var distanceBetweenNextWaypoints = Vector3.Distance(nearestWpAhead, nextWaypoint);
            if (distanceBetweenNextWaypoints > remainingDistance)
            {
                var direction = (nextWaypoint - nearestWpAhead).normalized * remainingDistance;
                returnVector = nearestWpAhead + direction + new Vector3(spawnPoint.x, 0, spawnPoint.y);
                break;
            }
            nearestWpAhead = nextWaypoint;
            remainingDistance -= distanceBetweenNextWaypoints;
        }
        if (returnVector != Vector3.zero)
        {
            returnVector.y = Random.Range(minHeightFromTerrain + minHeight + terrain.SampleHeight(returnVector), 
                maxHeightFromTerrain);
        }
        return returnVector;
    }
        
        private Vector3 NextWaypoint(Vector3 waypoint)
        {
            var index = waypoints.IndexOf(waypoint);
            return index == waypoints.Count - 1 ? waypoints[0] : waypoints[index + 1];
        }
        private void SpawnEnemy(Vector3 pos)
        {
            Instantiate(enemyPrefab, pos, Quaternion.identity);
        }
        private void SpawnEnemy(Vector3 pos, GameObject prefabToSpawn)
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
        private void ManualSpawn(InputAction.CallbackContext context)
        {
            if (comboCounter.IsCheating) spawned = false;
        }
    #endregion
    #region coroutines
    private IEnumerator SpawnEnemyWithDelay(Vector3 pos, GameObject prefabToSpawn)
    {
        yield return new WaitForSeconds(Random.Range(0, 0.5f));
        var enemy = Instantiate(prefabToSpawn, pos, Quaternion.identity);
        enemy.transform.LookAt(comboCounter.transform);
        spawnStarted = false;
    }
    #endregion
}