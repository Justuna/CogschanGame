using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasicFlyingEnemyAI : MonoBehaviour
{
    [Header("General Attributes")]
    public Transform Player;
    public Transform Model;

    public LayerMask GroundMask, PlayerMask;

    public float Speed;
    public float OrbitDistance;
    public float PreferredHeight;

    public BasicEnemyState State { get; protected set; }

    protected UnityEngine.AI.NavMeshAgent _agent;

    public float SightRange;
    public float LOSInterruptThreshold;

    protected float _LOSInterruptClock;

    protected bool _canSee;

    protected Rigidbody _rb;

    protected void Awake()
    {
        _agent = GetComponent<UnityEngine.AI.NavMeshAgent>();
        State = BasicEnemyState.Confused;
        _rangedAttackTimer = Random.Range(MinTimeUntilRangedAttack, MaxTimeUntilRangedAttack);
    }

    protected void Update()
    {
        Vector3 dir = Player.position - transform.position;
        Ray ray = new Ray(transform.position, dir);
        RaycastHit hit = new RaycastHit();
        bool LOS = Physics.CheckSphere(transform.position, SightRange, PlayerMask) && !Physics.Raycast(ray, out hit, dir.magnitude, GroundMask);
        Debug.DrawLine(transform.position, hit.point, Color.red);
        if (LOS)
        {
            _LOSInterruptClock = LOSInterruptThreshold;
            _canSee = true;
        }
        else
        {
            if (_LOSInterruptClock > 0)
            {
                _LOSInterruptClock -= Time.deltaTime;
                _canSee = true;
            }
            else _canSee = false;
        }

        Debug.Log(State);
        switch (State)
        {
            case BasicEnemyState.Confused:
                LookAroundConfused();
                break;
            case BasicEnemyState.Patrol:
                Patrolling();
                break;
            case BasicEnemyState.Alerted:
                Alerted();
                break;
            case BasicEnemyState.Chase:
                ChasePlayer();
                break;
            case BasicEnemyState.ChaseLastSeen:
                ChaseLastSeen();
                break;
            case BasicEnemyState.Waiting:
                Waiting();
                break;
        }
    }
        /*if (Vector3.Distance(transform.position, Target.position) <= OrbitDistance) return;

        Vector3 dir = Vector3.Normalize(Target.position - transform.position);
        Ray ray = new Ray(transform.position, Vector3.down);
        if (Physics.Raycast(ray, out RaycastHit hit, PreferredHeight, GroundMask))
        {
            dir.y = Mathf.Max(dir.y, 0);
        }

        _rb.velocity = dir * Speed;*/
}
