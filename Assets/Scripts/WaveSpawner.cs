using System.Collections;
using UnityEngine;

public class WaveSpawner : MonoBehaviour
{
    [System.Serializable]
    public class Wave
    {
        public int totalEnemies;        // Total enemies in this wave
        public float timeBetweenSpawns; // Time between each enemy spawn
    }

    [Header("Spawn Points")]
    [SerializeField] private Transform[] spawnPoints;

    [Header("Enemy Prefabs")]
    [SerializeField] private GameObject[] enemyPrefabs;  // Array of different enemy prefabs

    [Header("Wave Settings")]
    [SerializeField] private Wave[] waves;

    [SerializeField] private MainMenuController mainMenuController;

    private int _currentWave = 0;
    private int _remainingEnemies = 0;
    private bool _isSpawning = false;

    private void Start()
    {
        if (spawnPoints.Length == 0)
        {
            Debug.LogError("No spawn points assigned to WaveSpawner!");
            return;
        }

        if (enemyPrefabs == null || enemyPrefabs.Length == 0)
        {
            Debug.LogError("No enemy prefabs assigned to WaveSpawner!");
            return;
        }

        StartNextWave();
    }

    private void StartNextWave()
    {
        if (_currentWave >= waves.Length)
        {
            Debug.Log("All waves completed!");
            Debug.Log("VICTORY!");
            mainMenuController.Back();
            return;
        }

        Wave wave = waves[_currentWave];
        _remainingEnemies = wave.totalEnemies;
        Debug.Log($"Wave {_currentWave + 1} starting! Enemies to spawn: {_remainingEnemies}");

        StartCoroutine(SpawnWaveEnemies(wave));
    }

    private IEnumerator SpawnWaveEnemies(Wave wave)
    {
        _isSpawning = true;

        for (int i = 0; i < wave.totalEnemies; i++)
        {
            SpawnEnemy();
            yield return new WaitForSeconds(wave.timeBetweenSpawns);
        }

        _isSpawning = false;
    }

    private void SpawnEnemy()
    {
        // Get a random spawn point
        Transform spawnPoint = spawnPoints[Random.Range(0, spawnPoints.Length)];

        // Get a random enemy prefab
        GameObject randomEnemyPrefab = enemyPrefabs[Random.Range(0, enemyPrefabs.Length)];

        // Spawn the random enemy
        GameObject enemy = Instantiate(randomEnemyPrefab, spawnPoint.position, spawnPoint.rotation);

        // Get the Enemy component
        Enemy enemyComponent = enemy.GetComponent<Enemy>();
        if (enemyComponent != null)
        {
            StartCoroutine(WatchEnemyDestruction(enemy));
        }
    }

    private IEnumerator WatchEnemyDestruction(GameObject enemy)
    {
        yield return new WaitUntil(() => enemy == null);
        _remainingEnemies--;

        Debug.Log($"Enemy destroyed! Remaining enemies: {_remainingEnemies}");

        if (_remainingEnemies <= 0 && !_isSpawning)
        {
            Debug.Log($"Wave {_currentWave + 1} completed!");
            _currentWave++;
            StartNextWave();
        }
    }

    public int GetCurrentWave()
    {
        return _currentWave + 1;
    }

    public int GetRemainingEnemies()
    {
        return _remainingEnemies;
    }
}