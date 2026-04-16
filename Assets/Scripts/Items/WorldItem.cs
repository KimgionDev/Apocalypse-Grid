using UnityEngine;

public class WorldItem : MonoBehaviour
{
    public ItemData data;
    public SpriteRenderer spriteRenderer;

    public void SetupItem(ItemData itemData)
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

        if (data.isGold)
        {
            Debug.Log("Lụm được Vàng trị giá: " + data.value);
        }
        else if (MissionManager.Instance != null)
        {
            MissionManager.Instance.AddMissionItem(data, 1);
            Debug.Log("Lụm vật phẩm nhiệm vụ: " + data.itemName);
        }

        Destroy(gameObject);
    }
}