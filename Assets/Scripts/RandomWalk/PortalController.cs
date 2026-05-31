using UnityEngine;
using UnityEngine.SceneManagement;

public class PortalController : MonoBehaviour
{
    [SerializeField] private string nextSceneName = "UpgradeMenuScene";
    [SerializeField] private Animator portalAnimator; 
    
    private bool isOpen = false;

    private void Update()
    {
        // Chờ nhiệm vụ hoàn thành để mở cổng (chỉ gọi 1 lần)
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
                SceneManager.LoadScene(nextSceneName);
            }
            else
            {
                Debug.Log("Cổng chưa mở, hãy làm xong nhiệm vụ!");
            }
        }
    }
}