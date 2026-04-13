using UnityEngine;

public class ZombieAI : MonoBehaviour
{
    public EnemyData data;
    public Animator animator;
    public float stopDistance = 1.2f;

    private Transform target;
    private float currentHealth;

    void Start()
    {
        // Khởi tạo thông số từ Data
        currentHealth = data.maxHealth;

        GameObject playerObject = GameObject.FindGameObjectWithTag("Player");
        if (playerObject != null)
        {
            target = playerObject.transform;
        }
    }

    void Update()
    {
        if (target != null)
        {
            float distance = Vector2.Distance(transform.position, target.position);
            Vector2 direction = (target.position - transform.position).normalized;

            if (distance > stopDistance)
            {
                transform.position = Vector2.MoveTowards(transform.position, target.position, data.moveSpeed * Time.deltaTime);
                if (animator != null) animator.SetBool("IsWalking", true);
            }
            else
            {
                if (animator != null) animator.SetBool("IsWalking", false);
            }

            if (animator != null && direction != Vector2.zero)
            {
                animator.SetFloat("DirX", direction.x);
                animator.SetFloat("DirY", direction.y);
            }
        }
    }

    public void TakeDamage(float damage)
    {
        currentHealth -= damage;
        if (currentHealth <= 0)
        {
            Die();
        }
    }

    public void Die()
    {
        if (animator != null) animator.SetTrigger("Die");
        Destroy(gameObject, 0.5f); // Hủy sau 0.5 giây để hiệu ứng chết có thời gian phát
    }
}