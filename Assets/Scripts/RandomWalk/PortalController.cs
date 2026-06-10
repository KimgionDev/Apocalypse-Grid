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
        if (other.CompareTag("Player"))
        {
            if (isOpen)
            {
                if (SaveManager.Instance != null && SaveManager.Instance.playerStats != null && InventoryManager.Instance != null)
                {
                    SaveManager.Instance.playerStats.totalGold += InventoryManager.Instance.gold;
                    SaveManager.Instance.playerStats.currentLevel++;
                    SaveManager.Instance.SaveGame();
                }

                if (GameUIManager.Instance != null)
                {
                    GameUIManager.Instance.ShowResult(true);
                    
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