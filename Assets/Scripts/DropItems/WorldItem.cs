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

        if (data.isGold)
        {
        }
        else if (MissionManager.Instance != null)
        {
            MissionManager.Instance.AddMissionItem(data, 1);
        }

        Destroy(gameObject);
    }
}