using UnityEngine;
using System.Collections;

public class PlayerHealth : MonoBehaviour, IDamageable
{
    public PlayerStatsSO playerStats;
    public float maxHealth = 100f;
    public float currentHealth;
    private bool hasRunEnded;
    public Animator animator;
    
    [Header("Cơ chế Bất tử (I-Frames)")]
    public float invulnerabilityTime = 0.5f;
    private bool isInvulnerable = false;
    private FlashEffect flashEffect; 

    private void Awake()
    {
        flashEffect = GetComponent<FlashEffect>();
    }

    private void Start()
    {
        if (playerStats != null)
        {
            maxHealth = playerStats.maxHealth;
        }

        currentHealth = maxHealth;
    }

    public void TakeDamage(float damage)
    {
        if (hasRunEnded || isInvulnerable) return;
        
        currentHealth -= damage;

        if (currentHealth <= 0f)
        {
            Die();
        }
        else
        {
            if (flashEffect != null)
            {
                flashEffect.TriggerFlash();
            }
            
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
        if (!TryEndRun()) return;
        
        if (animator != null) animator.SetTrigger(AnimParams.Die); 
        
        if (GameUIManager.Instance != null)
        {
            GameUIManager.Instance.ShowResult(false);
        }
    }

    public bool TryEndRun()
    {
        if (hasRunEnded) return false;
        hasRunEnded = true;

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

        return true;
    }
    
    public void Heal(float amount)
    {
        if (hasRunEnded) return;

        currentHealth += amount;
        
        if (currentHealth > maxHealth) 
        {
            currentHealth = maxHealth;
        }
    }
}
