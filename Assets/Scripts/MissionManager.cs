using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
public class MissionRequirement
{
    public ItemData item;
    public int current;
    public int target;
}

public class MissionManager : MonoBehaviour
{
    public static MissionManager Instance;

    [Header("Kho Dữ Liệu")]
    public List<ItemData> allItemsDatabase;

    [Header("UI Nhiệm Vụ")]
    public Transform missionUIContainer;
    public GameObject missionSlotPrefab;

    public List<MissionRequirement> currentMission = new List<MissionRequirement>();

    private void Awake() => Instance = this;

    private void Start()
    {
        GenerateRandomMission(2, 10);
    }

    // Hàm tự động tạo nhiệm vụ ngẫu nhiên
    public void GenerateRandomMission(int typesCount, int maxAmount)
    {
        currentMission.Clear();
        List<ItemData> tempList = new List<ItemData>(allItemsDatabase);

        for (int i = 0; i < typesCount; i++)
        {
            if (tempList.Count == 0) break;

            int randIndex = Random.Range(0, tempList.Count);

            currentMission.Add(new MissionRequirement
            {
                item = tempList[randIndex],
                current = 0,
                target = Random.Range(3, maxAmount + 1)
            });

            tempList.RemoveAt(randIndex);
        }
        RefreshUI();
    }

    public void AddMissionItem(ItemData collectedItem, int amount = 1)
    {
        bool isMissionItem = false;
        foreach (var req in currentMission)
        {
            if (req.item == collectedItem && req.current < req.target)
            {
                req.current += amount;
                isMissionItem = true;
                break;
            }
        }

        if (isMissionItem)
        {
            RefreshUI();
            CheckWin();
        }
    }

    void RefreshUI()
    {
        foreach (Transform child in missionUIContainer) Destroy(child.gameObject);

        foreach (var req in currentMission)
        {
            GameObject slot = Instantiate(missionSlotPrefab, missionUIContainer);
            slot.GetComponent<MissionSlotUI>().Setup(req.item.icon, req.current, req.target);
        }
    }

    void CheckWin()
    {
        foreach (var req in currentMission)
        {
            if (req.current < req.target) return;
        }
        Debug.Log("ĐÃ NHẶT ĐỦ TẤT CẢ VẬT PHẨM! MỞ CỔNG!");
    }
}