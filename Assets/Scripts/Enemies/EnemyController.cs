using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : MonoBehaviour, IDamageable
{
    [Header("Statics")]
    [SerializeField] private Rigidbody rb;
    [Header("Properties")]
    [SerializeField] private int _lifePoints = 2;
    [SerializeField] private float _chaseAcceleration = 1000;
    [SerializeField] private float _chaseMaxSpeed = 1;
    [SerializeField] private float _uprightCorrectionSmoothingSpeed = 1;
    [SerializeField] private float _playerInRangeDistance = 0;

    private int currentLifePoints;

    private void Awake()
    {
        currentLifePoints = _lifePoints;
    }

    private void FixedUpdate()
    {
        // CHeck to only update movement when not in range
        float distToPlayer = Vector3.Distance(transform.position, PlayerController.Instance.transform.position);
        if (distToPlayer <= _playerInRangeDistance)
            return;

        Vector3 targetDir = (PlayerController.Instance.transform.position - transform.position).normalized;

        rb.AddForce(targetDir * _chaseAcceleration * Time.deltaTime, ForceMode.Acceleration);
        rb.velocity = Vector3.ClampMagnitude(rb.velocity, _chaseMaxSpeed);
    }

    private void Update()
    {
        transform.LookAt(PlayerController.Instance.transform.position, PlayerController.Instance.transform.up);
    }

    public void TakeDamage(int damage)
    {
        currentLifePoints -= damage;

        if (currentLifePoints <= 0)
        {
            OnDeath();
        }
    }

    public void OnDeath()
    {
        // Trigger particle effects

        // Trigger sound clip

        Destroy(gameObject);
    }
}
