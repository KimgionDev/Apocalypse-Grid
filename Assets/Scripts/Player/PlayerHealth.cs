using UnityEngine;
using System.Collections;

public class PlayerHealth : MonoBehaviour
{
    public PlayerStatsSO playerStats;

    public float maxHealth = 100f;
    public float currentHealth;
    private bool isDead;
    public Animator animator;

    [Header("Hiệu ứng Flash Shader")]
    public float blinkDecaySpeed = 10f;
    private SpriteRenderer[] renderers;
    private MaterialPropertyBlock propertyBlock;
    private float blinkFactor = 0f;

    [Header("Cơ chế Bất tử (I-Frames)")]
    public float invulnerabilityTime = 0.5f;
    private bool isInvulnerable = false;

    private void Start()
    {
        if (playerStats != null)
        {
            maxHealth = playerStats.maxHealth;
        }

        currentHealth = maxHealth;

        renderers = GetComponentsInChildren<SpriteRenderer>();
        propertyBlock = new MaterialPropertyBlock();
        
        blinkFactor = 0f;
        ApplyBlinkFactor();
    }

    private void Update()
    {
        if (blinkFactor > 0f)
        {
            blinkFactor = Mathf.Lerp(blinkFactor, 0f, Time.deltaTime * blinkDecaySpeed);
            if (blinkFactor < 0.01f)
            {
                blinkFactor = 0f;
            }
            ApplyBlinkFactor();
        }
    }

    private void ApplyBlinkFactor()
    {
        foreach (var renderer in renderers)
        {
            renderer.GetPropertyBlock(propertyBlock);
            propertyBlock.SetFloat("_BlinkFactor", blinkFactor);
            renderer.SetPropertyBlock(propertyBlock);
        }
    }

    public void TakeDamage(float damage)
    {
        if (isDead || isInvulnerable) return; 
        
        currentHealth -= damage;

        if (currentHealth <= 0f)
        {
            Die();
        }
        else
        {
            blinkFactor = 1f;
            ApplyBlinkFactor();
            StartCoroutine(InvulnerabilityRoutine());
        }
    }

    private IEnumerator InvulnerabilityRoutine()
    {
        isInvulnerable = true;
        yield return new WaitForSeconds(invulnerabilityTime);
        isInvulnerable = false;
    }

    private void Die()
    {
        if (isDead) return;
        isDead = true;
        
        // Đoạn này của bạn gọi Animation siêu chuẩn
        if (animator != null) animator.SetTrigger("Die"); 
        
        if (GameUIManager.Instance != null)
        {
            GameUIManager.Instance.ShowResult(false);
        }

        PlayerMovement movement = GetComponent<PlayerMovement>();
        if (movement != null)
        {
            movement.enabled = false;
        }

        PlayerShoot shoot = GetComponentInChildren<PlayerShoot>();
        if (shoot != null)
        {
            shoot.enabled = false;
        }
    }
}