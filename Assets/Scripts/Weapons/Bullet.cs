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
        ObjectPoolManager.Return(gameObject);
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
                ObjectPoolManager.Spawn(bloodEffectPrefab, transform.position, Quaternion.identity);
            }

            ObjectPoolManager.Return(gameObject);
            return;
        }

        ObjectPoolManager.Return(gameObject);
    }
}