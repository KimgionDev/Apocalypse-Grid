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
    public GameObject notificationPanel;
    public GameObject pausePanel;
    public TextMeshProUGUI txtNotification;
    public TextMeshProUGUI txtAmmo;
    public AudioClip victorySound;
    public AudioClip defeatSound;

    private Coroutine notificationCoroutine;
    private bool isPaused = false;
    private bool isWaitingToReturn = false;
    private bool canSkipInput = false;
    private string nextScene = "DashboardScene";

    private void Awake()
    {
        Instance = this;
        if (resultPanel != null) resultPanel.SetActive(false);
        if (notificationPanel != null) notificationPanel.SetActive(false);
    }

    private IEnumerator Start()
    {
        yield return new WaitForSeconds(0.5f);

        int level = 1;
        if (SaveManager.Instance != null && SaveManager.Instance.playerStats != null)
        {
            level = SaveManager.Instance.playerStats.currentLevel;
            if (level <= 0) level = 1;
        }

        ShowNotification("FLOOR " + level + "\nCOLLECT LOOT TO UNLOCK PORTAL!");
    }

    private IEnumerator SlideUI(RectTransform uiElement, Vector2 startPos, Vector2 endPos, float duration)
    {
        float elapsed = 0f;
        uiElement.anchoredPosition = startPos;

        while (elapsed < duration)
        {
            elapsed += Time.unscaledDeltaTime;
            float t = elapsed / duration;
            float smoothStep = t * t * (3f - 2f * t);

            uiElement.anchoredPosition = Vector2.Lerp(startPos, endPos, smoothStep);
            yield return null;
        }

        uiElement.anchoredPosition = endPos;
    }

    public void ShowNotification(string message)
    {
        if (notificationCoroutine != null)
        {
            StopCoroutine(notificationCoroutine);
        }

        notificationCoroutine = StartCoroutine(NotificationRoutine(message));
    }

    private IEnumerator NotificationRoutine(string message)
    {
        if (notificationPanel != null)
        {
            notificationPanel.SetActive(true);
            if (txtNotification != null) txtNotification.text = message;
            RectTransform rect = notificationPanel.GetComponent<RectTransform>();
            float originalY = rect.anchoredPosition.y;

            Vector2 startPos = new Vector2(-1500f, originalY);
            Vector2 centerPos = new Vector2(0f, originalY);
            Vector2 endPos = new Vector2(1500f, originalY);

            yield return StartCoroutine(SlideUI(rect, startPos, centerPos, 0.5f));
            yield return new WaitForSecondsRealtime(2.5f);
            yield return StartCoroutine(SlideUI(rect, centerPos, endPos, 0.5f));

            notificationPanel.SetActive(false);
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape) && !isWaitingToReturn)
        {
            TogglePause();
        }

        if (isWaitingToReturn && canSkipInput && Input.anyKeyDown)
        {
            ReturnToDashboard();
        }
    }

    public void TogglePause()
    {
        isPaused = !isPaused;
        if (isPaused)
        {
            Time.timeScale = 0f;
            if (pausePanel != null) pausePanel.SetActive(true);
        }
        else
        {
            Time.timeScale = 1f;
            if (pausePanel != null) pausePanel.SetActive(false);
        }
    }

    public void ResumeGame()
    {
        if (isPaused) TogglePause();
    }

    public void QuitToDashboard()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(nextScene);
    }

    public void ShowResult(bool isVictory, bool isGameClear = false)
    {
        StartCoroutine(ShowResultRoutine(isVictory, isGameClear));
    }

    private IEnumerator ShowResultRoutine(bool isVictory, bool isGameClear)
    {
        yield return new WaitForSecondsRealtime(1.2f);

        Time.timeScale = 0f;

        if (AudioManager.Instance != null && AudioManager.Instance.musicSource != null)
        {
            AudioManager.Instance.musicSource.Stop();
        }

        if (resultPanel != null)
        {
            resultPanel.SetActive(true);
            RectTransform rect = resultPanel.GetComponent<RectTransform>();
            Vector2 topPos = new Vector2(0f, 1500f);
            Vector2 centerPos = Vector2.zero;
            StartCoroutine(SlideUI(rect, topPos, centerPos, 0.6f));
        }

        isWaitingToReturn = true;
        canSkipInput = false;


        int earnedGold = InventoryManager.Instance != null ? InventoryManager.Instance.gold : 0;
        txtGold.text = "+ " + earnedGold + " Gold";
        txtGold.color = Color.gold;

        if (isVictory)
        {
            if (isGameClear)
            {
                txtTitle.text = "ESCAPED!";
                txtTitle.color = Color.cyan;
                txtMessage.text = "Congratulations! You have survived all 21 floors and escaped the Apocalypse Grid!";
            }
            else
            {
                txtTitle.text = "MISSION COMPLETE";
                txtTitle.color = Color.green;
                txtMessage.text = "You've survived the zombie horde! Prepare for the next challenge";
            }
            
            if (victorySound != null && AudioManager.Instance != null)
            {
                AudioManager.Instance.PlaySFX(victorySound);
            }
        }
        else
        {
            txtTitle.text = "DEFEATED";
            txtTitle.color = Color.red;
            txtMessage.text = "The zombies overwhelmed you this time. Don't give up, try again!";

            if (defeatSound != null && AudioManager.Instance != null)
            {
                AudioManager.Instance.PlaySFX(defeatSound);
            }

            if (SaveManager.Instance != null && SaveManager.Instance.playerStats != null)
            {
                SaveManager.Instance.playerStats.totalGold += earnedGold;
                SaveManager.Instance.SaveGame();
            }
        }

        yield return new WaitForSecondsRealtime(1.5f);
        canSkipInput = true;

        yield return new WaitForSecondsRealtime(3.5f);

        if (isWaitingToReturn)
        {
            ReturnToDashboard();
        }
    }

    private void ReturnToDashboard()
    {
        Time.timeScale = 1f;
        isWaitingToReturn = false;
        canSkipInput = false;
        SceneManager.LoadScene(nextScene);
    }
    
    public void UpdateAmmo(int currentAmmo)
    {
        if (txtAmmo != null)
        {
            txtAmmo.text = currentAmmo.ToString() + "/∞";
        }
    }

    public void ShowReloadingText()
    {
        if (txtAmmo != null)
        {
            txtAmmo.text = "Reloading...";
        }
    }
}