using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RammingEnemyAttack : EnemyAttackModule
{
    [Header("Statics")]
    [SerializeField] private EnemyController _enemyController;
    [Header("Properties")]
    [SerializeField] private bool _diesOnImpact = false;
    [Min(0)]
    [SerializeField] private float _ramAttemptCooldown = 3;
    [SerializeField] private float _ramAcceleration = 2000;
    [SerializeField] private bool _respectsBaseMaxVelocity = false;
    [Min(0)]
    [SerializeField] private float _ramDuration = 2;

    private float cooldownTimer = 0;
    private bool attacking = false;
    private Coroutine attackCoroutine;

    private void Update()
    {
        cooldownTimer -= Time.deltaTime;

        if (cooldownTimer <= 0)
        {
            cooldownTimer = 0;
        }
    }

    public override void TryAttack()
    {

        if (cooldownTimer <= 0 && !attacking)
        {
            Attack();
            cooldownTimer = _ramAttemptCooldown;
        }
    }

    public override void Attack()
    {
        Debug.Log($"{gameObject} Attacking!");

        attackCoroutine = StartCoroutine(AttackSequence());
    }

    private IEnumerator AttackSequence()
    {
        // Take over movement input control
        _enemyController.CanMove = false;

        attacking = true;

        // Test to infinitely attack or not
        if (_ramDuration == 0)
        {
            while (true)
            {
                yield return null;
            }
        }
        else
            yield return new WaitForSeconds(_ramDuration);

        attacking = false;
        // Return input to controller
        _enemyController.CanMove = true;
    }

    private void FixedUpdate()
    {
        if (attacking)
        {
            // Ram at target at crazy speeds
            Vector3 attackTargetDir = (PlayerController.Instance.transform.position - transform.position).normalized;
            _enemyController.RB.AddForce(attackTargetDir * _ramAcceleration * Time.deltaTime, ForceMode.Acceleration);

            // Check ti respect or go crazy with max velocity
            if (_respectsBaseMaxVelocity)
                _enemyController.RB.velocity = Vector3.ClampMagnitude(_enemyController.RB.velocity, _enemyController.ChaseMaxSpeed);
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (attacking)
        {
            // Cancel ram once it hits something early
            StopCoroutine(attackCoroutine);
            attackCoroutine = null;

            // Check hitting damageable, but not other enemies
            if (collision.gameObject.TryGetComponent(out IDamageable damageable) && collision.gameObject.GetComponent<EnemyController>() == null)
            {
                damageable.TakeDamage(AttackDamage);
            }

            attacking = false;
            _enemyController.CanMove = true;

            // Check  not to kill-self when colliding with other bubble enemies
            if (_diesOnImpact && collision.gameObject.GetComponent<EnemyController>() == null)
                _enemyController.OnDeath();
        }
    }
}
