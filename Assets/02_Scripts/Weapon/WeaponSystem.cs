using System;
using UnityEngine;
using UnityEngine.Serialization;

public class WeaponSystem : MonoBehaviour
{
    public Weapon[] weapons;
    private Transform[] attackPoints;
    private int damage = 1;
    private float fireRate, range, projectileSpeed, reuseTime = 0;
    public bool allowButtonHold = false;
    private string enemyType = string.Empty;
    private bool shooting = false;
    private GameObject bulletPrefab;
    // public RaycastHit raycastHit;
    // public LayerMask layerMask;
    private void Start()
    {
        enemyType = gameObject.CompareTag("Enemy") == true ? "Player" : "Enemy";
        shooting = gameObject.CompareTag("Enemy") == true ? true : false;
        reuseTime = Time.time;
        SwitchGun(0);
    }
    private void Update()
    {
        if (Time.timeScale != 0)
        {
            TriggerPressed();
        }
    }
    private void TriggerPressed()
    {
        if (enemyType == "Enemy")
        {
            if (allowButtonHold) shooting = Input.GetKey(KeyCode.Mouse0) || Input.GetAxis("Fire1") != 0;
            else shooting = Input.GetKeyDown(KeyCode.Mouse0) || Input.GetAxis("Fire1") != 0;
        }
        if (shooting == true && Time.time>reuseTime) { Shoot(); }
    }
    private void Shoot()
    {
        HandleVisuals(); 
        reuseTime = Time.time+fireRate;
        //if (Physics.Raycast(attackPoint.position, attackPoint.forward, out raycastHit, range) && raycastHit.collider.CompareTag(enemyType))
        //{
        //    raycastHit.collider.GetComponent<HealthManager>().TakeDamage(damage);
        //}
    }
    private void HandleVisuals()
    {   foreach (var attackPoint in attackPoints)
        {
            Bullet bullet = Instantiate(bulletPrefab, attackPoint.position, attackPoint.rotation).GetComponent<Bullet>();
            bullet.TargetTag = enemyType;
            bullet.Damage = damage;
            bullet.Range = range;
            bullet.Speed = projectileSpeed;
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
}