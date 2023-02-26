using UnityEngine;
using UnityEngine.AI;

public enum BasicEnemyState { Patrol, Alerted, Chase, ChaseLastSeen, Confused, Waiting, Attacking, Shooting }

public abstract class BasicEnemyAI : MonoBehaviour
{
    [Header("General Attributes")]
    public Transform Player;
    public Transform Model;

    public LayerMask GroundMask, PlayerMask;

    public BasicEnemyState State { get; protected set; }

    protected NavMeshAgent _agent;

    public float SightRange;
    public float LOSInterruptThreshold;

    protected float _LOSInterruptClock;

    protected bool _canSee;

    protected void Awake()
    {
        _agent = GetComponent<NavMeshAgent>();
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

    #region Confused State Stuff

    public float MinConfusionTime, MaxConfusionTime;

    protected float _confusionTimer = 0;

    protected void LookAroundConfused()
    {
        if (_canSee)
        {
            State = BasicEnemyState.Alerted;
            _alertTimer = AlertTime;
            return;
        }

        if (_confusionTimer <= 0)
        {
            State = BasicEnemyState.Patrol;
            _boredTimer = TimeUntilBored;
            return;
        }

        _confusionTimer -= Time.deltaTime;
    }

    #endregion

    #region Patrol State Stuff

    [Header("Patrolling State Attributes")]
    public float MinPatrolRange;
    public float MaxPatrolRange;
    public float PatrolPointMetRange;
    public float TimeUntilBored;

    protected Vector3 _patrolPoint;
    protected Vector3 _randomPoint;
    protected Vector3 _attemptPoint;
    protected bool _hasSetPatrolPoint;

    protected float _boredTimer = 0;

    protected void Patrolling()
    {
        if (_canSee)
        {
            _hasSetPatrolPoint = false;
            _agent.ResetPath();
            State = BasicEnemyState.Alerted;
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
            State = BasicEnemyState.Confused;
            _confusionTimer = Random.Range(MinConfusionTime, MaxConfusionTime);
            return;
        }
    }

    protected void SearchWalkPoint()
    {
        NavMeshHit hit;
        
        NavMeshPath path = new NavMeshPath();

        Vector3 randomDir = Quaternion.Euler(0, Random.Range(0, 360), 0f) * Vector3.forward;
        float randomDist = Random.Range(MinPatrolRange, MaxPatrolRange);
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

    protected float _alertTimer = 0;
    protected Vector3 _lastSeenPosition;
    protected Vector3 _lastSeenNavPos;

    protected void Alerted()
    {
        if (!_canSee)
        {
            _hasSetPatrolPoint = false;
            _agent.ResetPath();
            State = BasicEnemyState.Confused;
            _confusionTimer = Random.Range(MinConfusionTime, MaxConfusionTime);
            return;
        }

        _lastSeenPosition = Player.position;

        Model.LookAt(new Vector3(Player.position.x, transform.position.y, Player.position.z));
        Model.Rotate(new Vector3(0, 180, 0));

        _alertTimer -= Time.deltaTime;

        if (_alertTimer <= 0)
        {
            State = BasicEnemyState.Chase;
        }
    }

    protected void ChasePlayer()
    {
        if (!_canSee)
        {
            State = BasicEnemyState.ChaseLastSeen;
            return;
        }

        if (HasRangedAttack)
        {
            _rangedAttackTimer -= Time.deltaTime;
            if (_rangedAttackTimer <= 0)
            {
                _agent.ResetPath();
                _rangedAttackTimer = Random.Range(MinTimeUntilRangedAttack, MaxTimeUntilRangedAttack);
                State = BasicEnemyState.Shooting;
                BeginRangedAttack();
                return;
            }
        }

        if (Vector3.Distance(transform.position, Player.position) <= AttackRange)
        {
            if (HasMeleeAttack)
            {
                _agent.ResetPath();
                State = BasicEnemyState.Attacking;
                BeginAttack();
            }
            else
            {
                _agent.ResetPath();
                State = BasicEnemyState.Waiting;
            }
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

    protected void ChaseLastSeen()
    {
        if (!_canSee && (Vector3.Distance(transform.position, _lastSeenNavPos) <= PatrolPointMetRange))
        {
            _hasSetPatrolPoint = false;
            _agent.ResetPath();
            State = BasicEnemyState.Confused;
            _confusionTimer = Random.Range(MinConfusionTime, MaxConfusionTime);
            return;
        }

        if (_canSee)
        {
            State = BasicEnemyState.Chase;
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

    protected void Waiting()
    {
        if (!_canSee || Vector3.Distance(transform.position, Player.position) > AttackRange)
        {
            State = BasicEnemyState.Chase;
        }
    }

    protected abstract void BeginAttack();

    #endregion

    #region Ranged Attack State Stuff

    [Header("Ranged Attack State Attributes")]
    public bool HasRangedAttack;
    public float MinTimeUntilRangedAttack, MaxTimeUntilRangedAttack;

    protected float _rangedAttackTimer = 0;

    protected abstract void BeginRangedAttack();

    #endregion

    protected void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, AttackRange);
        if (State == BasicEnemyState.Alerted || State == BasicEnemyState.Chase || State == BasicEnemyState.ChaseLastSeen ||
            State == BasicEnemyState.Attacking) 
            Gizmos.DrawSphere(_lastSeenPosition, 1f);
        else Gizmos.DrawSphere(_randomPoint, 1f);
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, SightRange);
        Gizmos.DrawSphere(_attemptPoint, 1f);
        Gizmos.color = Color.green;
        if (State == BasicEnemyState.Alerted || State == BasicEnemyState.Chase || State == BasicEnemyState.ChaseLastSeen ||
            State == BasicEnemyState.Attacking) 
            Gizmos.DrawSphere(_lastSeenNavPos, 1f);
        else Gizmos.DrawSphere(_patrolPoint, 1f);
    }
}
