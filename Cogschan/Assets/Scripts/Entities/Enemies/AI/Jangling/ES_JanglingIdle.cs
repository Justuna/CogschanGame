using System.Collections;
using System.Collections.Generic;
using System.Net.NetworkInformation;
using Unity.VisualScripting;
using UnityEngine;

public class ES_JanglingIdle : MonoBehaviour, IJanglingState
{
    [SerializeField] private EntityServiceLocator _services;

    public CogschanSimpleEvent IdleToWatching;
    public CogschanSimpleEvent IdleToStartled;
    public CogschanSimpleEvent IdleToStunned;

    public void Behavior()
    {
        if (_services.LOSChecker.CanSee)
        {
            float dist = Vector3.Distance(_services.LOSChecker.LastSeenPosition, _services.Jangling.position);
            if (dist <= _services.JanglingAI.RunDistance)
            {
                IdleToStartled?.Invoke();
            }
            else
            {
                IdleToWatching?.Invoke();
            }
        }
    }

    public void OnStun()
    {
        IdleToStunned?.Invoke();
    }

    public void OnDamaged()
    {
        IdleToStartled?.Invoke();
    }
}
