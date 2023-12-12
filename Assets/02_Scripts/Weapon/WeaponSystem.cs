using System;
using UnityEditor;
using UnityEngine;
public class WeaponSystem : MonoBehaviour
{
    [SerializeField] private Weapon[] Weapons;
    public int damage;
    public float cooldown, range,  fireRate, projectileSpeed;
    public bool allowButtonHold;
    public GameObject BulletPrefab;
    string enemyType = string.Empty;
    bool shooting, readyToShoot, reloading;
    public Transform[] attackPoints;
    public RaycastHit raycastHit;
    public LayerMask layerMask;
    private void Start()
    {
        readyToShoot = true;
        if (gameObject.CompareTag("Enemy") == true) { enemyType = "Player"; }
        else { enemyType = "Enemy"; }
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
        //if (Physics.Raycast(attackPoint.position, attackPoint.forward, out raycastHit, range) && raycastHit.collider.CompareTag(enemyType))
        //{
        //    raycastHit.collider.GetComponent<HealthManager>().TakeDamage(damage);
        //}
        HandleVisuals();
        Invoke("ResetShot", cooldown);
    }
    private void ResetShot() { readyToShoot = true; }

    private void HandleVisuals()
    {
        //GameObject newObject = Instantiate(BulletPrefab, attackPoint.position, attackPoint.rotation);
        foreach (var attackPoint in attackPoints)
        {
            Bullet bullet = Instantiate(BulletPrefab, attackPoint.position, attackPoint.rotation).GetComponent<Bullet>();
            bullet.TargetTag = enemyType;
            bullet.Damage = damage;
            bullet.Range = range;
            bullet.Speed = projectileSpeed;
        }
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