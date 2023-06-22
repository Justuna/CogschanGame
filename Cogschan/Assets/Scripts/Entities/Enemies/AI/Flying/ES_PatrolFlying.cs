using UnityEngine;

public class ES_PatrolFlying : ES_Patrol
{
    [SerializeField]
    private float _minPatrolHeight;
    [SerializeField]
    private float _maxPatrolHeight;
    [SerializeField]
    private LayerMask _solidMask;

    protected override void SearchPatrolPoint()
    {
        Vector3 point = transform.position;
        for (int i = 0; i < Constants.MAX_ITER; i++)
        {
            float rho = Random.Range(_minPatrolRange, _maxPatrolRange);
            float phi = Random.Range(0, Mathf.PI) % Mathf.PI;
            Vector3 position = new Vector3(rho, phi).CylindricalToCartesian() + transform.position;
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