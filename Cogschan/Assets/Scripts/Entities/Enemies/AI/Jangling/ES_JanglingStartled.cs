using System;
using UnityEngine;

public class ES_JanglingStartled : MonoBehaviour, IJanglingState
{
    [SerializeField] private EntityServiceLocator _services;
    [SerializeField] private float _startleTime;
    [SerializeField] private float _turnSpeed;

    public CogschanSimpleEvent StartledToRunning;
    public CogschanSimpleEvent StartledToStunned;

    private float _timer;

    public void Init()
    {
        _timer = _startleTime;
    }

    public void Behavior()
    {
        Vector3 dir = _services.LOSChecker.LastSeenPosition - _services.Jangling.position;

        _services.Model.transform.rotation = Quaternion.Lerp(_services.Model.transform.rotation, Quaternion.LookRotation(dir), _turnSpeed * Time.deltaTime);

        _timer -= Time.deltaTime;
        if (_timer <= 0 ) 
        {
            StartledToRunning?.Invoke();
        }
    }

    public void OnStun()
    {
        StartledToStunned?.Invoke();
    }

    public void OnDamaged()
    {
        // Do nothing
    }
}
