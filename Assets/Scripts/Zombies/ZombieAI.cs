using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class ZombieAI : MonoBehaviour, IDamageable
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
    public float currentDamage;
    
    [Header("Ammo Drops")]
    public List<DropItemData> ammoPool;
    public float ammoDropChance = 10f;

    [SerializeField] private float obstacleCheckDistance = 0.8f;
    [SerializeField] private LayerMask obstacleLayer;
    private Vector2 currentMoveDirection;

    private float currentMaxHealth;
    private float currentMoveSpeed;
    private Transform target;
    private float currentHealth;
    private bool isDead;
    public bool isAwake = false;
    private Rigidbody2D rb;
    private bool isWalking;
    private float nextAttackTime = 0f;
    private float nextGrowlTime = 0f;
    private static float globalNextGrowlTime = 0f;
    private FlashEffect flashEffect;
    private Vector2 chosenAvoidDirection;
    private float avoidTimeLeft = 0f;

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

    public void WakeUp()
    {
        isAwake = true;
        nextGrowlTime = Time.time + Random.Range(1f, 3f);
    }

    private void OnEnable()
    {
        int currentLevel = SaveManager.GetCurrentLevel();

        float statMultiplier = 1f + ((currentLevel - 1) * 0.20f);
        float speedMultiplier = 1f + ((currentLevel - 1) * 0.05f);

        currentMaxHealth = data.maxHealth * statMultiplier;
        currentDamage = data.damage * statMultiplier;
        currentMoveSpeed = data.moveSpeed * speedMultiplier;

        currentHealth = currentMaxHealth;
        isDead = false;
        isAwake = false;
        avoidTimeLeft = 0f;

        if (TryGetComponent<BoxCollider2D>(out BoxCollider2D col)) col.enabled = true;

        target = PlayerMovement.InstanceTransform;

        nextGrowlTime = Time.time + Random.Range(2f, 5f);
    }

    private void Update()
    {
        if (isDead || target == null) return;

        if (!isAwake)
        {
            isWalking = false;
            currentMoveDirection = Vector2.zero;
            if (animator != null)
            {
                animator.SetBool(IsWalkingHash, false);
            }
            return;
        }

        float distance = Vector2.Distance(rb.position, target.position);

        Vector2 direction = ((Vector2)target.position - rb.position).normalized;
        isWalking = distance > stopDistance;

        if (isWalking)
        {
            currentMoveDirection = CheckAndAvoidObstacles(direction);
        }
        else
        {
            currentMoveDirection = Vector2.zero;
        }

        if (animator != null)
        {
            animator.SetBool(IsWalkingHash, isWalking);

            if (currentMoveDirection != Vector2.zero)
            {
                animator.SetFloat(DirXHash, currentMoveDirection.x);
                animator.SetFloat(DirYHash, currentMoveDirection.y);
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

        rb.linearVelocity = isWalking ? currentMoveDirection * currentMoveSpeed : Vector2.zero;
    }

    private Vector2 CheckAndAvoidObstacles(Vector2 targetDir)
    {
        float zombieRadius = 0.3f;

        if (avoidTimeLeft > 0f)
        {
            avoidTimeLeft -= Time.deltaTime;
            Debug.DrawRay(transform.position, chosenAvoidDirection * obstacleCheckDistance, Color.green);
            return chosenAvoidDirection;
        }

        RaycastHit2D hit = Physics2D.CircleCast(transform.position, zombieRadius, targetDir, obstacleCheckDistance,
            obstacleLayer);
        Debug.DrawRay(transform.position, targetDir * obstacleCheckDistance, Color.red);

        if (hit.collider != null)
        {
            List<Vector2> potentialDirections = new List<Vector2>();
            foreach (Vector2Int dirInGrid in Direction2D.eightDirectionsList)
            {
                potentialDirections.Add(new Vector2(dirInGrid.x, dirInGrid.y).normalized);
            }

            for (int i = 0; i < potentialDirections.Count; i++)
            {
                Vector2 temp = potentialDirections[i];
                int randomIndex = Random.Range(i, potentialDirections.Count);
                potentialDirections[i] = potentialDirections[randomIndex];
                potentialDirections[randomIndex] = temp;
            }

            chosenAvoidDirection = -targetDir;
            foreach (Vector2 possibleDir in potentialDirections)
            {
                if (Vector2.Dot(possibleDir, targetDir) > 0.8f) continue;

                RaycastHit2D checkHit = Physics2D.CircleCast(transform.position, zombieRadius, possibleDir,
                    obstacleCheckDistance, obstacleLayer);
                if (checkHit.collider == null)
                {
                    chosenAvoidDirection = possibleDir;
                    break;
                }
            }

            avoidTimeLeft = 0.2f;
            return chosenAvoidDirection;
        }

        return targetDir;
    }

    public void TakeDamage(float damage)
    {
        if (isDead) return;

        if (!isAwake)
        {
            WakeUp();
        }

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
        StartCoroutine(ReturnToPoolAfterDelay(1f));
    }

    private System.Collections.IEnumerator ReturnToPoolAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        ObjectPoolManager.Return(gameObject);
    }

    private void DropLoot()
    {
        if (goldData != null && Random.Range(0f, 100f) <= goldDropChance)
        {
            SpawnItem(goldData, transform.position);
        }

        if (itemPool != null && itemPool.Count > 0 && Random.Range(0f, 100f) <= itemDropChance)
        {
            DropItemData itemToDrop = null;

            if (MissionManager.Instance != null && !MissionManager.Instance.isMissionCompleted)
            {
                List<DropItemData> neededItems = MissionManager.Instance.GetNeededItems();
                if (neededItems != null && neededItems.Count > 0)
                {
                    if (Random.Range(0f, 100f) <= 50f)
                    {
                        itemToDrop = neededItems[Random.Range(0, neededItems.Count)];
                    }
                }
            }

            if (itemToDrop == null)
            {
                int randomIndex = Random.Range(0, itemPool.Count);
                itemToDrop = itemPool[randomIndex];
            }

            Vector2 offsetPos = (Vector2)transform.position + new Vector2(
                Random.Range(-0.5f, 0.5f),
                Random.Range(-0.5f, 0.5f));

            SpawnItem(itemToDrop, offsetPos);
        }

        if (ammoPool != null && ammoPool.Count > 0 && Random.Range(0f, 100f) <= ammoDropChance)
        {
            DropItemData ammoToDrop = ammoPool[Random.Range(0, ammoPool.Count)];
            Vector2 offsetPos = (Vector2)transform.position + new Vector2(
                Random.Range(-0.5f, 0.5f),
                Random.Range(-0.5f, 0.5f));
            SpawnItem(ammoToDrop, offsetPos);
        }
    }

    private void SpawnItem(DropItemData itemData, Vector2 spawnPos)
    {
        if (lootPrefab == null) return;

        GameObject drop = ObjectPoolManager.Spawn(lootPrefab, spawnPos, Quaternion.identity);
        if (drop.TryGetComponent<WorldItem>(out WorldItem wItem))
        {
            wItem.SetupItem(itemData);
        }
    }
}