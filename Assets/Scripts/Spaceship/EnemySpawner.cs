using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [SerializeField] private EnemyController _enemyToSpawn;
    [SerializeField] private int _minEnemySpawnVolume = 1;
    [SerializeField] private int _maxEnemySpawnVolume = 5;
    [SerializeField] private float _minEnemySpawnCooldown = 5;
    [SerializeField] private float _maxEnemySpawnCooldown = 20;
    [SerializeField] private float _EnemySpawnCooldownOffsetRandomness = 2;

    private float spawnCooldownTimer = 0;

    private void Update()
    {
        spawnCooldownTimer -= Time.deltaTime;

        if (spawnCooldownTimer <= 0)
        {
            spawnCooldownTimer = 0;

            // Get current difficulty based on player's progress towards the mothership
            float difficulty = GameManager.Instance.GetPlayerProgressTowardsSpaceship();

            // Spawn enemies based on difficulty
            int spawnAmount = _minEnemySpawnVolume + Mathf.FloorToInt((_maxEnemySpawnVolume - _minEnemySpawnVolume) * Mathf.Clamp01(difficulty - 0.05f));
            for (int i = 0; i < spawnAmount; i++)
            {
                EnemyController newEnemy = Instantiate(_enemyToSpawn, transform.position, Quaternion.identity);
            }

            // Add a new delay based on current difficulty, with a bit of randomness
            // TODO
        }
    }
}
