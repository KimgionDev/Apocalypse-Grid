using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class MissionManager : MonoBehaviour
{
    public static MissionManager Instance;

    [Header("UI Elements")]
    public Image itemIcon;
    public TextMeshProUGUI progressText;

    [Header("Mission Data")]
    public int targetAmount = 30;
    private int currentAmount = 0;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        UpdateUI();
    }

    // Hàm gọi khi bắt đầu màn chơi mới 
    public void StartNewMission(int newTarget, Sprite newIcon)
    {
        targetAmount = newTarget;
        currentAmount = 0; // Reset bộ đếm về 0

        // Cập nhật hình ảnh vật phẩm mới
        if (itemIcon != null && newIcon != null)
        {
            itemIcon.sprite = newIcon;
        }

        // Trả màu chữ về lại mặc định (trắng)
        if (progressText != null)
        {
            progressText.color = Color.white;
        }

        UpdateUI();
    }

    public void AddMissionItem(int amount = 1)
    {
        if (currentAmount >= targetAmount) return; // Nhặt đủ rồi thì ngưng đếm

        currentAmount += amount;
        UpdateUI();

        if (currentAmount >= targetAmount)
        {
            LevelComplete();
        }
    }

    private void UpdateUI()
    {
        if (progressText != null)
        {
            progressText.text = currentAmount + " / " + targetAmount;
        }
    }

    private void LevelComplete()
    {
        Debug.Log("Mở cổng qua màn!");
        progressText.text = "Hoàn thành!";
        progressText.color = Color.green;
    }
}