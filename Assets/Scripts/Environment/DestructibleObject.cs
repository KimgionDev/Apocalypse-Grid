using UnityEngine;

public class DestructibleObject : MonoBehaviour, IDamageable
{
    [Header("Chỉ số cơ bản")]
    public float maxHealth = 20f;
    protected float currentHealth;

    [Header("Hiệu ứng & Âm thanh vỡ")]
    public AudioClip breakSound;          
    public GameObject breakEffectPrefab; 

    private FlashEffect flashEffect;

    protected virtual void Awake()
    {
        currentHealth = maxHealth;
        flashEffect = GetComponent<FlashEffect>(); 
    }

    public void TakeDamage(float damage)
    {
        if (currentHealth <= 0f) return;

        currentHealth -= damage;

        if (flashEffect != null)
        {
            flashEffect.TriggerFlash();
        }

        if (currentHealth <= 0f)
        {
            BreakObject();
        }
    }

    protected virtual void BreakObject() 
    {
        if (breakSound != null && AudioManager.Instance != null)
        {
            AudioManager.Instance.PlaySFX(breakSound);
        }

        if (breakEffectPrefab != null)
        {
            Instantiate(breakEffectPrefab, transform.position, Quaternion.identity);
        }

        Destroy(gameObject);
    }
}