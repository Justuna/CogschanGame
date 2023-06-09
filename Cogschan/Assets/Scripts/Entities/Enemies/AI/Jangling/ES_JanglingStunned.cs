using UnityEngine;

public class ES_JanglingStunned : MonoBehaviour, IJanglingState
{
    [SerializeField] private EntityServiceLocator _services;
    [SerializeField] private float _stunTime;

    public CogschanSimpleEvent StunEnded;

    private float _stunTimer;

    public void Init()
    {
        _stunTimer = _stunTime;
    }

    public void Behavior()
    {
        _stunTimer -= Time.deltaTime;
        if (_stunTimer <= 0 )
        {
            StunEnded?.Invoke();
        }
    }

    public void OnStun()
    {
        // Do nothing
        // Should not even be triggerable
    }

    public void OnDamaged()
    {
        // Do nothing
        // Should not even be triggerable
    }
}
