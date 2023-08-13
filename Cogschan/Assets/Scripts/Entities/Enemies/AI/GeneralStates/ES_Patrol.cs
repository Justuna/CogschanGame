using System;
using UnityEngine;

public abstract class ES_Patrol : MonoBehaviour, IEnemyState
{
    [SerializeField] protected EntityServiceLocator _services;
    [SerializeField] protected float _minPatrolRange;
    [SerializeField] protected float _maxPatrolRange;
    [SerializeField] protected float _patrolPointMetRange;
    [SerializeField] protected float _timeUntilBored;

    public CogschanSimpleEvent LOSMade;
    public CogschanSimpleEvent Bored;

    protected Vector3 _patrolPoint;
    protected bool _hasSetPatrolPoint;
    protected float _boredTimer;

    public virtual void Init()
    {
        _boredTimer = _timeUntilBored;
        _hasSetPatrolPoint = false;
    }

    public virtual void OnBehave()
    {
        if (_services.LOSChecker.CanSee)
        {
            LOSMade?.Invoke();
            return;
        }

        if (!_hasSetPatrolPoint) SearchPatrolPoint();
        if (_hasSetPatrolPoint) MoveToPatrolPoint();

        Vector3 distanceToPatrolPoint = transform.position - _patrolPoint;
        _boredTimer -= Time.deltaTime;

        if (distanceToPatrolPoint.magnitude < _patrolPointMetRange || _boredTimer <= 0)
        {
            Bored?.Invoke();
        }
    }

    protected abstract void SearchPatrolPoint();
    protected abstract void MoveToPatrolPoint();
}
