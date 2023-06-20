using System;
using System.IO;
using UnityEngine;
using UnityEngine.AI;

public class ES_PatrolGround : ES_Patrol
{
    [SerializeField] private float _cornerDistance;
    
    private NavMeshPath _path;
    private int _currentCorner;

    private void Start()
    {
        _path = new NavMeshPath();
    }

    public override void Init()
    {
        base.Init();
        _currentCorner = 0;
    }

    protected override void SearchPatrolPoint()
    {
        NavMeshHit hit;

        // Pick a random point on the navmesh by first picking a direction
        // and then picking a distance to travel in that direction
        // and then finally sampling the navmesh where the direction + distance leads
        Vector3 randomDir = Quaternion.Euler(0, UnityEngine.Random.Range(0, 360), 0f) * Vector3.forward;
        float randomDist = UnityEngine.Random.Range(_minPatrolRange, _maxPatrolRange);
        Vector3 randomPos = transform.position + randomDir * randomDist;

        NavMesh.SamplePosition(randomPos, out hit, Mathf.Infinity, NavMesh.AllAreas);

        // If the path to that point is a complete one and not right next to the enemy, pick it
        if (NavMesh.CalculatePath(transform.position, hit.position, NavMesh.AllAreas, _path) && 
            _path.status == NavMeshPathStatus.PathComplete && Vector3.Distance(hit.position, _patrolPoint) > 0.5f)
        {
            _patrolPoint = hit.position;
            _hasSetPatrolPoint = true;
            _boredTimer = _timeUntilBored;
        }
    }

    protected override void MoveToPatrolPoint()
    {
        // Get the next point in the path
        Vector3 targetPosition = _path.corners[_currentCorner];

        // If you get close enough to the next point and there are more points left, pick a new next point
        if (Vector3.Distance(targetPosition, transform.position) <= _cornerDistance && _currentCorner < _path.corners.Length - 1)
        {
            _currentCorner += 1;
        }
        // Otherwise, travel toward the next point
        else if (Vector3.Distance(targetPosition, transform.position) > _cornerDistance)
        {
            Vector3 moveDir = (targetPosition - transform.position).normalized;
            Vector3 moveDirHorizontal = moveDir;
            moveDirHorizontal.y = 0;

            _services.KinematicPhysics.DesiredVelocity = moveDir * _services.GroundedAI.Speed;
            _services.Model.transform.rotation = Quaternion.Lerp(_services.Model.transform.rotation, Quaternion.LookRotation(moveDirHorizontal), 
                Time.deltaTime * _services.GroundedAI.TurnSpeed);
        }
    }
}