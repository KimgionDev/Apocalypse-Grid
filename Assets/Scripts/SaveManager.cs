using UnityEngine;
using System.IO;

public class SaveManager : MonoBehaviour
{
    public static SaveManager Instance;
    public PlayerStatsSO playerStats;

    private string saveFilePath;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
        saveFilePath = Application.persistentDataPath + "/savegame.json";
        LoadGame();
    }

    public void SaveGame()
    {
        if (playerStats == null) return;

        string json = JsonUtility.ToJson(playerStats, true);

        File.WriteAllText(saveFilePath, json);
        Debug.Log("Đã lưu dữ liệu game thành công tại: " + saveFilePath);
    }

    public void LoadGame()
    {
        if (File.Exists(saveFilePath))
        {
            string json = File.ReadAllText(saveFilePath);

            JsonUtility.FromJsonOverwrite(json, playerStats);
            Debug.Log("Đã tải dữ liệu game thành công!");
        }
        else
        {
            Debug.LogWarning("Không tìm thấy file save, sẽ tạo mới khi lưu.");
        }
    }
    
    public  static int GetCurrentLevel()
    {
        if (Instance != null && Instance.playerStats != null)
        {
            return Mathf.Max(1, Instance.playerStats.currentLevel);
        }
        return 1;
    }
}