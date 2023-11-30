using UnityEngine;

public class HealthManager : MonoBehaviour
{
    [SerializeField] float startingHealth;
    [SerializeField] float maxHealth;
    [SerializeField] float currentHealth;
    [SerializeField] int pointValue;
    ComboCounter counter;
    bool enemy = false;
    public float CurrentHealth {  get { return currentHealth; } }
    private void Start()
    {
        counter = FindObjectOfType<ComboCounter>();
        enemy = CompareTag("Enemy");
    }
    private void Update()
    {
        if (currentHealth < 1)
        {
            if (enemy==true) { counter.AddKill(pointValue); }
            Destroy(this.gameObject);
        }
    }
    public void TakeDamage(float dmg)
    {
        if (enemy==false) { counter.ImHit(); }
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