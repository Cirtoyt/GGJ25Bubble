using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpaceJunkSpawner : MonoBehaviour
{
    [SerializeField] private List<Rigidbody> _spaceJunkPrefabs = new List<Rigidbody>();
    [Min(0)]
    [SerializeField] private int _minSpawnVolume = 1;
    [Min(0)]
    [SerializeField] private int _maxSpawnVolume = 5;
    [Min(0)]
    [SerializeField] private Vector3 _maxSpawnOffsetRange = Vector3.one;
    [SerializeField] private float _impulseForceOnSpawn = 1;
    [SerializeField] private float _minRotationSpin = 1;
    [SerializeField] private float _maxRotationSpin = 2;
    [Min(0)]
    [SerializeField] private float _minSpawnCooldown = 5;
    [Min(0)]
    [SerializeField] private float _maxSpawnCooldown = 20;
    [Min(0)]
    [SerializeField] private float _spawnCooldownOffsetRandomness = 2;

    private float spawnCooldownTimer = 0;

    private void Awake()
    {
        spawnCooldownTimer = _minSpawnVolume;
    }

    private void Update()
    {
        spawnCooldownTimer -= Time.deltaTime;

        if (spawnCooldownTimer <= 0)
        {
            // Get current difficulty based on player's progress towards the mothership
            float difficulty = GameManager.Instance.GetPlayerProgressTowardsSpaceship();

            // Spawn enemies based on difficulty, adding posistive bias to combat flooring
            float posBias = (1f / (float)(_maxSpawnVolume + 1)) * 0.5f;
            int spawnAmount = _minSpawnVolume + Mathf.FloorToInt((_maxSpawnVolume - _minSpawnVolume) * (difficulty + posBias));
            for (int i = 0; i < spawnAmount; i++)
            {
                Rigidbody prefabToSpawn = _spaceJunkPrefabs[Random.Range(0, _spaceJunkPrefabs.Count)];

                // Position based on offsets & spawn number
                Vector3 spawnPosition = transform.position + (Vector3.back * i * Random.Range(0, _maxSpawnOffsetRange.z))
                                                           + (Vector3.right * i * Random.Range(-_maxSpawnOffsetRange.x, _maxSpawnOffsetRange.x))
                                                           + (Vector3.up * i * Random.Range(-_maxSpawnOffsetRange.y, _maxSpawnOffsetRange.y));

                // Set origin rotation as random
                Quaternion spawnRotation = Quaternion.Euler(Random.Range(0, 360), Random.Range(0, 360), Random.Range(0, 360));

                Rigidbody newSpaceJunk = Instantiate(prefabToSpawn, spawnPosition, spawnRotation);

                // Add initial move force
                newSpaceJunk.AddForce(Vector3.back * _impulseForceOnSpawn, ForceMode.Impulse);

                // Add initial spin force
                newSpaceJunk.AddTorque(new Vector3(Random.Range(_minRotationSpin, _maxRotationSpin), Random.Range(_minRotationSpin, _maxRotationSpin), Random.Range(_minRotationSpin, _maxRotationSpin)), ForceMode.Impulse);
            }

            // Set a new delay based on current difficulty, with a bit of randomness
            spawnCooldownTimer = (_maxSpawnCooldown - ((_maxSpawnCooldown - _minSpawnCooldown) * difficulty)) + Random.Range(-_spawnCooldownOffsetRandomness, _spawnCooldownOffsetRandomness);

            Debug.Log($"Spawned {spawnAmount} space junk! difficulty: {difficulty}, new spawnCooldownTimer = {spawnCooldownTimer}");
        }
    }
}
