using UnityEngine;
using UnityEngine.UI;

public class HealthDisplay : MonoBehaviour
{
    public Image healthFill;
    public Image healthYellowFill;

    [SerializeField] private float chipSpeed = 2f;
    private PlayerHealth playerHealth;
    private float lastFillAmount = -1f;
    private float lerpTimer;

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

        if (healthFill != null && healthYellowFill != null)
        {
            float fillAmount = playerHealth.currentHealth / playerHealth.maxHealth;
            float fillBack = healthYellowFill.fillAmount;
            if (fillAmount != lastFillAmount)
            {
                lerpTimer = 0f;
                lastFillAmount = fillAmount;
            }

            if (fillBack > fillAmount)
            {
                healthFill.fillAmount = fillAmount;
                lerpTimer += Time.deltaTime;
                float percentComplete = lerpTimer / chipSpeed;
                percentComplete = percentComplete * percentComplete;

                healthYellowFill.fillAmount = Mathf.Lerp(fillBack, fillAmount, percentComplete);
            }
            else if (healthFill.fillAmount < fillAmount)
            {
                healthYellowFill.fillAmount = fillAmount;

                lerpTimer += Time.deltaTime;
                float percentComplete = lerpTimer / chipSpeed;
                percentComplete = percentComplete * percentComplete;

                healthFill.fillAmount = Mathf.Lerp(healthFill.fillAmount, fillAmount, percentComplete);
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