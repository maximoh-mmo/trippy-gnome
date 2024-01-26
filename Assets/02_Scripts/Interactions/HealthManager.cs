using UnityEngine;


public class HealthManager : MonoBehaviour
{
    [SerializeField] private int maxHealth;
    [SerializeField] private int CoinsForSpawnSystem;
    private MeshRenderer[] materials;
    private float currentHealth, currentDelay;
    private int pointValue;
    private bool loot, bright;
    [SerializeField] private bool lootDropOnBoom = true;
    private AutoSpawner autoSpawner;
    private ComboCounter counter;
    private GameObject poof;
    private float intensity = 1f;
    private Color[] colours = { Color.blue, Color.green, Color.red, Color.yellow, Color.cyan, Color.magenta };
    private static readonly int EmissionColor = Shader.PropertyToID("_EmissionColor");

    public int MaxHealth => maxHealth;

    private void Start()
    {
        materials = GetComponentsInChildren<MeshRenderer>();
       
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
        if (currentDelay < Time.unscaledTime && bright)
        {
            foreach (var m in materials)
            {
                m.material.DisableKeyword("_EMISSION");
            }
            bright = false;
        }
        if (!(currentHealth < 1)) return;
        counter.AddKill(maxHealth);
        autoSpawner.AddMoney(CoinsForSpawnSystem);
        if (loot) GetComponent<Loot>().GetLoot(transform.position);
        if (poof) Instantiate(poof, transform.position, Quaternion.identity);
        Destroy(gameObject);
    }

    public void TakeDamage(float dmg)
    {
        var c = GetColor();
        foreach (var m in materials)
        {
            m.material.SetColor(EmissionColor, c * intensity);
            m.material.EnableKeyword("_EMISSION");
        }
        bright = true;
        currentDelay = Time.unscaledTime + 0.05f;
        currentHealth -= dmg;
    }
    private Color GetColor()
    {
        return colours[Random.Range(0, colours.Length)];
    }public void Kill()
    {
        counter.AddKill(maxHealth);
        if (loot && lootDropOnBoom) GetComponent<Loot>().GetLoot(transform.position);
        if (poof) Instantiate(poof, transform.position, Quaternion.identity);
        Destroy(gameObject);
    }
}