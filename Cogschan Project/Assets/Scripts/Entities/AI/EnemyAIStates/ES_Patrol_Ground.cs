using System;
using UnityEngine;
using UnityEngine.AI;

public class ES_Patrol_Ground : MonoBehaviour, EnemyState
{
    public float MinPatrolRange;
    public float MaxPatrolRange;
    public float PatrolPointMetRange;
    public float TimeUntilBored;

    public NavMeshAgent Agent;
    public LOSCalculator LOS;

    public event Action LOSMade, Bored;

    private Vector3 _patrolPoint;
    private bool _hasSetPatrolPoint;
    private float _boredTimer;

    public void Behavior()
    {
        if (LOS.CanSee)
        {
            LOSMade?.Invoke();
            return;
        }

        if (!_hasSetPatrolPoint) SearchWalkPoint();
        if (_hasSetPatrolPoint)
            Agent.SetDestination(_patrolPoint);

        Vector3 distanceToWalkPoint = transform.position - _patrolPoint;
        _boredTimer -= Time.deltaTime;

        if (distanceToWalkPoint.magnitude < PatrolPointMetRange || _boredTimer <= 0)
        {
            Bored?.Invoke();
        }
    }

    private void SearchWalkPoint()
    {
        NavMeshHit hit;

        NavMeshPath path = new NavMeshPath();

        Vector3 randomDir = Quaternion.Euler(0, UnityEngine.Random.Range(0, 360), 0f) * Vector3.forward;
        float randomDist = UnityEngine.Random.Range(MinPatrolRange, MaxPatrolRange);
        Vector3 randomPos = transform.position + randomDir * randomDist;

        NavMesh.SamplePosition(randomPos, out hit, Mathf.Infinity, NavMesh.AllAreas);

        if (Agent.CalculatePath(hit.position, path) && path.status == NavMeshPathStatus.PathComplete && Vector3.Distance(hit.position, _patrolPoint) > 0.5f)
        {
            _patrolPoint = hit.position;
            _hasSetPatrolPoint = true;
            _boredTimer = TimeUntilBored;
        }
    }

    public void ResetTimer()
    {
        _boredTimer = TimeUntilBored;
    }
}