using UnityEngine;
using UnityEngine.AI;

public enum EnemyState { Patrol, Alerted, Chase, ChaseLastSeen, Confused, WindUpAtk, Attacking, WindDownAtk, PrepShot, Shooting }

public class EnemyAI : MonoBehaviour
{
    [Header("General Attributes")]
    public Transform Player;
    public Transform Model;

    public LayerMask GroundMask, PlayerMask;

    public EnemyState State { get; private set; }

    private NavMeshAgent _agent;

    public float SightRange;
    public float LOSInterruptThreshold;

    private float _LOSInterruptClock;

    private bool _canSee;

    private void Awake()
    {
        _agent = GetComponent<NavMeshAgent>();
        State = EnemyState.Confused;
    }

    private void Update()
    {
        Ray ray = new Ray(transform.position, Player.position - transform.position);
        bool LOS = Physics.CheckSphere(transform.position, SightRange, PlayerMask) && !Physics.Raycast(ray, SightRange, GroundMask);
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
            case EnemyState.Confused:
                LookAroundConfused();
                break;
            case EnemyState.Patrol:
                Patrolling();
                break;
            case EnemyState.Alerted:
                Alerted();
                break;
            case EnemyState.Chase:
                ChasePlayer();
                break;
            case EnemyState.ChaseLastSeen:
                ChaseLastSeen();
                break;
            case EnemyState.WindUpAtk:
                WindUpAttack();
                break;
            case EnemyState.Attacking:
                Attacking();
                break;
            case EnemyState.WindDownAtk:
                WindDownAttack();
                break;
            case EnemyState.PrepShot:
                break;
            case EnemyState.Shooting:
                break;
        }
    }

    #region Confused State Stuff

    public float MinConfusionTime, MaxConfusionTime;

    private float _confusionTimer = 0;

    private void LookAroundConfused()
    {
        if (_canSee)
        {
            State = EnemyState.Alerted;
            _alertTimer = AlertTime;
            return;
        }

        if (_confusionTimer <= 0)
        {
            State = EnemyState.Patrol;
            _boredTimer = TimeUntilBored;
            return;
        }

        _confusionTimer -= Time.deltaTime;
    }

    #endregion

    #region Patrol State Stuff

    [Header("Patrolling State Attributes")]
    public float MinPatrolPointRange, MaxPatrolRange, PatrolPointMetRange;
    public float TimeUntilBored;

    private Vector3 _patrolPoint;
    private Vector3 _randomPoint;
    private Vector3 _attemptPoint;
    private bool _hasSetPatrolPoint;

    private float _boredTimer = 0;

    private void Patrolling()
    {
        if (_canSee)
        {
            _hasSetPatrolPoint = false;
            _agent.ResetPath();
            State = EnemyState.Alerted;
            _alertTimer = AlertTime;
            return;
        }

        if (!_hasSetPatrolPoint) SearchWalkPoint();
        if (_hasSetPatrolPoint)
            _agent.SetDestination(_patrolPoint);

        Vector3 distanceToWalkPoint = transform.position - _patrolPoint;
        _boredTimer -= Time.deltaTime;

        if (distanceToWalkPoint.magnitude < PatrolPointMetRange || _boredTimer <= 0)
        {
            if (_boredTimer <= 0) Debug.Log("Bored now!");
            _hasSetPatrolPoint = false;
            _agent.ResetPath();
            State = EnemyState.Confused;
            _confusionTimer = Random.Range(MinConfusionTime, MaxConfusionTime);
            return;
        }
    }

    private void SearchWalkPoint()
    {
        NavMeshHit hit;
        
        NavMeshPath path = new NavMeshPath();

        Vector3 randomDir = Quaternion.Euler(0, Random.Range(0, 360), 0f) * Vector3.forward;
        float randomDist = Random.Range(MinPatrolPointRange, MaxPatrolRange);
        Vector3 randomPos = transform.position + randomDir * randomDist;
        _randomPoint = randomPos;

        NavMesh.SamplePosition(randomPos, out hit, Mathf.Infinity, NavMesh.AllAreas);
        _attemptPoint = hit.position;

        if (_agent.CalculatePath(hit.position, path) && path.status == NavMeshPathStatus.PathComplete && Vector3.Distance(hit.position, _patrolPoint) > 0.5f)
        {
            _patrolPoint = hit.position;
            _hasSetPatrolPoint = true;
            _boredTimer = TimeUntilBored;
        }
    }

    #endregion

    #region Chase State Stuff

    [Header("Chase State Attributes")]
    public float AlertTime;

    private float _alertTimer = 0;
    private Vector3 _lastSeenPosition;
    private Vector3 _lastSeenNavPos;

    private void Alerted()
    {
        if (!_canSee)
        {
            _hasSetPatrolPoint = false;
            _agent.ResetPath();
            State = EnemyState.Confused;
            _confusionTimer = Random.Range(MinConfusionTime, MaxConfusionTime);
            return;
        }

        _lastSeenPosition = Player.position;

        Model.LookAt(new Vector3(Player.position.x, transform.position.y, Player.position.z));
        Model.Rotate(new Vector3(0, 180, 0));

        _alertTimer -= Time.deltaTime;

        if (_alertTimer <= 0)
        {
            State = EnemyState.Chase;
        }
    }

    private void ChasePlayer()
    {
        if (!_canSee)
        {
            State = EnemyState.ChaseLastSeen;
            return;
        }

        if (Vector3.Distance(transform.position, Player.position) <= AttackRange && HasMeleeAttack)
        {
            _agent.ResetPath();
            _attackTimer = WindUpDuration;
            State = EnemyState.WindUpAtk;
            return;
        }

        _lastSeenPosition = Player.position;

        NavMeshPath path = new NavMeshPath();
        NavMesh.SamplePosition(Player.position, out NavMeshHit hit, Mathf.Infinity, NavMesh.AllAreas);
        bool valid = _agent.CalculatePath(hit.position, path);
        if (valid)
        {
            Vector3 bestTarget = path.corners[path.corners.Length - 1];
            _lastSeenNavPos = bestTarget;
            _agent.SetPath(path);
        }
        
        Model.LookAt(new Vector3(Player.position.x, transform.position.y, Player.position.z));
        Model.Rotate(new Vector3(0, 180, 0));
    }

    private void ChaseLastSeen()
    {
        if (!_canSee && (Vector3.Distance(transform.position, _lastSeenNavPos) <= PatrolPointMetRange))
        {
            _hasSetPatrolPoint = false;
            _agent.ResetPath();
            State = EnemyState.Confused;
            _confusionTimer = Random.Range(MinConfusionTime, MaxConfusionTime);
            return;
        }

        Debug.Log(_agent.pathStatus);

        if (_canSee)
        {
            State = EnemyState.Chase;
            return;
        }

        NavMeshPath path = new NavMeshPath();
        NavMesh.SamplePosition(_lastSeenPosition, out NavMeshHit hit, Mathf.Infinity, NavMesh.AllAreas);
        bool valid = _agent.CalculatePath(hit.position, path);
        if (valid)
        {
            Vector3 bestTarget = path.corners[path.corners.Length - 1];
            _lastSeenNavPos = bestTarget;
            _agent.SetPath(path);
        }

        Model.LookAt(new Vector3(_lastSeenPosition.x, transform.position.y, _lastSeenPosition.z));
        Model.Rotate(new Vector3(0, 180, 0));
    }

    #endregion

    #region Attack State Stuff

    [Header("Attacking State Attributes")]
    public bool HasMeleeAttack;
    public float AttackRange;
    public float WindUpDuration;
    public float AttackDuration;
    public float WindDownDuration;

    private float _attackTimer = 0;

    private void WindUpAttack()
    {
        _attackTimer -= Time.deltaTime;

        if (_canSee)
        {
            Model.LookAt(new Vector3(Player.position.x, transform.position.y, Player.position.z));
            Model.Rotate(new Vector3(0, 180, 0));
        }

        if (_attackTimer <= 0)
        {
            _attackTimer = AttackDuration;
            State = EnemyState.Attacking;
            //Spawn hitbox
        }
    }

    private void Attacking() 
    {
        _attackTimer -= Time.deltaTime;
        if (_attackTimer <= 0)
        {
            _attackTimer = WindDownDuration;
            State = EnemyState.WindDownAtk;
            //Turn off hitbox
        }
    }

    private void WindDownAttack()
    {
        _attackTimer -= Time.deltaTime;
        if (_attackTimer <= 0)
        {
            State = EnemyState.Chase;
        }
    }

    #endregion

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, AttackRange);
        if (State == EnemyState.Alerted || State == EnemyState.Chase || State == EnemyState.ChaseLastSeen ||
            State == EnemyState.WindUpAtk || State == EnemyState.Attacking || State == EnemyState.WindDownAtk) 
            Gizmos.DrawSphere(_lastSeenPosition, 1f);
        else Gizmos.DrawSphere(_randomPoint, 1f);
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, SightRange);
        Gizmos.DrawSphere(_attemptPoint, 1f);
        Gizmos.color = Color.green;
        if (State == EnemyState.Alerted || State == EnemyState.Chase || State == EnemyState.ChaseLastSeen ||
            State == EnemyState.WindUpAtk || State == EnemyState.Attacking || State == EnemyState.WindDownAtk) 
            Gizmos.DrawSphere(_lastSeenNavPos, 1f);
        else Gizmos.DrawSphere(_patrolPoint, 1f);
    }
}
