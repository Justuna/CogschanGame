using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class GroundedEnemyAI : MonoBehaviour
{
    protected EnemyState _state;

    public ES_Confused Confused;
    public ES_Alerted Alerted;
    public ES_Patrol_Ground Patrol;
    public ES_Chase_Ground Chase;
    public ES_ChaseLastSeen_Ground ChaseLastSeen;
    public ES_MeleeAttack MeleeAttack;
    public ES_RangedAttack RangedAttack;
    public ES_Waiting Waiting;

    public bool HasMeleeAttack, HasRangedAttack;

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

    }

    protected virtual void Patrol_To_Alerted()
    {

    }

    protected virtual void Chase_To_ChaseLastSeen()
    {

    }

    protected virtual void Chase_To_MeleeAttack()
    {

    }

    protected virtual void Chase_To_RangedAttack()
    {

    }

    protected virtual void Skip_MeleeAttack()
    {

    }

    protected virtual void Skip_RangedAttack()
    {

    }

    protected virtual void ChaseLastSeen_To_Confused()
    {

    }

    protected virtual void ChaseLastSeen_To_Chase()
    {

    }

    protected virtual void Waiting_To_Chase()
    {

    }

    protected abstract void BeginMeleeAttack();
    public abstract void EndMeleeAttack();

    protected abstract void BeginRangedAttack();
    public abstract void EndRangedAttack();
}
