using UnityEngine;
using System.Collections.Generic;

public class InventoryManager : MonoBehaviour
{
    public static InventoryManager Instance;

    public int gold;
    public Dictionary<string, int> missionItems = new Dictionary<string, int>();

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
        }
    }

    public void AddItem(DropItemData data)
    {
        if (data.isGold)
        {
            gold += data.value;
            Debug.Log("Vàng hiện tại: " + gold);
        }
        else
        {
            gold += data.value;
            if (missionItems.ContainsKey(data.itemName))
                missionItems[data.itemName] += 1;
            else
                missionItems.Add(data.itemName, 1);
            
            if (MissionManager.Instance != null)
            {
                MissionManager.Instance.AddMissionItem(data, 1);
            }
        }
    }
}