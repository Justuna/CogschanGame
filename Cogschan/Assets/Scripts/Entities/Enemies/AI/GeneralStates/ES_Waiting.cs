using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ES_Waiting : MonoBehaviour, IEnemyState
{
    [SerializeField] private EntityServiceLocator _services;
    [SerializeField] private float _waitRange;

    public CogschanSimpleEvent OutOfRange;

    public void OnBehave()
    {
        if (_services.LOSChecker.CanSee || Vector3.Distance(transform.position, _services.LOSChecker.LastSeenPosition) > _waitRange)
        {
            OutOfRange?.Invoke();
        }

        Vector3 lookDir = (_services.LOSChecker.LastSeenPosition - transform.position).normalized;
        lookDir.y = 0;
        _services.Model.transform.rotation = Quaternion.Lerp(_services.Model.transform.rotation, Quaternion.LookRotation(lookDir),
                Time.deltaTime * _services.GroundedAI.TurnSpeed);
    }
}
