using UnityEngine;

public class ES_ChaseFlying : MonoBehaviour, IEnemyState
{
    [SerializeField]
    private EntityServiceLocator _services;
    [SerializeField] private float _minTimeUntilRangedAttack;
    [SerializeField] private float _maxTimeUntilRangedAttack;
    [SerializeField]
    private float _attackRange;

    private float _rangedAttackTimer;

    public CogschanSimpleEvent LostPlayer;
    public CogschanSimpleEvent InAttackRange;

    public void Init()
    {
        _rangedAttackTimer = Random.Range(_minTimeUntilRangedAttack, _maxTimeUntilRangedAttack);
    }

    public void Behavior()
    {
        _rangedAttackTimer -= Time.deltaTime;

        if (_services.LOSChecker.CanSee)
        {
            Vector3 displacement = _services.LOSChecker.LastSeenPosition - transform.position;
            if (displacement.magnitude > _attackRange)
            {
                _services.KinematicPhysics.DesiredVelocity =
                    displacement.normalized * _services.FlyingAI.Speed;
                _services.Model.transform.rotation = Quaternion.Lerp(_services.Model.transform.rotation, Quaternion.LookRotation(displacement),
                Time.deltaTime * _services.FlyingAI.TurnSpeed);
            }
            else if (_rangedAttackTimer <= 0 && _services.LOSChecker.CanSee)
            {
                _rangedAttackTimer = 0;
                InAttackRange?.Invoke();
            }
        }
        else
            LostPlayer();
    }
}