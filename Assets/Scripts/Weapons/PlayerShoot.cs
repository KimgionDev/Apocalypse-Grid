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
                UpdateAmmoUI();
            }
            else
            {
                return;
            }
        }

        bool canReload = gunData.ammoType == AmmoType.Infinite || (playerStats != null && playerStats.GetAmmo(gunData.ammoType) > 0);

        if ((currentAmmo <= 0 || Input.GetKeyDown(KeyCode.R)) && currentAmmo < gunData.magSize)
        {
            if (canReload)
            {
                reloadCoroutine = StartCoroutine(ReloadRoutine());
                return;
            }
        }

        if (Input.GetMouseButton(0) && Time.time >= nextFireTime && currentAmmo > 0)
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
            GameObject flash = ObjectPoolManager.Spawn(gunData.muzzleFlashPrefab, firePoint.position, firePoint.rotation);
            flash.transform.SetParent(firePoint);
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
            GameObject bullet = ObjectPoolManager.Spawn(gunData.bulletPrefab, firePoint.position, bulletRotation);

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

        if (gunData.ammoType == AmmoType.Infinite)
        {
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
        }
        else
        {
            int reserveAmmo = playerStats != null ? playerStats.GetAmmo(gunData.ammoType) : 0;
            if (reserveAmmo > 0)
            {
                if (gunData.reloadOneByOne)
                {
                    int ammoNeeded = gunData.magSize - currentAmmo;
                    for (int i = 0; i < ammoNeeded; i++)
                    {
                        if (playerStats.GetAmmo(gunData.ammoType) <= 0) break;

                        if (animator != null) animator.SetTrigger(AnimParams.Reload);
                        if (reloadSound != null && AudioManager.Instance != null) AudioManager.Instance.PlaySFX(reloadSound);

                        yield return new WaitForSeconds(gunData.reloadTime);
                        playerStats.ConsumeAmmo(gunData.ammoType, 1);
                        currentAmmo++;
                        UpdateAmmoUI();
                    }
                }
                else
                {
                    int ammoNeeded = gunData.magSize - currentAmmo;
                    int ammoToLoad = Mathf.Min(ammoNeeded, reserveAmmo);

                    if (animator != null) animator.SetTrigger(AnimParams.Reload);
                    if (reloadSound != null && AudioManager.Instance != null) AudioManager.Instance.PlaySFX(reloadSound);

                    yield return new WaitForSeconds(gunData.reloadTime);
                    playerStats.ConsumeAmmo(gunData.ammoType, ammoToLoad);
                    currentAmmo += ammoToLoad;
                    UpdateAmmoUI();
                }
            }
        }

        isReloading = false;
    }
    
    private void UpdateAmmoUI()
    {
        if (GameUIManager.Instance != null)
        {
            if (gunData.ammoType == AmmoType.Infinite)
            {
                GameUIManager.Instance.UpdateAmmoText($"{currentAmmo} / ∞");
            }
            else
            {
                int reserveAmmo = playerStats != null ? playerStats.GetAmmo(gunData.ammoType) : 0;
                GameUIManager.Instance.UpdateAmmoText($"{currentAmmo} / {reserveAmmo}");
            }
        }
    }

    public void ForceUpdateAmmoUI()
    {
        UpdateAmmoUI();
    }

    private void ShowReloadingUI()
    {
        if (GameUIManager.Instance != null)
        {
            GameUIManager.Instance.ShowReloadingText();
        }
    }
}