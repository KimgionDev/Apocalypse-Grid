using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [SerializeField] private float spawnInterval = 3f;
    [SerializeField] private int baseEnemiesPerWave = 10;
    [SerializeField] private float timeBetweenWaves = 5f;
    [SerializeField] private float spawnRadius = 2f;
    [SerializeField] private List<GameObject> enemyPrefabs;

    private List<GameObject> spawnedEnemies = new List<GameObject>();
    private int currentLevelEnemies;
    private HashSet<Vector2Int> validRoomFloors; 

    private bool isTriggered = false;
    private bool isCleared = false;

    private void Awake()
    {
        CircleCollider2D trigger = GetComponent<CircleCollider2D>();
        if (trigger == null)
        {
            trigger = gameObject.AddComponent<CircleCollider2D>();
        }
        trigger.isTrigger = true;
        trigger.radius = 8f; // Bán kính nhận diện Player bước vào phòng

        int level = 1;
        if (SaveManager.Instance != null && SaveManager.Instance.playerStats != null)
        {
            level = SaveManager.Instance.playerStats.currentLevel;
        }

        currentLevelEnemies = baseEnemiesPerWave + ((level - 1) * 3);
    }

    public void InitializeSpawner(HashSet<Vector2Int> floors)
    {
        validRoomFloors = floors;
        SpawnAllEnemies();
    }

    private void SpawnAllEnemies()
    {
        for (int i = 0; i < currentLevelEnemies; i++)
        {
            int randomIndex = Random.Range(0, enemyPrefabs.Count);
            GameObject enemyToSpawn = enemyPrefabs[randomIndex];

            if (TryGetSpawnPositionNearCenter(Vector2Int.RoundToInt(transform.position), spawnRadius,
                    validRoomFloors, out Vector2Int validSpawnPos))
            {
                Vector3 spawnPosition = new Vector3(validSpawnPos.x + 0.5f, validSpawnPos.y + 0.5f, 0);
                GameObject newEnemy = ObjectPoolManager.Spawn(enemyToSpawn, spawnPosition, Quaternion.identity);
                newEnemy.transform.SetParent(transform);
                
                spawnedEnemies.Add(newEnemy);
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (isTriggered || isCleared) return;

        if (collision.CompareTag(Tags.Player))
        {
            isTriggered = true;
            Debug.Log("Ổ Quái Vật đã bị đánh thức!");
            
            foreach (GameObject enemy in spawnedEnemies)
            {
                if (enemy != null && enemy.activeInHierarchy)
                {
                    if (enemy.TryGetComponent<ZombieAI>(out ZombieAI zombie))
                    {
                        zombie.WakeUp();
                    }
                }
            }
            
            StartCoroutine(CheckClearedRoutine());
        }
    }

    private IEnumerator CheckClearedRoutine()
    {
        yield return new WaitUntil(() => AllEnemiesDead());

        if (MissionManager.Instance != null && MissionManager.Instance.isMissionCompleted)
        {
            isCleared = true;
            Debug.Log("Phòng đã an toàn vĩnh viễn! Nhiệm vụ đã hoàn thành.");
            yield break;
        }

        Debug.Log("Quái đã chết hết nhưng nhiệm vụ chưa xong. Chờ 30s để tái sinh...");
        yield return new WaitForSeconds(30f);

        if (MissionManager.Instance != null && MissionManager.Instance.isMissionCompleted)
        {
            isCleared = true;
            Debug.Log("Phòng đã an toàn vĩnh viễn! Nhiệm vụ đã hoàn thành.");
            yield break;
        }

        Debug.Log("Quái vật bắt đầu hồi sinh lén lút trong phòng...");
        isTriggered = false;
        SpawnAllEnemies();
    }

    public bool TryGetSpawnPositionNearCenter(Vector2Int roomCenter, float spawnRadius, HashSet<Vector2Int> validFloors, out Vector2Int spawnPos)
    {
        spawnPos = Vector2Int.zero;
        int maxAttempts = 30; 

        for (int i = 0; i < maxAttempts; i++)
        {
            Vector2 randomOffset = Random.insideUnitCircle * spawnRadius;

            Vector2Int checkPos = new Vector2Int(
                roomCenter.x + Mathf.RoundToInt(randomOffset.x),
                roomCenter.y + Mathf.RoundToInt(randomOffset.y)
            );

            if (validFloors.Contains(checkPos))
            {
                spawnPos = checkPos;
                return true; 
            }
        }
        return false;
    }

    private bool AllEnemiesDead()
    {
        spawnedEnemies.RemoveAll(enemy => enemy == null || !enemy.activeInHierarchy);
        return spawnedEnemies.Count == 0;
    }
}