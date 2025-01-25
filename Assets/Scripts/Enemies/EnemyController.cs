using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : MonoBehaviour, IDamageable
{
    [Header("Statics")]
    [SerializeField] private Rigidbody rb;
    [SerializeField] private EnemyAttackModule attackModule;
    [Header("Properties")]
    [SerializeField] private int _lifePoints = 2;
    [SerializeField] private float _chaseAcceleration = 1000;
    [SerializeField] private float _chaseMaxSpeed = 1;
    [SerializeField] private float _uprightCorrectionSmoothingSpeed = 1;
    [SerializeField] private float _playerInRangeDistance = 0;
    [SerializeField] private float _deathSoundDuration = 1;

    public Rigidbody RB => rb;
    public float ChaseMaxSpeed => _chaseMaxSpeed;

    public bool CanMove = true;

    private int currentLifePoints;

    private void Awake()
    {
        currentLifePoints = _lifePoints;
    }

    private void FixedUpdate()
    {
        if (currentLifePoints <= 0)
            return;

        if (!CanMove)
            return;

        // Check to only move towards target when not in range
        float distToPlayer = Vector3.Distance(transform.position, PlayerController.Instance.transform.position);
        if (distToPlayer <= _playerInRangeDistance)
            return;

        // Move towards player
        Vector3 targetDir = (PlayerController.Instance.transform.position - transform.position).normalized;

        rb.AddForce(targetDir * _chaseAcceleration * Time.deltaTime, ForceMode.Acceleration);
        rb.velocity = Vector3.ClampMagnitude(rb.velocity, _chaseMaxSpeed);
    }

    private void Update()
    {
        if (currentLifePoints <= 0)
            return;

        // Try attack when in-range
        float distToPlayer = Vector3.Distance(transform.position, PlayerController.Instance.transform.position);
        if (distToPlayer <= _playerInRangeDistance)
            attackModule.TryAttack();

        // Rotate towards player, staying up-right in alignment with player's up direction
        transform.LookAt(PlayerController.Instance.transform.position, PlayerController.Instance.transform.up);
    }

    public void TakeDamage(int damage)
    {
        if (currentLifePoints <= 0)
            return;

        // Reduce life points
        currentLifePoints -= damage;

        if (currentLifePoints <= 0)
        {
            OnDeath();
        }
    }

    public void OnDeath()
    {
        Debug.Log($"{gameObject} OnDeath!");

        // Trigger particle effects

        // Trigger sound clip

        gameObject.SetActive(false);
        Destroy(gameObject, _deathSoundDuration);
    }
}
