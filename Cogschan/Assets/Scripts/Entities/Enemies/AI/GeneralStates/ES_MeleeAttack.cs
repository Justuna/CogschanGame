using UnityEngine;

public class ES_MeleeAttack : MonoBehaviour, IEnemyState
{
    [SerializeField]
    [Tooltip("The attack rate of the enemy.")]
    private float _attackRate;
    [SerializeField]
    [Tooltip("The amount of time the attack hitbox lasts.")]
    private float _attackActiveTime;
    [SerializeField]
    [Tooltip("The maximum distance of the melee attack")]
    private float _distance;
    [SerializeField]
    [Tooltip("The service locator for the enemy.")]
    private EntityServiceLocator _services;

    public CogschanSimpleEvent OutOfRange;
    private bool _active = false;
    private float _timer = float.PositiveInfinity;

    public void OnBehave()
    {
        Vector3 playerDir = _services.LOSChecker.LastSeenPosition - transform.position;
        if (!_services.LOSChecker.CanSee || playerDir.magnitude > _distance)
        {
            _timer = float.PositiveInfinity;
            _active = false;
            OutOfRange?.Invoke();
            return;
        }
        Vector3 playerDirHorizontal = playerDir;
        playerDir.y = 0;
        if (playerDirHorizontal != Vector3.zero)
            _services.Model.transform.rotation = Quaternion.RotateTowards(_services.Model.transform.rotation,
                Quaternion.LookRotation(playerDirHorizontal), Time.deltaTime * _services.GroundedAI.TurnSpeed);
        if (_timer > _attackRate)
        {
            _timer = 0;
            _active = true;
            _services.GroundedAI.BeginMeleeAttack();
            
        }
        if (_timer > _attackActiveTime && _active)
        {
            _active = false;
            _services.GroundedAI.EndMeleeAttack();
        }
        _timer += Time.deltaTime;
    }
}
