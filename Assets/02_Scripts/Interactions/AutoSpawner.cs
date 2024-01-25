using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Pool;
using Random = UnityEngine.Random;

public class AutoSpawner : MonoBehaviour
{
    private SpawnBubble spawnBubble;
    private int numToSpawn, minCost;
    [SerializeField] private List<Enemy> enemies;
    private List<GameObject>enemiesToSpawn;
    [SerializeField] private int shopValue;
    [SerializeField] private bool useShop;
    [SerializeField] private float delay;
    private float startTime;
    private ComboCounter player;
    private bool isSpawning;
    private bool startDelay;
    public int extraCash;
    public int MinSpawns { get; set; }
    public int NumToSpawn { set => numToSpawn = value; }
    public int ShopValue { set => shopValue = value; }
    void Start()
    {
        startTime = Time.time + delay;
        player = FindFirstObjectByType<ComboCounter>();
        spawnBubble = FindFirstObjectByType<SpawnBubble>();
        enemiesToSpawn = new List<GameObject>();
        minCost = enemies
            .OrderBy(t => t.cost)
            .FirstOrDefault()!
            .cost;
        //create list based on price of enemies
    }
    void Update()
    {
        if (!startDelay)
        {
            if (Time.time > startTime)
            {
                startDelay = true;
                player.StartCoroutine("ComboLevelCountDown");
            }
            else
                return;
        }
        if (!player) Destroy(gameObject);
        if (spawnBubble.CountSpawns() < MinSpawns && !spawnBubble.SpawnStarted && !isSpawning)
        {
            isSpawning = true;
            if (useShop) ShopAndSpawn();
            else
            {
                spawnBubble.SpawnEnemies(numToSpawn);
            }
        }
    }

    private void ShopAndSpawn()
    {
        enemiesToSpawn.Clear();
        var money = shopValue + extraCash;
        extraCash = 0;
        while (money > minCost)
        {
            var enemy = Random.Range(0, enemies.Count);
            var cost = enemies[enemy].cost;
            if (money - cost >=0)
            {
                enemiesToSpawn.Add(enemies[enemy].prefab);
                money -= cost;
            }
        }
        spawnBubble.SpawnEnemies(enemiesToSpawn.ToArray());
        isSpawning = false;
    }

    public void AddMoney(int coins)
    {
         extraCash += coins;
    }
}

[Serializable]
public class Enemy
{
    public GameObject prefab;
    public int cost;
}