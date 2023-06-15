using System;
using TMPro;
using UnityEngine;

public class ES_Alerted : MonoBehaviour, IEnemyState
{
    [SerializeField] private EntityServiceLocator _services;
    [SerializeField] private float _alertTime;

    public CogschanSimpleEvent LOSBroken;
    public CogschanSimpleEvent ReadyToChase;

    private float _alertTimer = 0;

    public void Behavior()
    {
        if (!_services.LOSChecker.CanSee)
        {
            LOSBroken?.Invoke();
            return;
        }

        Vector3 lookDir = (_services.LOSChecker.LastSeenPosition - transform.position).normalized;
        lookDir.y = 0;
        if (_services.GroundedAI != null)
            _services.Model.transform.rotation = Quaternion.Lerp(_services.Model.transform.rotation, Quaternion.LookRotation(lookDir),
                Time.deltaTime * _services.GroundedAI.TurnSpeed);
        else
            _services.Model.transform.rotation = Quaternion.LookRotation(lookDir);

        if (_alertTimer > 0)
        {
            _alertTimer -= Time.deltaTime;
        }
        else
        {
            ReadyToChase?.Invoke();
        }
    }

    public void ResetTimer()
    {
        _alertTimer = _alertTime;
    }
}
