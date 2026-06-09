using System.Collections;
using UnityEngine;

public class PlayerShoot : MonoBehaviour
{
    public PlayerStatsSO playerStats;

    public GunData gunData;
    public Transform firePoint;
    public Animator animator;

    private int currentAmmo;
    private bool isReloading = false;
    private float nextFireTime = 0f;

    void Start()
    {
        currentAmmo = gunData.magSize;
    }
    
    private void OnDisable()
    {
        isReloading = false;
    }

    void Update()
    {
        if (isReloading) return;

        if (currentAmmo <= 0 || (Input.GetKeyDown(KeyCode.R) && currentAmmo < gunData.magSize))
        {
            StartCoroutine(ReloadRoutine());
            return;
        }

        if (Input.GetMouseButton(0) && Time.time >= nextFireTime)
        {
            Shoot();
            nextFireTime = Time.time + gunData.fireRate;
        }
    }

    void Shoot()
    {
        currentAmmo--;

        if (gunData.muzzleFlashPrefab != null)
        {
            GameObject flash = Instantiate(gunData.muzzleFlashPrefab, firePoint.position, firePoint.rotation, firePoint);
            flash.transform.localScale = new Vector3(gunData.flashScale, gunData.flashScale, 1f);
        }

        float totalDamage = gunData.bulletDamage;
        if (playerStats != null)
        {
            totalDamage += playerStats.baseDamage; 
        }

        for (int i = 0; i < gunData.bulletCount; i++)
        {
            float randomSpread = Random.Range(-gunData.spreadAngle, gunData.spreadAngle);
            Quaternion bulletRotation = firePoint.rotation * Quaternion.Euler(0, 0, randomSpread);
            GameObject bullet = Instantiate(gunData.bulletPrefab, firePoint.position, bulletRotation);
            
            bullet.GetComponent<Bullet>().Setup(totalDamage, gunData.bulletLifeTime);
        }
    }

    IEnumerator ReloadRoutine()
    {
        isReloading = true;
        float baseClipLength = 1f;  
        float calculatedSpeed = baseClipLength / gunData.reloadTime;    

        if (animator != null)
        {
            animator.SetFloat("ReloadSpeed", calculatedSpeed);
        }

        if (gunData.reloadOneByOne)
        {
            int ammoNeeded = gunData.magSize - currentAmmo;
            for (int i = 0; i < ammoNeeded; i++)
            {
                if (animator != null) animator.SetTrigger("Reload");
                yield return new WaitForSeconds(gunData.reloadTime);
                currentAmmo++;
            }
        }
        else
        {
            if (animator != null) animator.SetTrigger("Reload");
            yield return new WaitForSeconds(gunData.reloadTime);
            currentAmmo = gunData.magSize;
        }

        isReloading = false;
    }
}