using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class EnemyAttackModule : MonoBehaviour, IAttacker
{
    [Header("Attack Module Properties")]
    public int AttackDamage;

    public abstract void TryAttack();

    public abstract void Attack();
}
