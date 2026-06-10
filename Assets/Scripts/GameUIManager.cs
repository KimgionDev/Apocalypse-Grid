using System.Collections;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class GameUIManager : MonoBehaviour
{
    public static GameUIManager Instance;
    public GameObject resultPanel;
    public TextMeshProUGUI txtTitle;
    public TextMeshProUGUI txtMessage;
    public TextMeshProUGUI txtGold;

    private bool isWaitingToReturn = false;
    private string nextScene = "DashboardScene";

    private void Awake()
    {
        Instance = this;
        if (resultPanel != null) resultPanel.SetActive(false);
    }

    private void Update()
    {
        if (isWaitingToReturn && Input.anyKeyDown)
        {
            ReturnToDashboard();
        }
    }

    public void ShowResult(bool isVictory)
    {
        if (resultPanel != null) resultPanel.SetActive(true);
        isWaitingToReturn = true;

        int earnedGold = InventoryManager.Instance != null ? InventoryManager.Instance.gold : 0;
        txtGold.text = "+ " + earnedGold + " Vàng";

        if (isVictory)
        {
            txtTitle.text = "HOÀN THÀNH!";
            txtTitle.color = Color.green;
            txtMessage.text = "Bạn đã sống sót và tìm được lối thoát.";
        }
        else
        {
            txtTitle.text = "TỬ TRẬN!";
            txtTitle.color = Color.red;
            txtMessage.text = "Bầy xác sống đã xé xác bạn...";
            
            if (SaveManager.Instance != null && SaveManager.Instance.playerStats != null)
            {
                SaveManager.Instance.playerStats.totalGold += earnedGold;
                SaveManager.Instance.SaveGame();
            }
        }

        StartCoroutine(AutoReturnRoutine());
    }

    private IEnumerator AutoReturnRoutine()
    {
        yield return new WaitForSecondsRealtime(5f); 
        
        if (isWaitingToReturn)
        {
            ReturnToDashboard();
        }
    }

    private void ReturnToDashboard()
    {
        isWaitingToReturn = false;
        SceneManager.LoadScene(nextScene);
    }
}