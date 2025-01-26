using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [SerializeField] private EnemyController _enemyToSpawn;
    [Min(0)]
    [SerializeField] private int _minEnemySpawnVolume = 1;
    [Min(0)]
    [SerializeField] private int _maxEnemySpawnVolume = 5;
    [Min(0)]
    [SerializeField] private float _minEnemySpawnCooldown = 5;
    [Min(0)]
    [SerializeField] private float _maxEnemySpawnCooldown = 20;
    [Min(0)]
    [SerializeField] private float _enemySpawnCooldownOffsetRandomness = 2;

    private float spawnCooldownTimer = 0;

    private void Awake()
    {
        spawnCooldownTimer = _minEnemySpawnCooldown;
    }

    private void Update()
    {
        spawnCooldownTimer -= Time.deltaTime;

        if (spawnCooldownTimer <= 0)
        {
            // Get current difficulty based on player's progress towards the mothership
            float difficulty = GameManager.Instance.GetPlayerProgressTowardsSpaceship();

            // Spawn enemies based on difficulty, adding posistive bias to combat flooring
            float posBias = (1f / (float)(_maxEnemySpawnVolume + 1)) * 0.5f;
            int spawnAmount = _minEnemySpawnVolume + Mathf.FloorToInt((_maxEnemySpawnVolume - _minEnemySpawnVolume) * (difficulty + posBias));
            for (int i = 0; i < spawnAmount; i++)
            {
                EnemyController newEnemy = Instantiate(_enemyToSpawn, transform.position, Quaternion.identity);
            }

            // Set a new delay based on current difficulty, with a bit of randomness
            spawnCooldownTimer = (_maxEnemySpawnCooldown - ((_maxEnemySpawnCooldown - _minEnemySpawnCooldown) * difficulty)) + Random.Range(-_enemySpawnCooldownOffsetRandomness, _enemySpawnCooldownOffsetRandomness);

            //Debug.Log($"Spawned {spawnAmount} enemies! difficulty: {difficulty}, new spawnCooldownTimer = {spawnCooldownTimer}");
        }
    }
}
