using System;
using UnityEngine;

public abstract class ES_Patrol : MonoBehaviour, EnemyState
{
    public float MinPatrolRange;
    public float MaxPatrolRange;
    public float PatrolPointMetRange;
    public float TimeUntilBored;

    public LOSCalculator LOS;

    public event Action LOSMade, Bored;

    protected Vector3 _patrolPoint;
    protected bool _hasSetPatrolPoint;
    protected float _boredTimer;

    public virtual void Behavior()
    {
        if (LOS.CanSee)
        {
            LOSMade?.Invoke();
            return;
        }

        if (!_hasSetPatrolPoint) SearchPatrolPoint();
        if (_hasSetPatrolPoint) MoveToPatrolPoint();

        Vector3 distanceToPatrolPoint = transform.position - _patrolPoint;
        _boredTimer -= Time.deltaTime;

        if (distanceToPatrolPoint.magnitude < PatrolPointMetRange || _boredTimer <= 0)
        {
            Bored?.Invoke();
        }
    }

    protected abstract void SearchPatrolPoint();
    protected abstract void MoveToPatrolPoint();

    public virtual void ResetTimer()
    {
        _boredTimer = TimeUntilBored;
    }

    public virtual void ResetPatrolPoint()
    {
        _hasSetPatrolPoint = false;
    }

    protected virtual void OnDrawGizmos()
    {
        if (_hasSetPatrolPoint)
        {
            Gizmos.color = Color.green;
        }
        else
        {
            Gizmos.color = Color.red;
        }

        Gizmos.DrawSphere(_patrolPoint, 0.5f);
    }
}
