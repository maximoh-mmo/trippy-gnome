using UnityEngine;

public class HealthManager : MonoBehaviour
{
    [SerializeField] int startingHealth;
    [SerializeField] int maxHealth;
    [SerializeField] int currentHealth;

    private void Update()
    {
        if (currentHealth < 1)
        {
            Destroy(gameObject);
        }
    }
    public void TakeDamage(int dmg)
    {
        currentHealth -= dmg;
    }
    public void HealDamage(int dmg)
    {
        if (currentHealth < maxHealth)
        {
            currentHealth += dmg;
        }
    }
}