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

    private void Start()
    {
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
        StartCoroutine(SpawnWaveRoutine());
    }

    private IEnumerator SpawnWaveRoutine()
    {
        while (true)
        {
            for (int i = 0; i < currentLevelEnemies; i++)
            {
                yield return new WaitForSeconds(spawnInterval);

                int randomIndex = Random.Range(0, enemyPrefabs.Count);
                GameObject enemyToSpawn = enemyPrefabs[randomIndex];

                if (TryGetSpawnPositionNearCenter(Vector2Int.RoundToInt(transform.position), spawnRadius,
                        validRoomFloors, out Vector2Int validSpawnPos))
                {
                    Vector3 spawnPosition = new Vector3(validSpawnPos.x + 0.5f, validSpawnPos.y + 0.5f, 0);
                    GameObject newEnemy;
                    
                    if (ObjectPoolManager.Instance != null)
                    {
                        newEnemy = ObjectPoolManager.Instance.SpawnFromPool(enemyToSpawn, spawnPosition, Quaternion.identity);
                        newEnemy.transform.SetParent(transform);
                    }
                    else
                    {
                        newEnemy = Instantiate(enemyToSpawn, spawnPosition, Quaternion.identity, transform);
                    }
                    
                    spawnedEnemies.Add(newEnemy);
                }
            }

            yield return new WaitUntil(() => AllEnemiesDead());

            Debug.Log($"Đã dọn sạch đợt quái ({currentLevelEnemies} con)! Sẵn sàng cho đợt tiếp theo...");
            yield return new WaitForSeconds(timeBetweenWaves);
        }
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