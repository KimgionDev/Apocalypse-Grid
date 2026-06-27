using UnityEngine;
using UnityEngine.UI;

public class HealthDisplay : MonoBehaviour
{
    public Image healthFill;
    
    private PlayerHealth playerHealth;
    private float lastFillAmount = -1f;

    void Start()
    {
        FindPlayer();
    }

    void Update()
    {
        if (playerHealth == null)
        {
            FindPlayer();
            return;
        }

        if (healthFill != null)
        {
            float fillAmount = playerHealth.currentHealth / playerHealth.maxHealth;
            if (fillAmount != lastFillAmount)
            {
                healthFill.fillAmount = fillAmount;
                lastFillAmount = fillAmount;
            }
        }
    }

    private void FindPlayer()
    {
        Transform playerObj = PlayerMovement.InstanceTransform;
        if (playerObj != null)
        {
            playerHealth = playerObj.GetComponent<PlayerHealth>();
        }
    }
}