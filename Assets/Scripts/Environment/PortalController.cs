using UnityEngine;
using UnityEngine.SceneManagement;

public class PortalController : MonoBehaviour
{
    [SerializeField] private Animator portalAnimator; 
    
    private bool isOpen = false;

    private void Update()
    {
        if (!isOpen && MissionManager.Instance != null && MissionManager.Instance.isMissionCompleted)
        {
            OpenPortal();
        }
    }

    private void OpenPortal()
    {
        isOpen = true;
        if (portalAnimator != null)
        {
            portalAnimator.SetTrigger("Open"); 
        }
        Debug.Log("Nhiệm vụ hoàn tất, Cổng đã mở!");
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag(Tags.Player))
        {
            if (isOpen)
            {
                bool isGameClear = false;
                int rewardGold = 0;

                if (SaveManager.Instance != null && SaveManager.Instance.playerStats != null && InventoryManager.Instance != null)
                {
                    int currentLevel = SaveManager.Instance.playerStats.currentLevel;
                    rewardGold = currentLevel * 50 + 50; // Lvl 1 -> 100, Lvl 2 -> 150
                    
                    SaveManager.Instance.playerStats.totalGold += (InventoryManager.Instance.gold + rewardGold);
                    
                    if (currentLevel >= 21)
                    {
                        isGameClear = true;
                        SaveManager.Instance.playerStats.currentLevel = 1;
                    }
                    else
                    {
                        SaveManager.Instance.playerStats.currentLevel++;
                    }

                    SaveManager.Instance.SaveGame();
                }

                if (GameUIManager.Instance != null)
                {
                    GameUIManager.Instance.ShowResult(true, isGameClear, rewardGold);
                    
                    PlayerMovement movement = other.GetComponent<PlayerMovement>();
                    if (movement != null) 
                    {
                        movement.enabled = false;
                    }

                    PlayerShoot shoot = other.GetComponentInChildren<PlayerShoot>();
                    if (shoot != null) 
                    {
                        shoot.enabled = false;
                    }
                }
            }
        }
    }
}