using UnityEngine;

public class WorldItem : MonoBehaviour
{
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

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!collision.CompareTag("Player"))
        {
            return;
        }

        if (InventoryManager.Instance != null)
        {
            InventoryManager.Instance.AddItem(data);
        }
        else
        {
            Debug.LogWarning("Không tìm thấy InventoryManager trong Scene!");
        }

        Destroy(gameObject);
    }
}