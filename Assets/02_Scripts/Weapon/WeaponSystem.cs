using System;
using UnityEngine;

public class WeaponSystem : MonoBehaviour
{
    public Weapon[] Weapons;
    private int damage = 1;
    public float fireRate;
    public float range, projectileSpeed;
    public bool allowButtonHold;
    [SerializeField] GameObject BulletPrefab;
    private string enemyType = string.Empty;
    private bool shooting, readyToShoot, reloading;
    public Transform[] attackPoints;
    public RaycastHit raycastHit;
    public LayerMask layerMask;
    private void Start()
    {
        readyToShoot = true;
        enemyType = gameObject.CompareTag("Enemy") == true ? "Player" : "Enemy";
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
        if (allowButtonHold) shooting = Input.GetKey(KeyCode.Mouse0) || Input.GetAxis("Fire1") != 0;
        else shooting = Input.GetKeyDown(KeyCode.Mouse0) || Input.GetAxis("Fire1") != 0;
        if (gameObject.CompareTag("Enemy")==true) shooting = true;
        if (readyToShoot == true && shooting == true) { Shoot(); }
    }
    private void Shoot()
    {
        readyToShoot = false;
        HandleVisuals();
        Invoke("ResetShot", fireRate);
        
        //if (Physics.Raycast(attackPoint.position, attackPoint.forward, out raycastHit, range) && raycastHit.collider.CompareTag(enemyType))
        //{
        //    raycastHit.collider.GetComponent<HealthManager>().TakeDamage(damage);
        //}
    }
    private void ResetShot() { readyToShoot = true; }

    private void HandleVisuals()
    {   foreach (var attackPoint in attackPoints)
        {
            Bullet bullet = Instantiate(BulletPrefab, attackPoint.position, attackPoint.rotation).GetComponent<Bullet>();
            Debug.Log(bullet);
            bullet.TargetTag = enemyType;
            bullet.Damage = damage;
            bullet.Range = range;
            bullet.Speed = projectileSpeed;
        }
    }
    public void SwitchGun(int id)
    {
        damage = Weapons[id].damage;
        BulletPrefab = Weapons[id].projectile;
        attackPoints = Weapons[id].firePoints;
    }
}
[Serializable]
public class Weapon
{
    public string weaponName;
    public int damage;
    public Transform[] firePoints;
    public GameObject projectile;
}