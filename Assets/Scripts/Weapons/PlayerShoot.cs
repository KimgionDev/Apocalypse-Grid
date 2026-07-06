using System.Collections;
using UnityEngine;
using Unity.Cinemachine;

public class PlayerShoot : MonoBehaviour
{
    public PlayerStatsSO playerStats;
    public GunData gunData;
    public Transform firePoint;
    public Animator animator;
    public AudioClip shootSound;
    public AudioClip reloadSound;
    public CinemachineImpulseSource impulseSource;


    private int currentAmmo;
    private bool isReloading = false;
    private float nextFireTime = 0f;
    private bool isInitialized = false;
    private Coroutine reloadCoroutine;

    void Start()
    {
        currentAmmo = gunData.magSize;
        isInitialized = true;
        UpdateAmmoUI();
    }

    private void OnDisable()
    {
        isReloading = false;
    }

    private void OnEnable()
    {
        if (isInitialized)
        {
            UpdateAmmoUI();
        }
    }

    void Update()
    {
        if (isReloading)
        {
            if (gunData.reloadOneByOne && currentAmmo > 0 && Input.GetMouseButton(0) && Time.time >= nextFireTime)
            {
                if (reloadCoroutine != null)
                {
                    StopCoroutine(reloadCoroutine);
                }

                isReloading = false;
            }
            else
            {
                return;
            }
        }

        if (currentAmmo <= 0 || (Input.GetKeyDown(KeyCode.R) && currentAmmo < gunData.magSize))
        {
            reloadCoroutine = StartCoroutine(ReloadRoutine());
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
        UpdateAmmoUI();
        
        if (impulseSource != null)
        {
            impulseSource.GenerateImpulse(); 
        }
        
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

            if (bullet.TryGetComponent<Bullet>(out Bullet bulletScript))
            {
                bulletScript.Setup(totalDamage, gunData.bulletLifeTime);
            }
        }

        if (shootSound != null && AudioManager.Instance != null)
        {
            AudioManager.Instance.PlaySFX(shootSound);
        }
    }

    IEnumerator ReloadRoutine()
    {
        isReloading = true;
        ShowReloadingUI();
        
        float baseClipLength = 1f;
        float calculatedSpeed = baseClipLength / gunData.reloadTime;

        if (animator != null)
        {
            animator.SetFloat(AnimParams.ReloadSpeed, calculatedSpeed);
        }

        if (gunData.reloadOneByOne)
        {
            int ammoNeeded = gunData.magSize - currentAmmo;
            for (int i = 0; i < ammoNeeded; i++)
            {
                if (animator != null) animator.SetTrigger(AnimParams.Reload);
                if (reloadSound != null && AudioManager.Instance != null) AudioManager.Instance.PlaySFX(reloadSound);

                yield return new WaitForSeconds(gunData.reloadTime);
                currentAmmo++;
                UpdateAmmoUI();
            }
        }
        else
        {
            if (animator != null) animator.SetTrigger(AnimParams.Reload);
            if (reloadSound != null && AudioManager.Instance != null) AudioManager.Instance.PlaySFX(reloadSound);

            yield return new WaitForSeconds(gunData.reloadTime);
            currentAmmo = gunData.magSize;
            UpdateAmmoUI();
        }

        isReloading = false;
    }
    
    private void UpdateAmmoUI()
    {
        if (GameUIManager.Instance != null)
        {
            GameUIManager.Instance.UpdateAmmo(currentAmmo);
        }
    }

    private void ShowReloadingUI()
    {
        if (GameUIManager.Instance != null)
        {
            GameUIManager.Instance.ShowReloadingText();
        }
    }
}