using UnityEngine;
using UnityEngine.UI;

public class HealthDisplay : MonoBehaviour
{
    public PlayerHealth playerHealth;
    public Image healthFill;

    void Update()
    {
        if (playerHealth != null && healthFill != null)
        {
            float fillAmount = playerHealth.currentHealth / playerHealth.maxHealth;
            healthFill.fillAmount = fillAmount;
        }
    }
}