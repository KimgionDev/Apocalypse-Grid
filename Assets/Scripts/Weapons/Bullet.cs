using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(Collider2D))]
public class Bullet : MonoBehaviour
{
    private float damage;

    public GameObject bloodEffectPrefab;

    public void Setup(float setDamage, float setLifeTime)
    {
        damage = setDamage;
        GetComponent<Rigidbody2D>().linearVelocity = transform.right * 20f;
        Destroy(gameObject, setLifeTime);
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
                Instantiate(bloodEffectPrefab, transform.position, Quaternion.identity);
            }

            Destroy(gameObject);
            return;
        }

        if (hitInfo.CompareTag(Tags.Wall))
        {
            Destroy(gameObject);
            return;
        }
    }
}