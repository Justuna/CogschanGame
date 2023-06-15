using UnityEngine;

public class ES_PatrolFlying : ES_Patrol
{
    [SerializeField]
    private LayerMask _solidMask;

    protected override void SearchPatrolPoint()
    {
        bool pointIsValid = false;
        Vector3 point = new();
        while (!pointIsValid)
        {
            float r = Random.Range(_minPatrolRange, _maxPatrolRange);
            float theta = Random.Range(0, Mathf.PI) % Mathf.PI;
            float phi = Random.Range(0, 2 * Mathf.PI) % (2 * Mathf.PI);
            point = new Vector3(r, theta, phi).SphericalToCartesian();

            Vector3 dir = point - transform.position;
            Ray ray = new(transform.position, dir);
            pointIsValid = !Physics.Raycast(ray, dir.magnitude, _solidMask);
        }
        _patrolPoint =  point;
        _hasSetPatrolPoint = true;
        _boredTimer = _timeUntilBored;

    }

    protected override void MoveToPatrolPoint()
    {
        Vector3 moveDir = (_patrolPoint - transform.position).normalized;
        _services.KinematicPhysics.DesiredVelocity = (_patrolPoint - transform.position).normalized * _services.FlyingAI.Speed;
        _services.Model.transform.rotation = Quaternion.LookRotation(moveDir); // TODO: Make this non-instant.
    }
}