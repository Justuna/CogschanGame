using System;
using UnityEngine;
using UnityEngine.AI;

public class ES_Chase_Ground : MonoBehaviour, EnemyState
{
    public float MeleeAttackRange, MinTimeUntilRangedAttack, MaxTimeUntilRangedAttack;

    public LOSCalculator LOS;
    public NavMeshAgent Agent;
    public GameObject Model;

    public event Action LOSBroken, RangedAttack, MeleeAttack;

    private float _rangedAttackTimer;

    public void Awake()
    {
        _rangedAttackTimer = UnityEngine.Random.Range(MinTimeUntilRangedAttack, MaxTimeUntilRangedAttack);
    }

    public void Behavior()
    {
        if (!LOS.CanSee)
        {
            LOSBroken?.Invoke();
            return;
        }

        _rangedAttackTimer -= Time.deltaTime;
        if (_rangedAttackTimer <= 0)
        {
            RangedAttack?.Invoke();
            return;
        }

        if (Vector3.Distance(transform.position, LOS.LastSeenPosition) <= MeleeAttackRange)
        {
            MeleeAttack?.Invoke();
            return;
        }

        NavMeshPath path = new NavMeshPath();
        NavMesh.SamplePosition(LOS.LastSeenPosition, out NavMeshHit hit, Mathf.Infinity, NavMesh.AllAreas);
        bool valid = Agent.CalculatePath(hit.position, path);
        if (valid)
        {
            Vector3 bestTarget = path.corners[path.corners.Length - 1];
            Agent.SetPath(path);
        }

        Model.transform.LookAt(new Vector3(LOS.LastSeenPosition.x, transform.position.y, LOS.LastSeenPosition.z));
        Model.transform.Rotate(new Vector3(0, 180, 0));
    }
}
