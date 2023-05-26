using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LOSCalculator : MonoBehaviour
{
    [SerializeField] private Transform _playerTarget;
    [SerializeField] private Transform _eyes;
    [SerializeField] private LayerMask _solidMask;
    [SerializeField] private LayerMask _playerMask;
    [SerializeField] private float _sightRange;
    [SerializeField] private float _interruptThreshold;

    public bool CanSee { get; private set; }
    public Vector3 LastSeenPosition { get; private set; }

    protected float _interruptClock;

    void Update()
    {
        Vector3 dir = _playerTarget.position - _eyes.position;
        Ray ray = new Ray(_eyes.position, dir);
        bool LOS = Physics.CheckSphere(_eyes.position, _sightRange, _playerMask) && !Physics.Raycast(ray, dir.magnitude, _solidMask);

        if (LOS)
        {
            _interruptClock = _interruptThreshold;
            LastSeenPosition = _playerTarget.position;
            CanSee = true;
        }
        else
        {
            if (_interruptClock > 0)
            {
                _interruptClock -= Time.deltaTime;
                LastSeenPosition = _playerTarget.position;
                CanSee = true;
            }
            else CanSee = false;
        }
    }
}
