using System;
using UnityEngine;
using UnityEngine.AI;

public class ES_Patrol_Ground : ES_Patrol
{
    public NavMeshAgent Agent;

    protected override void SearchPatrolPoint()
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

    protected override void MoveToPatrolPoint()
    {
        Agent.SetDestination(_patrolPoint);
    }
}