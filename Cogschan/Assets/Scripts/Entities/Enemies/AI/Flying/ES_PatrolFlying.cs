using UnityEngine;

public class ES_PatrolFlying : ES_Patrol
{
    [SerializeField]
    [Tooltip("The minimum height of the patrol point.")]
    private float _minPatrolHeight;
    [SerializeField]
    [Tooltip("The maximum height of the patrol point.")]
    private float _maxPatrolHeight;
    [SerializeField]
    [Tooltip("The layers that the enemy can't travel through.")]
    private LayerMask _solidMask;

    protected override void SearchPatrolPoint()
    {
        Vector3 point = transform.position;
        for (int i = 0; i < Constants.MAX_ITER; i++)
        {
            Vector3 position = ContinuousDistributions.GetRandomPointInAnnulus(_minPatrolRange, _maxPatrolRange, transform.position);
            position.y = GroundFinder.HeightOfGround(point) + Random.Range(_minPatrolHeight, _maxPatrolHeight);

            Vector3 dir = point - transform.position;
            Ray ray = new(transform.position, dir);
            if (GroundFinder.IsOverGround(point) && !Physics.Raycast(ray, dir.magnitude, _solidMask))
            {
                point = position;
                break;
            }
        }
        _patrolPoint =  point;
        _hasSetPatrolPoint = true;
        _boredTimer = _timeUntilBored;
    }

    protected override void MoveToPatrolPoint()
    {
        Vector3 moveDir = (_patrolPoint - transform.position).normalized;
        _services.KinematicPhysics.DesiredVelocity = (_patrolPoint - transform.position).normalized * _services.FlyingAI.Speed;
        _services.Model.transform.rotation = Quaternion.Lerp(_services.Model.transform.rotation, Quaternion.LookRotation(moveDir),
                Time.deltaTime * _services.FlyingAI.TurnSpeed);
    }
}