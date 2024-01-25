using UnityEngine;

public class HealthManager : MonoBehaviour
{
    [SerializeField] private int maxHealth;
    [SerializeField] private int CoinsForSpawnSystem;
    private float currentHealth;
    private int pointValue;
    private bool loot;
    [SerializeField] private bool lootDropOnBoom = true;
    private AutoSpawner autoSpawner;
    private ComboCounter counter;
    private GameObject poof;
    
    public int MaxHealth => maxHealth;

    private void Start()
    {
        if (GetComponentInChildren<EnemyBehaviour>().DeathPrefab != null)
        {
            poof = GetComponentInChildren<EnemyBehaviour>().DeathPrefab;
        }
        currentHealth = maxHealth;
        counter = FindObjectOfType<ComboCounter>();
        loot = GetComponent<Loot>();
        autoSpawner = FindFirstObjectByType<AutoSpawner>();
    }
    private void Update()
    {
        if (currentHealth < 1)
        {
            counter.AddKill(maxHealth);
            autoSpawner.AddMoney(CoinsForSpawnSystem);
            if (loot) GetComponent<Loot>().GetLoot(transform.position);
            if (poof) Instantiate(poof, transform.position, Quaternion.identity);
            Destroy(gameObject);
        }
    }
    public void TakeDamage(float dmg)
    {
        currentHealth -= dmg;
    }
    public void Kill()
    {
        counter.AddKill(maxHealth);
        if (loot && lootDropOnBoom) GetComponent<Loot>().GetLoot(transform.position);
        if (poof) Instantiate(poof, transform.position, Quaternion.identity);
        Destroy(gameObject);
    }
}