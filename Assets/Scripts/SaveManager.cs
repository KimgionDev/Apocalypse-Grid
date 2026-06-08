using UnityEngine;
using System.IO;

public class SaveManager : MonoBehaviour
{
    public static SaveManager Instance;

    [Header("Dữ liệu cần lưu")]
    public PlayerStatsSO playerStats; 

    private string saveFilePath;

    private void Awake()
    {
        // Singleton pattern
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject); 

        // Thiết lập đường dẫn lưu file an toàn trên hệ điều hành cục bộ
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
}