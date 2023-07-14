using System;
using UnityEngine;
using UnityEngine.AI;

public class ES_ChaseGround : MonoBehaviour, IEnemyState
{
    [SerializeField] private EntityServiceLocator _services;
    [SerializeField] private float _cornerDistance;
    [SerializeField] private float _meleeAttackRange;
    [SerializeField] private float _minTimeUntilRangedAttack;
    [SerializeField] private float _maxTimeUntilRangedAttack;
    [SerializeField] private float _recalculateTime;

    public CogschanSimpleEvent LostPlayer;
    public CogschanSimpleEvent RangedAttack;
    public CogschanSimpleEvent MeleeAttack;

    private NavMeshPath _path;
    private int _currentCorner;
    private float _rangedAttackTimer;
    private float _recalculateTimer;

    private void Start()
    {
        _rangedAttackTimer = UnityEngine.Random.Range(_minTimeUntilRangedAttack, _maxTimeUntilRangedAttack);
        _path = new NavMeshPath();
    }

    public void Init()
    {
        _rangedAttackTimer = UnityEngine.Random.Range(_minTimeUntilRangedAttack, _maxTimeUntilRangedAttack);
    }

    public void Behavior()
    {
        // Every once in a while, try to make a ranged attack
        // Only if you can actually see the target, though
        _rangedAttackTimer -= Time.deltaTime;
        if (_rangedAttackTimer <= 0 && _services.LOSChecker.CanSee)
        {
            RangedAttack?.Invoke();
            return;
        }

        // If you're within a certain distance of the last seen position, then you've arrived at the destination
        if (Vector3.Distance(transform.position, _services.LOSChecker.LastSeenPosition) <= _meleeAttackRange)
        {
            // If you do find the player, attack them
            if (_services.LOSChecker.CanSee)
            {
                MeleeAttack?.Invoke();
                return;
            }
            // If you don't find the player, then you need to go patrol for them
            else
            {
                LostPlayer?.Invoke();
                return;
            }
        }

        // Since the player is a moving target, recalculate every so often
        // Even if you lose sight of the player, going to the last seen position gives the best chance of finding them again
        // If you can't reach the player, accept the path anyway to get as close as possible
        _recalculateTimer -= Time.deltaTime;
        if (_recalculateTimer <= 0)
        {
            NavMesh.SamplePosition(transform.position, out NavMeshHit hit, Mathf.Infinity, NavMesh.AllAreas);
            NavMesh.SamplePosition(_services.LOSChecker.LastSeenPosition, out NavMeshHit hit2, Mathf.Infinity, NavMesh.AllAreas);
            NavMesh.CalculatePath(hit.position, hit2.position, NavMesh.AllAreas, _path);
            _recalculateTimer = _recalculateTime;
            _currentCorner = 0;
        }

        Vector3 targetPosition = _path.corners[_currentCorner];

        if (Vector3.Distance(targetPosition, transform.position) <= _cornerDistance && _currentCorner < _path.corners.Length - 1)
        {
            _currentCorner += 1;
        }
        else if (Vector3.Distance(targetPosition, transform.position) > _cornerDistance)
        {
            Vector3 moveDir = (targetPosition - transform.position).normalized;
            Vector3 moveDirHorizontal = moveDir;
            moveDirHorizontal.y = 0;

            if (moveDirHorizontal != Vector3.zero)
            {
                _services.Model.transform.rotation = Quaternion.RotateTowards(_services.Model.transform.rotation,
                    Quaternion.LookRotation(moveDirHorizontal), Time.deltaTime * _services.GroundedAI.TurnSpeed);

                _services.KinematicPhysics.DesiredVelocity = _services.Model.transform.forward * _services.GroundedAI.Speed;
            }
        }
    }
}
