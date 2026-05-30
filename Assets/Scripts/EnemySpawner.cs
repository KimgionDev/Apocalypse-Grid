using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [SerializeField] private float spawnInterval = 3f;
    [SerializeField] private int maxEnemiesPerRoom = 10;
    [SerializeField] private float spawnRadius = 2f;
    [SerializeField] private List<GameObject> enemyPrefabs;

    private int currentEnemyCount = 0;

    private void Start()
    {
        StartCoroutine(SpawnEnemyRoutine());
    }

    private IEnumerator SpawnEnemyRoutine()
    {
        while (currentEnemyCount < maxEnemiesPerRoom)
        {
            yield return new WaitForSeconds(spawnInterval);

            int randomIndex = Random.Range(0, enemyPrefabs.Count);
            GameObject enemyToSpawn = enemyPrefabs[randomIndex];

            Vector2 randomOffset = Random.insideUnitCircle * spawnRadius;
            Vector3 spawnPosition = transform.position + new Vector3(randomOffset.x, randomOffset.y, 0);

            Instantiate(enemyToSpawn, spawnPosition, Quaternion.identity, transform);
            currentEnemyCount++;
        }
    }
}