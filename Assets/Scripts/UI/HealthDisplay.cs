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
            float fillFront = healthFill.fillAmount;
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

                healthYellowFill.color = new Color(0.9f, 0.6f, 0.1f, 1f);
                healthYellowFill.fillAmount = Mathf.Lerp(fillBack, fillAmount, percentComplete);
            }
            else if (fillFront < fillAmount)
            {
                healthYellowFill.fillAmount = fillAmount;
                lerpTimer += Time.deltaTime;
                float percentComplete = lerpTimer / chipSpeed;
                percentComplete = percentComplete * percentComplete;

                healthYellowFill.color = new Color(0.15f, 0.75f, 0.45f, 1f);
                healthFill.fillAmount = Mathf.Lerp(healthFill.fillAmount, fillAmount, percentComplete);
            }
            else
            {
                healthFill.fillAmount = fillAmount;
                healthYellowFill.fillAmount = fillAmount;
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