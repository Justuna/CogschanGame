using UnityEngine;

public class ES_JanglingWatching : MonoBehaviour, IJanglingState
{
    [SerializeField] private EntityServiceLocator _services;
    [SerializeField] private float _turnSpeed;

    public CogschanSimpleEvent WatchingToRunning;
    public CogschanSimpleEvent WatchingToIdle;
    public CogschanSimpleEvent WatchingToStunned;

    public void Behavior()
    {
        if (_services.LOSChecker.CanSee)
        {
            Vector3 dir = _services.LOSChecker.LastSeenPosition - _services.Jangling.position;

            _services.Model.transform.rotation = Quaternion.Lerp(_services.Model.transform.rotation, Quaternion.LookRotation(dir), _turnSpeed * Time.deltaTime);

            float dist = Vector3.Distance(_services.LOSChecker.LastSeenPosition, _services.Jangling.position);
            if (dist <= _services.JanglingAI.RunDistance)
            {
                WatchingToRunning?.Invoke();
            }
        }
        else
        {
            WatchingToIdle?.Invoke();
        }
    }

    public void OnStun()
    {
        WatchingToStunned?.Invoke();
    }

    public void OnDamaged()
    {
        WatchingToRunning?.Invoke();
    }
}
