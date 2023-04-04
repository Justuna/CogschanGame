using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class FlyingEnemyAI : MonoBehaviour
{
    protected EnemyState _state;

    [SerializeField]
    protected ES_Confused Confused;
    [SerializeField]
    protected ES_Alerted Alerted;
    [SerializeField]
    protected ES_Patrol_Flying Patrol;

    protected virtual void Awake()
    {
        Confused.LOSMade += Confused_To_Alerted;
        Confused.Bored += Confused_To_Patrol;
        Alerted.LOSBroken += Alerted_To_Confused;
        Alerted.ReadyToChase += Alerted_To_Chase;
        Patrol.LOSMade += Patrol_To_Alerted;
        Patrol.Bored += Patrol_To_Confused;

        _state = Confused;
    }

    protected virtual void Update()
    {
        _state.Behavior();
    }

    protected virtual void Confused_To_Alerted()
    {
    }

    protected virtual void Confused_To_Patrol()
    {
        Patrol.ResetTimer();
        _state = Patrol;
    }

    protected virtual void Alerted_To_Confused()
    {
    }

    protected virtual void Alerted_To_Chase()
    {
    }

    protected virtual void Patrol_To_Confused()
    {
        Patrol.RB.velocity = Vector3.zero;
        Patrol.ResetPatrolPoint();
        Confused.ResetTimer();
        _state = Confused;
    }

    protected virtual void Patrol_To_Alerted()
    {
    }

    protected abstract void BeginRangedAttack();
    public abstract void EndRangedAttack();
}
