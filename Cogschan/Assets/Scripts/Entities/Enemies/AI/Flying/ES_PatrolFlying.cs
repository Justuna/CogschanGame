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
        bool pointIsValid = false;
        Vector3 point = new();
        while (!pointIsValid)
        {
            float rho = Random.Range(_minPatrolRange, _maxPatrolRange);
            float phi = Random.Range(0, Mathf.PI) % Mathf.PI;
            float z = Random.Range(_minPatrolHeight, _maxPatrolHeight) + GetGroundHeight();
            print(GetGroundHeight());
            point = new Vector3(rho, phi, z).CylindricalToCartesian();

            Vector3 dir = point - transform.position;
            Ray ray = new(transform.position, dir);
            pointIsValid = !Physics.Raycast(ray, dir.magnitude, _solidMask);
        }
        _patrolPoint =  point;
        print(point);
        _hasSetPatrolPoint = true;
        _boredTimer = _timeUntilBored;

    }

    protected override void MoveToPatrolPoint()
    {
        Vector3 moveDir = (_patrolPoint - transform.position).normalized;
        _services.KinematicPhysics.DesiredVelocity = (_patrolPoint - transform.position).normalized * _services.FlyingAI.Speed;
        _services.Model.transform.rotation = Quaternion.LookRotation(moveDir); // TODO: Make this non-instant.
    }

    private float GetGroundHeight()
    {
        Ray ray = new(transform.position, Vector3.down);
        Physics.Raycast(ray, out RaycastHit hit, float.PositiveInfinity, _solidMask);
        print(hit.collider.gameObject);
        return transform.position.y - hit.distance;
    }
}