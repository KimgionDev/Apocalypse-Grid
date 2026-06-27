using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class ZombieAI : MonoBehaviour
{
    public EnemyData data;
    public Animator animator;
    public float stopDistance = 1.2f;
    public float attackRate = 1f;

    [Header("Loot Settings")] 
    public GameObject lootPrefab;
    public DropItemData goldData;
    [Range(0, 100)] public float goldDropChance = 80f;
    public List<DropItemData> itemPool;
    [Range(0, 100)] public float itemDropChance = 40f;
    public float lootLifetime = 10f;
    public float currentDamage;

    private float currentMaxHealth;
    private float currentMoveSpeed;
    private Transform target;
    private float currentHealth;
    private bool isDead;
    private Rigidbody2D rb;
    private Vector2 direction;
    private bool isWalking;
    private float nextAttackTime = 0f;
    private float nextGrowlTime = 0f;
    private static float globalNextGrowlTime = 0f;
    private FlashEffect flashEffect;
    private static readonly int IsWalkingHash = AnimParams.IsWalking;
    private static readonly int DirXHash = AnimParams.DirX;
    private static readonly int DirYHash = AnimParams.DirY;
    private static readonly int AttackHash = AnimParams.Attack;
    private static readonly int DieHash = AnimParams.Die;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        flashEffect = GetComponent<FlashEffect>();
    }

    private void Start()
    {
        int currentLevel = SaveManager.GetCurrentLevel();

        float statMultiplier = 1f + ((currentLevel - 1) * 0.20f);
        float speedMultiplier = 1f + ((currentLevel - 1) * 0.05f);

        currentMaxHealth = data.maxHealth * statMultiplier;
        currentDamage = data.damage * statMultiplier;
        currentMoveSpeed = data.moveSpeed * speedMultiplier;

        currentHealth = currentMaxHealth;
        
        target = PlayerMovement.InstanceTransform;

        nextGrowlTime = Time.time + Random.Range(2f, 5f);
    }

    private void Update()
    {
        if (isDead || target == null) return;

        float distance = Vector2.Distance(rb.position, target.position);
        direction = ((Vector2)target.position - rb.position).normalized;
        isWalking = distance > stopDistance;

        if (animator != null)
        {
            animator.SetBool(IsWalkingHash, isWalking);

            if (direction != Vector2.zero)
            {
                animator.SetFloat(DirXHash, direction.x);
                animator.SetFloat(DirYHash, direction.y);
            }
        }

        if (isWalking && distance <= 10f && Time.time >= nextGrowlTime && Time.time >= globalNextGrowlTime &&
            data.growlSound != null)
        {
            AudioManager.Instance.PlaySFX(data.growlSound);
            nextGrowlTime = Time.time + Random.Range(12f, 20f);
            globalNextGrowlTime = Time.time + Random.Range(3f, 5f);
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

        rb.linearVelocity = isWalking ? direction * currentMoveSpeed : Vector2.zero;
    }

    public void TakeDamage(float damage)
    {
        if (isDead) return;

        currentHealth -= damage;

        if (flashEffect != null)
        {
            flashEffect.TriggerFlash();
        }

        if (data.takeDamageSound != null)
        {
            AudioManager.Instance.PlaySFX(data.takeDamageSound);
        }

        if (currentHealth <= 0f)
        {
            Die();
        }
    }

    private void AttackPlayer()
    {
        if (target.TryGetComponent<PlayerHealth>(out PlayerHealth playerHealth))
        {
            playerHealth.TakeDamage(currentDamage);
            if (animator != null) animator.SetTrigger(AttackHash);
        }

        if (data.attackSound != null)
        {
            AudioManager.Instance.PlaySFX(data.attackSound);
        }
    }

    public void Die()
    {
        if (isDead) return;
        isDead = true;

        rb.linearVelocity = Vector2.zero;

        if (TryGetComponent<BoxCollider2D>(out BoxCollider2D col)) col.enabled = false;

        if (animator != null) animator.SetTrigger(DieHash);

        if (data.deathSound != null)
        {
            AudioManager.Instance.PlaySFX(data.deathSound);
        }

        DropLoot();
        Destroy(gameObject, 1f);
    }

    private void DropLoot()
    {
        if (goldData != null && Random.Range(0f, 100f) <= goldDropChance)
        {
            SpawnItem(goldData, transform.position);
        }

        if (itemPool != null && itemPool.Count > 0 && Random.Range(0f, 100f) <= itemDropChance)
        {
            int randomIndex = Random.Range(0, itemPool.Count);
            DropItemData randomItem = itemPool[randomIndex];

            Vector2 offsetPos = (Vector2)transform.position + new Vector2(
                Random.Range(-0.5f, 0.5f),
                Random.Range(-0.5f, 0.5f));

            SpawnItem(randomItem, offsetPos);
        }
    }

    private void SpawnItem(DropItemData itemData, Vector2 spawnPos)
    {
        if (lootPrefab == null) return;

        GameObject drop = Instantiate(lootPrefab, spawnPos, Quaternion.identity);
        
        if (drop.TryGetComponent<WorldItem>(out WorldItem wItem))
        {
            wItem.SetupItem(itemData);
        }

        Destroy(drop, lootLifetime);
    }
}