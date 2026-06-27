using UnityEngine;
using UnityEngine.UI;

public class HealthDisplay : MonoBehaviour
{
    private PlayerHealth playerHealth;
    public Image healthFill;

    void Update()
    {
        if (playerHealth == null)
        {
            GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
            
            if (playerObj != null)
            {
                playerHealth = playerObj.GetComponent<PlayerHealth>();
            }
        }

        if (playerHealth != null && healthFill != null)
        {
            float fillAmount = playerHealth.currentHealth / playerHealth.maxHealth;
            healthFill.fillAmount = fillAmount;
        }
    }
}