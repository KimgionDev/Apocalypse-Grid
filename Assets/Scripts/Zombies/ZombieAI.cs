using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class ZombieAI : MonoBehaviour
{
    public EnemyData data;
    public Animator animator;
    public float stopDistance = 1.2f;
    public float attackRate = 1f;

    [Header("Loot Settings")] public GameObject lootPrefab;
    public DropItemData goldData;
    [Range(0, 100)] public float goldDropChance = 80f;
    public List<DropItemData> itemPool;
    [Range(0, 100)] public float itemDropChance = 40f;
    public float lootLifetime = 10f;

    [Header("Hit Effect (Shader)")] public float blinkDecaySpeed = 10f;
    private SpriteRenderer[] renderers;
    private MaterialPropertyBlock propertyBlock;
    private float blinkFactor = 0f;

    private float currentMaxHealth;
    [HideInInspector] public float currentDamage;
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

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();

        int currentLevel = 1;
        if (SaveManager.Instance != null && SaveManager.Instance.playerStats != null)
        {
            currentLevel = SaveManager.Instance.playerStats.currentLevel;
        }

        float statMultiplier = 1f + ((currentLevel - 1) * 0.20f);
        float speedMultiplier = 1f + ((currentLevel - 1) * 0.05f);

        currentMaxHealth = data.maxHealth * statMultiplier;
        currentDamage = data.damage * statMultiplier;
        currentMoveSpeed = data.moveSpeed * speedMultiplier;

        currentHealth = currentMaxHealth;

        GameObject playerObject = GameObject.FindGameObjectWithTag("Player");
        if (playerObject != null)
        {
            target = playerObject.transform;
        }

        renderers = GetComponentsInChildren<SpriteRenderer>();
        propertyBlock = new MaterialPropertyBlock();
        blinkFactor = 0f;
        ApplyBlinkFactor();
        nextGrowlTime = Time.time + Random.Range(2f, 5f);
    }

    private void Update()
    {
        if (blinkFactor > 0f)
        {
            blinkFactor = Mathf.Lerp(blinkFactor, 0f, Time.deltaTime * blinkDecaySpeed);
            if (blinkFactor < 0.01f) blinkFactor = 0f;
            ApplyBlinkFactor();
        }

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

        if (isWalking && distance <= 10f && Time.time >= nextGrowlTime && Time.time >= globalNextGrowlTime && data.growlSound != null)
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

    private void ApplyBlinkFactor()
    {
        foreach (var renderer in renderers)
        {
            renderer.GetPropertyBlock(propertyBlock);
            propertyBlock.SetFloat("_BlinkFactor", blinkFactor);
            renderer.SetPropertyBlock(propertyBlock);
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

        blinkFactor = 1f;
        ApplyBlinkFactor();

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
        PlayerHealth playerHealth = target.GetComponent<PlayerHealth>();
        if (playerHealth != null)
        {
            playerHealth.TakeDamage(currentDamage);
            if (animator != null) animator.SetTrigger("Attack");
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

        BoxCollider2D col = GetComponent<BoxCollider2D>();
        if (col != null) col.enabled = false;

        if (animator != null) animator.SetTrigger("Die");

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
        WorldItem wItem = drop.GetComponent<WorldItem>();
        if (wItem != null)
        {
            wItem.SetupItem(itemData);
        }

        Destroy(drop, lootLifetime);
    }
}