using UnityEngine;

public class HealthManager : MonoBehaviour
{
    [SerializeField] private int maxHealth;
    [SerializeField] private int CoinsForSpawnSystem;
    private float currentHealth;
    private int pointValue;
    private bool loot;
    private AutoSpawner autoSpawner;
    private ComboCounter counter;
    public int MaxHealth => maxHealth;

    private void Start()
    {
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
            Destroy(this.gameObject);
        }
    }
    public void TakeDamage(float dmg)
    {
        currentHealth -= dmg;
    }
    public void HealDamage(float dmg)
    {
        if (currentHealth < maxHealth)
        {
            currentHealth += dmg;
        }
    }

}