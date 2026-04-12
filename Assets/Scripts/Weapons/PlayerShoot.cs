using System.Collections;
using UnityEngine;

public class PlayerShoot : MonoBehaviour
{
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

    void Shoot()        // Bắn nhiều viên đạn cùng lúc với góc ngẫu nhiên dựa trên spreadAngle
    {
        currentAmmo--;

        for (int i = 0; i < gunData.bulletCount; i++)
        {
            float randomSpread = Random.Range(-gunData.spreadAngle, gunData.spreadAngle);
            Quaternion bulletRotation = firePoint.rotation * Quaternion.Euler(0, 0, randomSpread);
            Instantiate(gunData.bulletPrefab, firePoint.position, bulletRotation);
        }
    }

    IEnumerator ReloadRoutine()
    {
        isReloading = true;

        float baseClipLength = 1f;  // Giả sử độ dài cơ bản của clip là 1 giây

        float calculatedSpeed = baseClipLength / gunData.reloadTime;    // Tính toán tốc độ reload dựa trên thời gian reload của súng

        if (animator != null)
        {
            animator.SetFloat("ReloadSpeed", calculatedSpeed);  // Cập nhật thông số tốc độ reload trong Animator để đồng bộ với thời gian reload của súng
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