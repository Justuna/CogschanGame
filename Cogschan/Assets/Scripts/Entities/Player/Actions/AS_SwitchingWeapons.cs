using System;
using UnityEngine;

public class AS_SwitchingWeapons : MonoBehaviour, IActionState
{
    [SerializeField] private EntityServiceLocator _services;
    [SerializeField] private float _switchTime;

    public CogschanSimpleEvent SwitchingIntoIdle;
    public CogschanConditionEvent SwitchingIntoLocked;

    // TO-DO: Replace timer with actual animation-driven logic
    private float _switchTimer;

    public void Init(bool nextWeapon)
    {
        _switchTimer = _switchTime;
        if (nextWeapon) _services.WeaponCache.NextWeapon();
        else _services.WeaponCache.PrevWeapon();
    }

    public void Behavior()
    {
        if (_services.MovementController.CannotAct)
        {
            OnLock(() => !_services.MovementController.CannotAct);
        }
        else if (_switchTimer <= 0)
        {
            SwitchingIntoIdle?.Invoke();
        }
        else
        {
            _switchTimer -= Time.deltaTime;
        }
    }

    // Short-circuit the controller by just reinitializing here
    public void OnNextWeapon()
    {
        Init(true);
    }

    public void OnPrevWeapon()
    {
        Init(false);
    }

    public void OnReload()
    {
        // Do nothing
    }

    public void OnInteract()
    {
        // Do nothing
    }

    public void OnLock(Func<bool> unlockCondition)
    {
        SwitchingIntoLocked(unlockCondition);
    }
}