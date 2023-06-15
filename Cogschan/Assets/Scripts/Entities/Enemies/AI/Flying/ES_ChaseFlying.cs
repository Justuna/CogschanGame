using UnityEngine;

public class ES_ChaseFlying : MonoBehaviour, IEnemyState
{
    [SerializeField]
    private EntityServiceLocator _services;
    [SerializeField]
    private float _attackRange;

    public CogschanSimpleEvent LostPlayer;
    public CogschanSimpleEvent InAttackRange;


    public void Behavior()
    {
        if (_services.LOSChecker.CanSee)
        {
            Vector3 displacement = _services.LOSChecker.LastSeenPosition - transform.position;
            if (displacement.magnitude > _attackRange)
            {
                _services.KinematicPhysics.DesiredVelocity =
                    displacement.normalized * _services.FlyingAI.Speed;
                _services.Model.transform.rotation = Quaternion.LookRotation(displacement);
            }
            else
                InAttackRange();
        }
        else
            LostPlayer();
    }
}