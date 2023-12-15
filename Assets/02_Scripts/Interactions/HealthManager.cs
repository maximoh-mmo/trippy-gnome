using UnityEngine;

public class HealthManager : MonoBehaviour
{
    [SerializeField] int maxHealth;
    float currentHealth;
    [SerializeField] int pointValue;
    ComboCounter counter;
    //bool enemy = false;
    public float CurrentHealth {  get { return currentHealth; } }
    private void Start()
    {
        currentHealth = maxHealth;
        pointValue = maxHealth;
        counter = FindObjectOfType<ComboCounter>();
    //    enemy = CompareTag("Enemy");
    }
    private void Update()
    {
        if (currentHealth < 1)
        {
            counter.AddKill(pointValue);
            GetComponent<Loot>().GetLoot(transform.position);
            Destroy(this.gameObject);
        }
    }
    public void TakeDamage(float dmg)
    {
        //if (enemy==false) { counter.ImHit(); }
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