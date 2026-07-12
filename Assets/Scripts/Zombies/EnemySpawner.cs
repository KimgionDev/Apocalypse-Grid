using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [SerializeField] private int baseEnemiesPerWave = 10;
    [SerializeField] private float timeBetweenWaves = 20f;
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
        trigger.radius = 8f;

        int level = 1;
        if (SaveManager.Instance != null && SaveManager.Instance.playerStats != null)
        {
            level = SaveManager.Instance.playerStats.currentLevel;
        }

        currentLevelEnemies = baseEnemiesPerWave + (level - 1);
    }

    public void InitializeSpawner(HashSet<Vector2Int> floors)
    {
        validRoomFloors = floors;
        SpawnAllEnemies();
    }

    private void SpawnAllEnemies()
    {
        List<Vector2Int> floorList = new List<Vector2Int>(validRoomFloors);
        if (floorList.Count == 0) return;

        for (int i = 0; i < currentLevelEnemies; i++)
        {
            if (floorList.Count == 0) break;

            int randomIndex = Random.Range(0, enemyPrefabs.Count);
            GameObject enemyToSpawn = enemyPrefabs[randomIndex];

            int randomTileIndex = Random.Range(0, floorList.Count);
            Vector2Int validSpawnPos = floorList[randomTileIndex];
            
            floorList.RemoveAt(randomTileIndex);

            Vector3 spawnPosition = new Vector3(validSpawnPos.x + 0.5f, validSpawnPos.y + 0.5f, 0);
            GameObject newEnemy = ObjectPoolManager.Spawn(enemyToSpawn, spawnPosition, Quaternion.identity);
            newEnemy.transform.SetParent(transform);
            
            spawnedEnemies.Add(newEnemy);
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
        yield return new WaitForSeconds(timeBetweenWaves);

        if (MissionManager.Instance != null && MissionManager.Instance.isMissionCompleted)
        {
            isCleared = true;
            Debug.Log("Phòng đã an toàn vĩnh viễn! Nhiệm vụ đã hoàn thành.");
            yield break;
        }

        Debug.Log("Quái vật bắt đầu hồi sinh trong phòng...");
        isTriggered = false;
        SpawnAllEnemies();
    }



    private bool AllEnemiesDead()
    {
        spawnedEnemies.RemoveAll(enemy => enemy == null || !enemy.activeInHierarchy);
        return spawnedEnemies.Count == 0;
    }
}