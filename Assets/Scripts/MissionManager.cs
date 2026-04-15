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

    [Header("Tiến trình Ground")]
    [SerializeField] private int currentGround = 1;
    [SerializeField] private int baseTypesCount = 2;
    [SerializeField] private int maxTypesCount = 5;
    [SerializeField] private int baseMaxAmount = 6;
    [SerializeField] private int amountIncreasePerGround = 2;
    [SerializeField] private int groundStepToIncreaseType = 2;

    public List<MissionRequirement> currentMission = new List<MissionRequirement>();

    private bool isMissionCompleted;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        GenerateMissionForCurrentGround();
    }

    public void GenerateMissionForCurrentGround()
    {
        isMissionCompleted = false;

        List<ItemData> missionCandidates = new List<ItemData>();
        foreach (ItemData item in allItemsDatabase)
        {
            if (item != null && !item.isGold)
            {
                missionCandidates.Add(item);
            }
        }

        if (missionCandidates.Count == 0)
        {
            currentMission.Clear();
            RefreshUI();
            Debug.LogWarning("Không có item nhiệm vụ hợp lệ (non-gold) trong allItemsDatabase.");
            return;
        }

        int typesCount = baseTypesCount + ((currentGround - 1) / Mathf.Max(1, groundStepToIncreaseType));
        typesCount = Mathf.Clamp(typesCount, 1, Mathf.Min(maxTypesCount, missionCandidates.Count));

        int maxAmount = baseMaxAmount + ((currentGround - 1) * amountIncreasePerGround);

        GenerateRandomMission(typesCount, maxAmount, missionCandidates);
    }

    private void GenerateRandomMission(int typesCount, int maxAmount, List<ItemData> candidateItems)
    {
        currentMission.Clear();
        List<ItemData> tempList = new List<ItemData>(candidateItems);

        for (int i = 0; i < typesCount; i++)
        {
            if (tempList.Count == 0) break;

            int randIndex = Random.Range(0, tempList.Count);
            ItemData picked = tempList[randIndex];

            currentMission.Add(new MissionRequirement
            {
                item = picked,
                current = 0,
                target = Random.Range(3, maxAmount + 1)
            });

            tempList.RemoveAt(randIndex);
        }

        RefreshUI();
    }

    public void AddMissionItem(ItemData collectedItem, int amount = 1)
    {
        if (collectedItem == null || amount <= 0 || currentMission.Count == 0)
        {
            return;
        }

        bool isMissionItem = false;

        foreach (MissionRequirement req in currentMission)
        {
            if (req.item == null) continue;

            bool isMatch = req.item == collectedItem || req.item.itemName == collectedItem.itemName;
            if (isMatch && req.current < req.target)
            {
                req.current = Mathf.Min(req.current + amount, req.target);
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

    private void RefreshUI()
    {
        if (missionUIContainer == null || missionSlotPrefab == null) return;

        foreach (Transform child in missionUIContainer)
        {
            Destroy(child.gameObject);
        }

        foreach (MissionRequirement req in currentMission)
        {
            GameObject slot = Instantiate(missionSlotPrefab, missionUIContainer);
            MissionSlotUI slotUI = slot.GetComponent<MissionSlotUI>();
            if (slotUI != null && req.item != null)
            {
                slotUI.Setup(req.item.icon, req.current, req.target);
            }
        }
    }

    private void CheckWin()
    {
        if (isMissionCompleted) return;

        foreach (MissionRequirement req in currentMission)
        {
            if (req.current < req.target) return;
        }

        isMissionCompleted = true;
        Debug.Log("ĐÃ NHẶT ĐỦ TẤT CẢ VẬT PHẨM! MỞ CỔNG!");
    }

    public void CompleteGroundAndGenerateNextMission()
    {
        currentGround++;
        GenerateMissionForCurrentGround();
    }
}