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
    private bool canSkipInput = false;
    private string nextScene = "DashboardScene";

    private void Awake()
    {
        Instance = this;
        if (resultPanel != null) resultPanel.SetActive(false);
    }

    private void Update()
    {
        if (isWaitingToReturn && canSkipInput && Input.anyKeyDown)
        {
            ReturnToDashboard();
        }
    }

    public void ShowResult(bool isVictory)
    {
        StartCoroutine(ShowResultRoutine(isVictory));
    }

    private IEnumerator ShowResultRoutine(bool isVictory)
    {
        yield return new WaitForSecondsRealtime(1.2f);

        if (resultPanel != null) resultPanel.SetActive(true);
        isWaitingToReturn = true;
        canSkipInput = false;


        int earnedGold = InventoryManager.Instance != null ? InventoryManager.Instance.gold : 0;
        txtGold.text = "+ " + earnedGold + " Vàng";

        if (isVictory)
        {
            txtTitle.text = "MISSION COMPLETE!";
            txtTitle.color = Color.green;
            txtMessage.text = "You've survived the zombie horde! Prepare for the next challenge";
        }
        else
        {
            txtTitle.text = "DEFEATED!";
            txtTitle.color = Color.red;
            txtMessage.text = "The zombies overwhelmed you this time. Don't give up, try again!";

            if (SaveManager.Instance != null && SaveManager.Instance.playerStats != null)
            {
                SaveManager.Instance.playerStats.totalGold += earnedGold;
                SaveManager.Instance.SaveGame();
            }
        }

        yield return new WaitForSecondsRealtime(1.5f);
        canSkipInput = true;

        yield return new WaitForSecondsRealtime(3.5f);

        {
            ReturnToDashboard();
        }
    }

    private void ReturnToDashboard()
    {
        isWaitingToReturn = false;
        canSkipInput = false;
        SceneManager.LoadScene(nextScene);
    }
}