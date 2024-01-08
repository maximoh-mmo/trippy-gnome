using UnityEngine;

public class HealthManager : MonoBehaviour
{
    [SerializeField] int maxHealth;
    private float currentHealth;
    private int pointValue;
    ComboCounter counter;
    public float CurrentHealth {  get { return currentHealth; } }
    private void Start()
    {
        currentHealth = maxHealth;
        counter = FindObjectOfType<ComboCounter>();
    }
    private void Update()
    {
        if (currentHealth < 1)
        {
            counter.AddKill(maxHealth);
            GetComponent<Loot>().GetLoot(transform.position);
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