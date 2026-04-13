using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(Collider2D))]
public class Bullet : MonoBehaviour
{
    private float damage;

    public void Setup(float setDamage, float setLifeTime)
    {
        damage = setDamage;

        GetComponent<Rigidbody2D>().linearVelocity = transform.right * 20f;

        Destroy(gameObject, setLifeTime);
    }

    void OnTriggerEnter2D(Collider2D hitInfo)
    {
        if (!hitInfo.CompareTag("Player") && !hitInfo.CompareTag("Bullet"))
        {
            if(hitInfo.CompareTag("Zombie"))
            {
                hitInfo.GetComponent<ZombieAI>().TakeDamage(damage);
                
            }
            Destroy(gameObject);
        }
    }
}