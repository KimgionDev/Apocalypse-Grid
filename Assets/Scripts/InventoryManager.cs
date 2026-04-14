using UnityEngine;
using System.Collections.Generic;

public class InventoryManager : MonoBehaviour
{
    public static InventoryManager Instance;

    public int gold;
    public Dictionary<string, int> missionItems = new Dictionary<string, int>();

    private void Awake()
    {
        Instance = this;
    }

    public void AddItem(ItemData data)
    {
        if (data.isGold)
        {
            gold += data.value;
            Debug.Log("Vàng hiện tại: " + gold);
        }
        else
        {
            if (missionItems.ContainsKey(data.itemName))
                missionItems[data.itemName] += 1;
            else
                missionItems.Add(data.itemName, 1);

            Debug.Log("Đã nhặt: " + data.itemName + ". Số lượng: " + missionItems[data.itemName]);
        }
    }
}