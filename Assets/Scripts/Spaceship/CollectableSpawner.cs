using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollectableSpawner : MonoBehaviour
{
    [SerializeField] private List<Collectable> _collectablesToSpawn = new List<Collectable>();
    [SerializeField] private Vector3 _maxSpawnOffsetRange = Vector3.one;
    [Min(0)]
    [SerializeField] private float _minSpawnCooldown = 5;
    [Min(0)]
    [SerializeField] private float _maxSpawnCooldown = 20;

    private float spawnCooldownTimer = 0;

    private void Awake()
    {
        spawnCooldownTimer = _minSpawnCooldown;
    }

    private void Update()
    {
        spawnCooldownTimer -= Time.deltaTime;

        if (spawnCooldownTimer <= 0)
        {
            // Position based on offsets
            Vector3 spawnPosition = transform.position + (Vector3.back * Random.Range(0, _maxSpawnOffsetRange.z))
                                                       + (Vector3.right * Random.Range(-_maxSpawnOffsetRange.x, _maxSpawnOffsetRange.x))
                                                       + (Vector3.up * Random.Range(-_maxSpawnOffsetRange.y, _maxSpawnOffsetRange.y));

            // Spawn a collectable somweher inside based on difficulty, adding positive bias to combat flooring

            Collectable newCollectable = Instantiate(_collectablesToSpawn[Random.Range(0, _collectablesToSpawn.Count)], spawnPosition, Quaternion.identity);

            // Set a new delay based on current difficulty
            spawnCooldownTimer = _maxSpawnCooldown - ((_maxSpawnCooldown - _minSpawnCooldown) * GameManager.Instance.GetPlayerProgressTowardsSpaceship());

            //Debug.Log($"Spawned {newCollectable.name} collectable! difficulty: {GameManager.Instance.GetPlayerProgressTowardsSpaceship()}, new spawnCooldownTimer = {spawnCooldownTimer}");
        }
    }
}
