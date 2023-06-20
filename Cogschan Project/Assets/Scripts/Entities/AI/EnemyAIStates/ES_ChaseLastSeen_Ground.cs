using System;
using UnityEngine;
using UnityEngine.AI;

public class ES_ChaseLastSeen_Ground : MonoBehaviour, EnemyState
{
    public LOSCalculator LOS;
    public NavMeshAgent Agent;
    public GameObject Model;

    public float PatrolPointMetRange;

    public event Action LOSMade, LOSBroken;

    private Vector3 _lastSeenNavPos;

    public void Behavior()
    {
        NavMeshPath path = new NavMeshPath();
        NavMesh.SamplePosition(LOS.LastSeenPosition, out NavMeshHit hit, Mathf.Infinity, NavMesh.AllAreas);
        bool valid = Agent.CalculatePath(hit.position, path);
        if (valid)
        {
            Vector3 bestTarget = path.corners[path.corners.Length - 1];
            _lastSeenNavPos = bestTarget;
            Agent.SetPath(path);
        }

        if (!LOS.CanSee && (Vector3.Distance(transform.position, _lastSeenNavPos) <= PatrolPointMetRange))
        {
            LOSBroken?.Invoke();
            return;
        }

        if (LOS.CanSee)
        {
            LOSMade?.Invoke();
            return;
        }

        Model.transform.LookAt(new Vector3(LOS.LastSeenPosition.x, transform.position.y, LOS.LastSeenPosition.z));
        Model.transform.Rotate(new Vector3(0, 180, 0));
    }
}
