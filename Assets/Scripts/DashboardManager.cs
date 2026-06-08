using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class DashboardManager : MonoBehaviour
{
    [Header("Dữ liệu Người chơi")]
    public PlayerStatsSO playerStats;

    [Header("Hiển thị Tổng Tiền")]
    public TextMeshProUGUI totalGoldText;

    [Header("Nâng cấp Máu Tối Đa (Max Health)")]
    public TextMeshProUGUI healthStatText;
    public TextMeshProUGUI healthCostText;
    public Button healthUpgradeBtn;
    public float healthBaseValue = 100f;
    public float healthIncreaseStep = 20f;
    public int healthBaseCost = 50;
    [Header("Nâng cấp Sát Thương (Base Damage)")]
    public TextMeshProUGUI damageStatText;
    public TextMeshProUGUI damageCostText;
    public Button damageUpgradeBtn;
    public float damageBaseValue = 10f;
    public float damageIncreaseStep = 5f; 
    public int damageBaseCost = 100; 

    private void Start()
    {
        RefreshUI();
    }

    public void RefreshUI()
    {
        if (playerStats == null) return;

        // 1. Hiển thị tổng tiền
        totalGoldText.text = "Vàng: " + playerStats.totalGold.ToString();

        // 2. Tính toán lạm phát chi phí (BR-02)
        int currentHealthCost = CalculateCost(healthBaseCost, playerStats.maxHealth, healthBaseValue, healthIncreaseStep);
        int currentDamageCost = CalculateCost(damageBaseCost, playerStats.baseDamage, damageBaseValue, damageIncreaseStep);

        // 3. Cập nhật Text hiển thị chỉ số và giá tiền
        healthStatText.text = "Máu: " + playerStats.maxHealth;
        healthCostText.text = "Giá: " + currentHealthCost + " Vàng";

        damageStatText.text = "Sát thương: " + playerStats.baseDamage;
        damageCostText.text = "Giá: " + currentDamageCost + " Vàng";

        // 4. Bật/Tắt nút bấm nếu không đủ tiền (REQ-DB-03)
        healthUpgradeBtn.interactable = playerStats.totalGold >= currentHealthCost;
        damageUpgradeBtn.interactable = playerStats.totalGold >= currentDamageCost;
    }

    private int CalculateCost(int baseCost, float currentStat, float baseStatValue, float stepAmount)
    {
        int upgradeCount = Mathf.FloorToInt((currentStat - baseStatValue) / stepAmount);
        // Công thức: Giá hiện tại = Giá gốc + (Giá gốc * 0.5 * Số lần đã nâng)
        return baseCost + Mathf.FloorToInt(baseCost * 0.5f * upgradeCount);
    }

    // --- CÁC HÀM GẮN VÀO SỰ KIỆN ONCLICK CỦA NÚT BẤM ---

    public void OnClickUpgradeHealth()
    {
        int cost = CalculateCost(healthBaseCost, playerStats.maxHealth, healthBaseValue, healthIncreaseStep);
        if (playerStats.totalGold >= cost)
        {
            playerStats.totalGold -= cost; 
            playerStats.maxHealth += healthIncreaseStep; 
            SaveDataAndUpdate();
        }
    }

    public void OnClickUpgradeDamage()
    {
        int cost = CalculateCost(damageBaseCost, playerStats.baseDamage, damageBaseValue, damageIncreaseStep);
        if (playerStats.totalGold >= cost)
        {
            playerStats.totalGold -= cost; 
            playerStats.baseDamage += damageIncreaseStep; 
            SaveDataAndUpdate();
        }
    }

    private void SaveDataAndUpdate()
    {
        if (SaveManager.Instance != null)
        {
            SaveManager.Instance.SaveGame();
        }
        RefreshUI(); 
    }
}