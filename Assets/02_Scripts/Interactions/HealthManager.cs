using UnityEngine;

public class HealthManager : MonoBehaviour
{
    [SerializeField] private int maxHealth;
    private float currentHealth;
    private int pointValue;
    private bool loot;
    ComboCounter counter;
    public int MaxHealth => maxHealth;

    private void Start()
    {
        currentHealth = maxHealth;
        counter = FindObjectOfType<ComboCounter>();
        loot = GetComponent<Loot>();
    }
    private void Update()
    {
        if (currentHealth < 1)
        {
            counter.AddKill(maxHealth);
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