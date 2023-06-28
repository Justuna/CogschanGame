using System.Collections.Generic;
using System.IO;
using System.Linq;
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
    [SerializeField] private float _turnAngleDegrees = 15;
    [SerializeField] private float _turnCorrectionLength = .25f;

    public CogschanSimpleEvent LostPlayer;
    public CogschanSimpleEvent RangedAttack;
    public CogschanSimpleEvent MeleeAttack;

    private NavMeshPath _navMeshPath;
    private List<Vector3> _path;
    private int _currentCorner;
    private float _rangedAttackTimer;
    private float _recalculateTimer;

    private void Start()
    {
        _rangedAttackTimer = UnityEngine.Random.Range(_minTimeUntilRangedAttack, _maxTimeUntilRangedAttack);
        _navMeshPath = new NavMeshPath();
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
            NavMesh.CalculatePath(hit.position, hit2.position, NavMesh.AllAreas, _navMeshPath);
            SmoothPath();

            _recalculateTimer = _recalculateTime;
            _currentCorner = 0;
        }

        Vector3 targetPosition = _path[_currentCorner];

        if (Vector3.Distance(targetPosition, transform.position) <= _cornerDistance && _currentCorner < _path.Count - 1)
        {
            _currentCorner += 1;
        }
        else if (Vector3.Distance(targetPosition, transform.position) > _cornerDistance)
        {
            Vector3 moveDir = (targetPosition - transform.position).normalized;
            Vector3 moveDirHorizontal = moveDir;
            moveDirHorizontal.y = 0;

            _services.KinematicPhysics.DesiredVelocity = moveDir * _services.GroundedAI.Speed;
            if (moveDirHorizontal != Vector3.zero)
                _services.Model.transform.rotation = Quaternion.Lerp(_services.Model.transform.rotation, 
                    Quaternion.LookRotation(moveDirHorizontal), Time.deltaTime * _services.GroundedAI.TurnSpeed);
        }
    }

    // Store a smoothed out path in the _path variable.
    private void SmoothPath()
    {
        _path = _navMeshPath.corners.ToList();
        //_path.Insert(0, transform.position);
        print(_path.Count);
        for (int i = 1, j = 0; i < _path.Count - 1 && j < Constants.MAX_ITER; i++, j++)
        {
            if (j == Constants.MAX_ITER - 1)
                Debug.LogWarning($"Loop reached max iteration count. Point {i+1}/{_path.Count}.");

            // Get the directions of the two line segments this point is a part of.
            Vector3 dir1 = (_path[i] - _path[i - 1]).normalized;
            Vector3 dir2 = (_path[i + 1] - _path[i]).normalized;

            // If the angle is sufficiently small, skip the rest of this loop.
            float angle = Vector3.Angle(dir1, dir2);
            if (angle <= _turnAngleDegrees)
                continue;

            // Make a new line segment with the average direction of the above line segments.
            Vector3 newDir = (dir1 + dir2).normalized;
            Vector3 newPoint1 = _path[i] - newDir * _turnCorrectionLength;
            Vector3 newPoint2 = _path[i] + newDir * _turnCorrectionLength;

            // Map the new points to the navmesh and modify the list.
            NavMesh.SamplePosition(newPoint1, out NavMeshHit hit, float.PositiveInfinity, NavMesh.AllAreas);
            newPoint1 = hit.position;
            NavMesh.SamplePosition(newPoint2, out hit, float.PositiveInfinity, NavMesh.AllAreas);
            newPoint2 = hit.position;
            _path[i] = newPoint1;
            _path.Insert(i + 1, newPoint2);

            // We need to check this point again, so decrement i.
            i--;
        }
    }
}
