using UnityEngine;

public class ExplosiveBarrel : DestructibleObject
{
    public float explosionRadius = 2.5f;
    public float explosionDamage = 50f;
    public LayerMask damageableLayers;

    protected override void BreakObject()
    {
        if (breakSound != null && AudioManager.Instance != null)
        {
            AudioManager.Instance.PlaySFX(breakSound);
        }

        if (breakEffectPrefab != null)
        {
            Instantiate(breakEffectPrefab, transform.position, Quaternion.identity);
        }

        Collider2D[] targetsInExplosion = Physics2D.OverlapCircleAll(transform.position, explosionRadius, damageableLayers);

        foreach (Collider2D col in targetsInExplosion)
        {
            if (col.gameObject == this.gameObject) continue;

            if (col.TryGetComponent<IDamageable>(out IDamageable damageableTarget))
            {
                damageableTarget.TakeDamage(explosionDamage);
            }
        }

        Destroy(gameObject);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, explosionRadius);
    }
}