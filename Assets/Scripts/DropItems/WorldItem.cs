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
        if (ObjectPoolManager.Instance != null) ObjectPoolManager.Instance.ReturnToPool(gameObject);
        else Destroy(gameObject);
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
        
        if (InventoryManager.Instance != null)
        {
            InventoryManager.Instance.AddItem(data);
        }
        else
        {
            Debug.LogWarning("Không tìm thấy InventoryManager trong Scene!");
        }

        if (data.pickupSound != null && AudioManager.Instance != null)
        {
            AudioManager.Instance.PlaySFX(data.pickupSound);
        }

        if (ObjectPoolManager.Instance != null) ObjectPoolManager.Instance.ReturnToPool(gameObject);
        else Destroy(gameObject);
    }
}