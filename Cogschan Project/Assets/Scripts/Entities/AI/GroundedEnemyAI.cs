using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class GroundedEnemyAI : MonoBehaviour
{
    protected EnemyState _state;

    [SerializeField]
    protected ES_Confused Confused;
    [SerializeField]
    protected ES_Alerted Alerted;
    [SerializeField]
    protected ES_Patrol_Ground Patrol;
    [SerializeField]
    protected ES_Chase_Ground Chase;
    [SerializeField]
    protected ES_ChaseLastSeen_Ground ChaseLastSeen;
    [SerializeField]
    protected ES_MeleeAttack MeleeAttack;
    [SerializeField]
    protected ES_RangedAttack RangedAttack;
    [SerializeField]
    protected ES_Waiting Waiting;
    [SerializeField]
    protected bool HasMeleeAttack, HasRangedAttack;

    protected virtual void Awake()
    {
        Confused.LOSMade += Confused_To_Alerted;
        Confused.Bored += Confused_To_Patrol;
        Alerted.LOSBroken += Alerted_To_Confused;
        Alerted.ReadyToChase += Alerted_To_Chase;
        Patrol.LOSMade += Patrol_To_Alerted;
        Patrol.Bored += Patrol_To_Confused;
        Chase.LOSBroken += Chase_To_ChaseLastSeen;
        ChaseLastSeen.LOSBroken += ChaseLastSeen_To_Confused;
        ChaseLastSeen.LOSMade += ChaseLastSeen_To_Chase;
        Waiting.OutOfRange += Waiting_To_Chase;

        if (HasMeleeAttack)
        {
            Chase.MeleeAttack += Chase_To_MeleeAttack;
        }
        else
        {
            Chase.MeleeAttack += Skip_MeleeAttack;
        }
        
        if (HasRangedAttack)
        {
            Chase.RangedAttack += Chase_To_RangedAttack;
        }
        else
        {
            Chase.RangedAttack += Skip_RangedAttack;
        }

        _state = Confused;
    }

    protected virtual void Update()
    {
        _state.Behavior();
    }

    protected virtual void Confused_To_Alerted()
    {
        Alerted.ResetTimer();
        _state = Alerted;
    }

    protected virtual void Confused_To_Patrol()
    {
        Patrol.ResetTimer();
        Patrol.ResetPatrolPoint();
        _state = Patrol;
    }

    protected virtual void Alerted_To_Confused()
    {
        Confused.ResetTimer();
        _state = Confused;
    }

    protected virtual void Alerted_To_Chase()
    {
        _state = Chase;
    }

    protected virtual void Patrol_To_Confused()
    {
        Patrol.Agent.ResetPath();
        Patrol.ResetPatrolPoint();
        Confused.ResetTimer();
        _state = Confused;
    }

    protected virtual void Patrol_To_Alerted()
    {
        Patrol.Agent.ResetPath();
        Patrol.ResetPatrolPoint();
        Alerted.ResetTimer();
        _state = Alerted;
    }

    protected virtual void Chase_To_ChaseLastSeen()
    {
        _state = ChaseLastSeen;
    }

    protected virtual void Chase_To_MeleeAttack()
    {
        Chase.Agent.ResetPath();
        BeginMeleeAttack();
        _state = MeleeAttack;
    }

    protected virtual void Chase_To_RangedAttack()
    {
        Chase.Agent.ResetPath();
        Chase.ResetRangedAttackTimer();
        BeginRangedAttack();
        _state = RangedAttack;
    }

    protected virtual void Skip_MeleeAttack()
    {
        Chase.Agent.ResetPath();
        _state = Waiting;
    }

    protected virtual void Skip_RangedAttack()
    {
        Chase.ResetRangedAttackTimer();
    }

    protected virtual void ChaseLastSeen_To_Confused()
    {
        ChaseLastSeen.Agent.ResetPath();
        Confused.ResetTimer();
        _state = Confused;
    }

    protected virtual void ChaseLastSeen_To_Chase()
    {
        _state = Chase;
    }

    protected virtual void Waiting_To_Chase()
    {
        _state = Chase;
    }

    protected abstract void BeginMeleeAttack();
    public abstract void EndMeleeAttack();

    protected abstract void BeginRangedAttack();
    public abstract void EndRangedAttack();
}
