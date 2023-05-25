using System;
using UnityEngine;

public class AS_Idle : MonoBehaviour, IActionState
{
    [SerializeField] private PlayerServiceLocator _services;

    public CogschanSimpleEvent IdleIntoFiring;
    public CogschanSimpleEvent IdleIntoNextWeapon;
    public CogschanSimpleEvent IdleIntoPrevWeapon;
    public CogschanSimpleEvent IdleIntoReloading;
    public CogschanConditionEvent IdleIntoLocked;

    public void Behavior()
    {
        if (_services.MovementController.CannotAct)
        {
            OnLock(() => !_services.MovementController.CannotAct);
        }
        else if (CogschanInputSingleton.Instance.IsHoldingFire && !_services.MovementController.IsSprinting)
        {
            IdleIntoFiring.Invoke();
        }
    }

    public void OnNextWeapon()
    {
        IdleIntoNextWeapon.Invoke();
    }

    public void OnPrevWeapon()
    {
        IdleIntoPrevWeapon.Invoke();
    }

    public void OnReload()
    {
        IdleIntoReloading.Invoke();
    }

    public void OnLock(Func<bool> unlockCondition)
    {
        IdleIntoLocked.Invoke(unlockCondition);
    }
}
