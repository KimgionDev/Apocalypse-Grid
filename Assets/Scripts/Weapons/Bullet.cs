using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(Collider2D))]
public class Bullet : MonoBehaviour
{
    private float damage;

    public GameObject bloodEffectPrefab;

    private Coroutine lifetimeCoroutine;

    public void Setup(float setDamage, float setLifeTime)
    {
        damage = setDamage;
        GetComponent<Rigidbody2D>().linearVelocity = transform.right * 20f;
        
        if (lifetimeCoroutine != null) StopCoroutine(lifetimeCoroutine);
        lifetimeCoroutine = StartCoroutine(ReturnToPoolAfterTime(setLifeTime));
    }

    private System.Collections.IEnumerator ReturnToPoolAfterTime(float time)
    {
        yield return new WaitForSeconds(time);
        if (ObjectPoolManager.Instance != null) ObjectPoolManager.Instance.ReturnToPool(gameObject);
        else Destroy(gameObject);
    }

    void OnTriggerEnter2D(Collider2D hitInfo)
    {
        if (hitInfo.CompareTag(Tags.Player) || hitInfo.CompareTag(Tags.Bullet) || hitInfo.CompareTag(Tags.ItemDrop))
        {
            return;
        }

        if (hitInfo.TryGetComponent<IDamageable>(out IDamageable damageableTarget))
        {
            damageableTarget.TakeDamage(damage);
            if (hitInfo.CompareTag(Tags.Zombie) && bloodEffectPrefab != null)
            {
                if (ObjectPoolManager.Instance != null)
                {
                    ObjectPoolManager.Instance.SpawnFromPool(bloodEffectPrefab, transform.position, Quaternion.identity);
                }
                else
                {
                    Instantiate(bloodEffectPrefab, transform.position, Quaternion.identity);
                }
            }

            if (ObjectPoolManager.Instance != null) ObjectPoolManager.Instance.ReturnToPool(gameObject);
            else Destroy(gameObject);
            return;
        }

        if (hitInfo.CompareTag(Tags.Wall))
        {
            if (ObjectPoolManager.Instance != null) ObjectPoolManager.Instance.ReturnToPool(gameObject);
            else Destroy(gameObject);
            return;
        }
    }
}