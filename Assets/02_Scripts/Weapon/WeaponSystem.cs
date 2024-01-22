using System;
using UnityEngine;

public class WeaponSystem : MonoBehaviour
{
    private PlayerInputSystem playerInputSystem;
    public Weapon[] weapons;
    private Transform[] attackPoints;
    private int damage = 1;
    private float fireRate, range, projectileSpeed, reuseTime;
    private string enemyType = string.Empty;
    private bool shooting, allowButtonHold, psychoRush;
    private GameObject bulletPrefab;
    private GetNearestTarget getNearestTarget;
    private ComboCounter cc;

    public bool PsychoRush { set => psychoRush = value; }

    // public RaycastHit raycastHit;
    // public LayerMask layerMask;
    private void Awake()
    {
        getNearestTarget = FindFirstObjectByType<GetNearestTarget>();
        playerInputSystem = new PlayerInputSystem();
        playerInputSystem.InGame.Enable();
        cc = GetComponent<ComboCounter>();
    }

    private void Start()
    {
        enemyType = gameObject.CompareTag("Enemy") ? "Player" : "Enemy";
        shooting = gameObject.CompareTag("Enemy");
        reuseTime = Time.time;
        SwitchGun(0);
    }
    private void Update()
    {
        if (Time.timeScale == 0 || enemyType == "Player") return; 
        if (psychoRush) return;
        if (allowButtonHold == false)
        {
            if (playerInputSystem.FindAction("Fire").WasPressedThisFrame()) Shoot();
            return;
        }
        if (playerInputSystem.FindAction("Fire").IsPressed()) Shoot(); 
    }
    public void Shoot()
    {
        if (!(Time.time>=reuseTime)) return;
        HandleVisuals();
        reuseTime = Time.time+fireRate;
    }
   
    // Raycast Shoot
    //if (Physics.Raycast(attackPoint.position, attackPoint.forward, out raycastHit, range) && raycastHit.collider.CompareTag(enemyType))
    //{
    //    raycastHit.collider.GetComponent<HealthManager>().TakeDamage(damage);
    //}
    
    private void HandleVisuals()
    {   
        foreach (var attackPoint in attackPoints)
        {
            if (!gameObject) return;
            if (attackPoints == null) return;
            var projectile = Instantiate(bulletPrefab, attackPoint.position, attackPoint.rotation); 
            if (projectile.TryGetComponent<Bullet>(out var bullet))
            {
                if (bullet)
                {
                    bullet.Setup(enemyType, range, projectileSpeed, damage);
                    if (cc) cc.ShotFired = 1;
                }
            }
            
            if (projectile.TryGetComponent<Rocket>(out var rocket))
            {
                if (rocket)
                {
                    rocket.Setup(enemyType, range, projectileSpeed, damage);
                    if (getNearestTarget.GetTarget()) rocket.AddTarget(getNearestTarget.GetTarget());
                    if (cc) cc.ShotFired = 1;
                }
            }
        }
    }

    public void SwitchGun(int id)
    {
        damage = weapons[id].damage;
        bulletPrefab = weapons[id].projectile;
        attackPoints = weapons[id].firePoints;
        projectileSpeed = weapons[id].speed;
        fireRate = weapons[id].rateOfFire;
        range = weapons[id].range;
        attackPoints = weapons[id].firePoints;
        allowButtonHold = weapons[id].allowButtonHold;
    }
}

[Serializable]
public class Weapon
{
    public string weaponName;
    public int damage;
    public Transform[] firePoints;
    public GameObject projectile;
    public float speed, rateOfFire, range;
    public bool allowButtonHold;
}