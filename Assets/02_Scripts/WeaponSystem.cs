using UnityEngine;

public class WeaponSystem : MonoBehaviour
{
    public int damage;
    public float cooldown, spread, range, reloadTime, fireRate;
    public int magazineSize, bulletsPerTap;
    public bool allowButtonHold;
    int bulletsLeft, bulletsShot;
    public GameObject BulletPrefab;

    bool shooting, readyToShoot, reloading;
    public Transform attackPoint;
    public RaycastHit raycastHit;
    public LayerMask layerMask;
    private void Awake()
    {
        bulletsLeft = magazineSize;
        readyToShoot = true;
    }
    private void Update()
    {
        TriggerPressed();
    }
    private void TriggerPressed()
    {
        if (allowButtonHold) shooting = Input.GetKey(KeyCode.Mouse0) || Input.GetAxis("Fire1")!=0;
        else shooting = Input.GetKeyDown(KeyCode.Mouse0) || Input.GetAxis("Fire1") != 0;

        if (Input.GetKeyDown(KeyCode.R) && bulletsLeft < magazineSize && reloading != true) Reload();

        if (readyToShoot == true && shooting == true && reloading != true && bulletsLeft> 0) { Shoot(); }
    }

    private void Reload() 
    {
        readyToShoot = true;
        Invoke("ReloadFinished", reloadTime);
    }
    private void Shoot()
    {
        readyToShoot = false;
        if (Physics.Raycast(attackPoint.position, attackPoint.forward, out raycastHit, range) && raycastHit.collider.CompareTag("Enemy"))
        {
            raycastHit.collider.GetComponent<HealthManager>().TakeDamage(damage);
        }
        HandleVisuals();
        bulletsLeft--;
        Invoke("ResetShot", cooldown);
    }
    private void ResetShot() { readyToShoot = true; }

    private void ReloadFinished()
    {
        bulletsLeft = magazineSize;
        reloading = false;
    }
    private void HandleVisuals()
    {
        GameObject newObject = Instantiate(BulletPrefab, attackPoint.position, attackPoint.rotation);
        Bullet bullet = newObject.GetComponent<Bullet>();
        bullet.Damage = damage;
        bullet.Range = range;
    }

    private void OnDrawGizmos()
        {
            Gizmos.color = Color.red;
            //Gizmos.DrawRay(attackPoint.position, attackPoint.forward * range);

    }
}
