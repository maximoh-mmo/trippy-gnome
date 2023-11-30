using UnityEngine;

public class HealthManager : MonoBehaviour
{
    [SerializeField] float startingHealth;
    [SerializeField] float maxHealth;
    [SerializeField] float currentHealth;
    public float CurrentHealth {  get { return currentHealth; } }
    private void Update()
    {
        if (currentHealth < 1)
        {
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