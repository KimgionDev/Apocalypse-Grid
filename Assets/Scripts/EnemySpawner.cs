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

    private void Start()
    {
        int level = 1;
        if (SaveManager.Instance != null && SaveManager.Instance.playerStats != null)
        {
            level = SaveManager.Instance.playerStats.currentLevel;
        }

        currentLevelEnemies = baseEnemiesPerWave + ((level - 1) * 3);

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

                Vector2 randomOffset = Random.insideUnitCircle * spawnRadius;
                Vector3 spawnPosition = transform.position + new Vector3(randomOffset.x, randomOffset.y, 0);

                GameObject newEnemy = Instantiate(enemyToSpawn, spawnPosition, Quaternion.identity, transform);
                
                spawnedEnemies.Add(newEnemy);
            }

            yield return new WaitUntil(() => AllEnemiesDead());

            Debug.Log($"Đã dọn sạch đợt quái ({currentLevelEnemies} con)! Sẵn sàng cho đợt tiếp theo...");
            yield return new WaitForSeconds(timeBetweenWaves);
        }
    }

    private bool AllEnemiesDead()
    {
        spawnedEnemies.RemoveAll(enemy => enemy == null);
        return spawnedEnemies.Count == 0;
    }
}