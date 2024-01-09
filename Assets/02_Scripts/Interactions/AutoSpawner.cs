using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class AutoSpawner : MonoBehaviour
{
    private SpawnBubble spawnBubble;
    private int minSpawns, numToSpawn, extraCash;
    [SerializeField]private List<GameObject> enemies;
    [SerializeField] private int shopValue;
    [SerializeField]private bool useShop; 
    public int MinSpawns { set => minSpawns = value; }
    public int NumToSpawn { set => numToSpawn = value; }
    public int ShopValue { set => shopValue = value; }

    void Start()
    {
        spawnBubble = GameObject.FindFirstObjectByType<SpawnBubble>();
    }
    void Update()
    {
        if (spawnBubble.CountSpawns() < minSpawns)
        {
            if (useShop) ShopAndSpawn();
            else
            {
                spawnBubble.SpawnEnemies(numToSpawn);
            }
        }
    }

    private void ShopAndSpawn()
    {
        var money = shopValue + extraCash;
        extraCash = 0;
        var enemiesToSpawn = new List<GameObject>();
        //create list based on price of enemies
        while (money > 0)
        {
            var enemy = Random.Range(0, enemies.Count);
            var cost = enemies[enemy].GetComponent<HealthManager>().MaxHealth;
            if (money - cost >=0)
            {
                enemiesToSpawn.Add(enemies[enemy]);
                money -= cost;
            }
            Debug.Log("money left = "+money);
        }
        spawnBubble.SpawnEnemies(enemiesToSpawn.ToArray());
    }

    public void AddMoney(int coins)
    {
         extraCash += coins;
    }
}