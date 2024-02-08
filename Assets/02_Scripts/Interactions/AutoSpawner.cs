using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

public class AutoSpawner : MonoBehaviour
{
    private SpawnBubbleV2 spawnBubble;
    private int numToSpawn, minCost;
    private List<GameObject>enemiesToSpawn;
    [Header("Random Spawn Settings")]
    [SerializeField] private List<Enemy> enemies;
    [SerializeField] private int shopValue;
    [SerializeField] private bool spawnEnemiesUsingShop;
    [SerializeField] private float initialSpawnDelay;
    private int extraCash;
    private float startTime;
    private ComboCounter player;
    private bool isSpawning;
    private bool isStartDelayOver;
    
    public int MinSpawns { get; set; }
    public int NumToSpawn { set => numToSpawn = value; }
    public int ShopValue { set => shopValue = value; }
    void Start()
    {
        startTime = Time.time + initialSpawnDelay;
        player = FindFirstObjectByType<ComboCounter>();
        spawnBubble = FindFirstObjectByType<SpawnBubbleV2>();
        enemiesToSpawn = new List<GameObject>();
        minCost = enemies
            .OrderBy(t => t.cost)
            .FirstOrDefault()!
            .cost;
        //create list based on price of enemies
    }
    void Update()
    {
        if (!isStartDelayOver)
        {
            if (Time.time > startTime)
            {
                isStartDelayOver = true;
                player.StartCoroutine("ComboLevelCountDown");
            }
            else
                return;
        }
        if (!player) MinSpawns = 0;
        if (spawnBubble.CountSpawns() < MinSpawns && !spawnBubble.SpawnStarted && !isSpawning)
        {
            isSpawning = true;
            if (spawnEnemiesUsingShop) ShopAndSpawn();
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

    public void PauseRespawn(float pauseSeconds)
    {
        StartCoroutine(PauseRespawnForSeconds(pauseSeconds));
    }
    IEnumerator PauseRespawnForSeconds(float pauseSeconds)
    {
        var d = MinSpawns;
        MinSpawns = 0;
        yield return new WaitForSeconds(pauseSeconds);
        MinSpawns = d;
        player.StartCoroutine("ComboLevelCountDown");
        player.IsBoomActivated = false;
    }
}

[Serializable]
public class Enemy
{
    public GameObject prefab;
    public int cost;
}