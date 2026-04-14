using NUnit.Framework.Internal.Execution;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class ZombieAI : MonoBehaviour
{
    public EnemyData data;
    public Animator animator;
    public float stopDistance = 1.2f;
    public float attackRate = 1f;
    public GameObject lootPrefab;
    public ItemData missionDrop;

    private Transform target;
    private float currentHealth;
    private bool isDead;
    private Rigidbody2D rb;
    private Vector2 direction;
    private bool isWalking;
    private float nextAttackTime = 0f;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        currentHealth = data.maxHealth;

        GameObject playerObject = GameObject.FindGameObjectWithTag("Player");
        if (playerObject != null)
        {
            target = playerObject.transform;
        }
    }

    private void Update()
    {
        if (isDead || target == null) return;

        float distance = Vector2.Distance(rb.position, target.position);
        direction = ((Vector2)target.position - rb.position).normalized;
        isWalking = distance > stopDistance;

        if (animator != null)
        {
            animator.SetBool("IsWalking", isWalking);

            if (direction != Vector2.zero)
            {
                animator.SetFloat("DirX", direction.x);
                animator.SetFloat("DirY", direction.y);
            }
        }

        if (!isWalking && Time.time >= nextAttackTime)
        {
            AttackPlayer();
            nextAttackTime = Time.time + (1f / attackRate);
        }
    }

    private void FixedUpdate()
    {
        if (isDead || target == null)
        {
            rb.linearVelocity = Vector2.zero;
            return;
        }

        rb.linearVelocity = isWalking ? direction * data.moveSpeed : Vector2.zero;
    }

    public void TakeDamage(float damage)
    {
        if (isDead) return;

        currentHealth -= damage;
        if (currentHealth <= 0f)
        {
            Die();
        }
    }

    private void AttackPlayer()
    {
        PlayerHealth playerHealth = target.GetComponent<PlayerHealth>();
        if (playerHealth != null)
        {
            playerHealth.TakeDamage(data.damage);
            if (animator != null) animator.SetTrigger("Attack");
        }
    }

    public void Die()
    {
        if (isDead) return;
        isDead = true;

        rb.linearVelocity = Vector2.zero;

        BoxCollider2D col = GetComponent<BoxCollider2D>();
        if (col != null) col.enabled = false;

        if (animator != null) animator.SetTrigger("Die");
        Destroy(gameObject, 1f);

        if (missionDrop != null && lootPrefab != null)
        {
            GameObject drop = Instantiate(lootPrefab, transform.position, Quaternion.identity);

            WorldItem wItem = drop.GetComponent<WorldItem>();
            if (wItem != null)
            {
                wItem.SetupItem(missionDrop);
            }
        }
    }
}