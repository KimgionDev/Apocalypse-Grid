using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class DashboardManager : MonoBehaviour
{
    [Header("Dữ liệu Người chơi")] public PlayerStatsSO playerStats;

    [Header("Cụm Player Status (Góc phải trên)")]
    public TextMeshProUGUI txtCurrentGold;

    public TextMeshProUGUI txtCurrentHP;
    public TextMeshProUGUI txtCurrentDamage;
    public TextMeshProUGUI txtCurrentSpeed;
    public TextMeshProUGUI txtCurrentLevel;

    [Header("Thẻ Máu (Health Card)")] public TextMeshProUGUI txtPriceHealth;
    public Button btnUpgradeHealth;
    public float healthBaseValue = 100f;
    public float healthIncreaseStep = 20f;
    public int healthBaseCost = 50;

    [Header("Thẻ Sát Thương (Damage Card)")]
    public TextMeshProUGUI txtPriceDamage;

    public Button btnUpgradeDamage;
    public float damageBaseValue = 10f;
    public float damageIncreaseStep = 5f;
    public int damageBaseCost = 50;

    [Header("Thẻ Tốc Độ (Speed Card)")] public TextMeshProUGUI txtPriceSpeed;
    public Button btnUpgradeSpeed;
    public float speedBaseValue = 5f;
    public float speedIncreaseStep = 1f;
    public int speedBaseCost = 50;

    private void Start()
    {
        RefreshUI();
    }
    
    public void OnClickStartRun()
    {
        if (SaveManager.Instance != null)
        {
            SaveManager.Instance.SaveGame();
        }
        
        SceneManager.LoadScene("PlayScene"); 
    }

    public void OnClickBack()
    {
        SceneManager.LoadScene("MenuScene"); 
    }

    public void RefreshUI()
    {
        if (playerStats == null) return;

        txtCurrentGold.text = "Gold: " + playerStats.totalGold;
        txtCurrentHP.text = "HP: " + playerStats.maxHealth;
        txtCurrentDamage.text = "Damage: " + playerStats.baseDamage;
        txtCurrentSpeed.text = "Speed: " + playerStats.moveSpeed;
        txtCurrentLevel.text = "Level: " + playerStats.currentLevel;

        int currentHealthCost =
            CalculateCost(healthBaseCost, playerStats.maxHealth, healthBaseValue, healthIncreaseStep);
        int currentDamageCost =
            CalculateCost(damageBaseCost, playerStats.baseDamage, damageBaseValue, damageIncreaseStep);
        int currentSpeedCost = CalculateCost(speedBaseCost, playerStats.moveSpeed, speedBaseValue, speedIncreaseStep);

        txtPriceHealth.text = currentHealthCost + " Gold";
        txtPriceDamage.text = currentDamageCost + " Gold";
        txtPriceSpeed.text = currentSpeedCost + " Gold";

        btnUpgradeHealth.interactable = playerStats.totalGold >= currentHealthCost;
        btnUpgradeDamage.interactable = playerStats.totalGold >= currentDamageCost;
        btnUpgradeSpeed.interactable = playerStats.totalGold >= currentSpeedCost;
    }

    private int CalculateCost(int baseCost, float currentStat, float baseStatValue, float stepAmount)
    {
        int upgradeCount = Mathf.Max(0, Mathf.FloorToInt((currentStat - baseStatValue) / stepAmount));
        return baseCost + Mathf.FloorToInt(baseCost * 0.5f * upgradeCount);
    }

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

    public void OnClickUpgradeSpeed()
    {
        int cost = CalculateCost(speedBaseCost, playerStats.moveSpeed, speedBaseValue, speedIncreaseStep);
        if (playerStats.totalGold >= cost)
        {
            playerStats.totalGold -= cost;
            playerStats.moveSpeed += speedIncreaseStep;
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