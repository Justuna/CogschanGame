using System;
using UnityEngine;

public abstract class FlyingEnemyAI : MonoBehaviour
{
    [Header("States")]
    [SerializeField] protected ES_Confused es_Confused;
    [SerializeField] protected ES_Alerted es_Alerted;
    [SerializeField] protected ES_PatrolFlying es_Patrol;
    [SerializeField] protected ES_ChaseFlying es_Chase;
    [SerializeField] protected ES_MeleeAttack es_MeleeAttack;
    [SerializeField] protected ES_RangedAttack es_RangedAttack;
    [Header("Other Attributes")]
    [SerializeField] protected bool _hasMeleeAttack;
    [SerializeField] protected bool _hasRangedAttack;
    [SerializeField] protected float _speed;

    public float Speed => _speed;

    protected IEnemyState _state;

    protected virtual void Awake()
    {
        es_Confused.LOSMade += ConfusedToAlerted;
        es_Confused.Bored += ConfusedToPatrol;
        es_Alerted.LOSBroken += AlertedToConfused;
        es_Alerted.ReadyToChase += AlertedToChase;
        es_Patrol.LOSMade += PatrolToAlerted;
        es_Patrol.Bored += PatrolToConfused;
        es_Chase.LostPlayer += ChaseToConfused;
        es_Chase.InAttackRange += ChaseToAttack;

        _state = es_Confused;
    }

    protected virtual void Update()
    {
        _state.Behavior();

        print(_state.GetType().ToString());
    }

    protected virtual void ConfusedToAlerted()
    {
        es_Alerted.ResetTimer();
        _state = es_Alerted;
    }

    protected virtual void ConfusedToPatrol()
    {
        es_Patrol.Init();
        _state = es_Patrol;
    }

    protected virtual void AlertedToConfused()
    {
        es_Confused.Init();
        _state = es_Confused;
    }

    protected virtual void AlertedToChase()
    {
        _state = es_Chase;
    }

    protected virtual void PatrolToConfused()
    {
        es_Confused.Init();
        _state = es_Confused;
    }

    protected virtual void PatrolToAlerted()
    {
        es_Alerted.ResetTimer();
        _state = es_Alerted;
    }

    protected virtual void ChaseToConfused()
    {
        es_Confused.Init();
        _state = es_Confused;
    }

#pragma warning disable CS0162 // Unreachable code detected
    protected virtual void ChaseToAttack()
    {
        if (!_hasMeleeAttack && !_hasRangedAttack)
            throw new InvalidOperationException("The enemy has no attacks and can not enter the attack state.");
        throw new NotImplementedException("Entering the attack state would cause the enemy to get stuck, so lets not do that right now.");
        _state = _hasMeleeAttack ? es_MeleeAttack : es_RangedAttack;
    }
#pragma warning restore CS0162 // Unreachable code detected

    protected abstract void BeginMeleeAttack();
    public abstract void EndMeleeAttack();

    protected abstract void BeginRangedAttack();
    public abstract void EndRangedAttack();
}