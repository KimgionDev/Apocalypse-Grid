using System;
using UnityEngine;

public class WorldItem : MonoBehaviour
{
    public float lifetime = 10f;
    private Coroutine lifetimeCoroutine;
    public DropItemData data;
    public SpriteRenderer spriteRenderer;

    public void SetupItem(DropItemData itemData)
    {
        data = itemData;
        if (spriteRenderer != null)
        {
            spriteRenderer.sprite = data.icon;
        }
    }

    private void OnEnable()
    {
        if (lifetimeCoroutine != null) StopCoroutine(lifetimeCoroutine);
        lifetimeCoroutine = StartCoroutine(ReturnToPoolAfterTime());
    }

    private System.Collections.IEnumerator ReturnToPoolAfterTime()
    {
        yield return new WaitForSeconds(lifetime);
        ObjectPoolManager.Return(gameObject);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!collision.CompareTag("Player"))
        {
            return;
        }

        if (data.isHealItem)
        {
            if (collision.TryGetComponent<PlayerHealth>(out PlayerHealth health))
            {
                health.Heal(data.healAmount);
            }
        }
        
        if (data.isAmmoItem)
        {
            if (SaveManager.Instance != null && SaveManager.Instance.playerStats != null)
            {
                SaveManager.Instance.playerStats.AddAmmo(data.ammoType, data.ammoAmount);
            }
            
            PlayerShoot[] shoots = collision.GetComponentsInChildren<PlayerShoot>(true);
            foreach (PlayerShoot shoot in shoots)
            {
                if (shoot.gameObject.activeInHierarchy)
                {
                    shoot.ForceUpdateAmmoUI();
                }
            }
        }
        
        if (InventoryManager.Instance == null)
        {
            Debug.LogWarning("Không tìm thấy InventoryManager trong Scene!");
            return;
        }

        InventoryManager.Instance.AddItem(data);

        if (data.pickupSound != null && AudioManager.Instance != null)
        {
            AudioManager.Instance.PlaySFX(data.pickupSound);
        }

        ObjectPoolManager.Return(gameObject);
    }
}