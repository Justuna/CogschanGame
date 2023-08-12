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
    [SerializeField] protected float _turnSpeed;

    public float Speed => _speed;
    public float TurnSpeed => _turnSpeed;

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

        if (_hasRangedAttack)
        {
            es_Chase.InAttackRange += ChaseToRangedAttack;
            es_RangedAttack.AttackTerminated += RangedAttackToChase;
        }
        else
        {
            es_Chase.InAttackRange += SkipRangedAttack;
        }

        _state = es_Confused;
    }

    protected virtual void Update()
    {
        _state.Behavior();
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

    protected virtual void ChaseToMeleeAttack()
    {
        BeginMeleeAttack();
        _state = es_MeleeAttack;
    }

    protected virtual void MeleeAttackToChase()
    {
        es_Chase.Init();
        _state = es_Chase;
        EndMeleeAttack();
    }

    protected virtual void ChaseToRangedAttack()
    {
        BeginRangedAttack();
        _state = es_RangedAttack.Init();
    }

    protected virtual void RangedAttackToChase()
    {
        es_Chase.Init();
        _state = es_Chase;
        EndRangedAttack();
    }

    protected virtual void SkipMeleeAttack()
    {
        _state = es_Chase;
    }

    protected virtual void SkipRangedAttack()
    {
        _state = es_Chase;
    }

    public abstract void BeginMeleeAttack();
    public abstract void EndMeleeAttack();

    public abstract void BeginRangedAttack();
    public abstract void EndRangedAttack();
}