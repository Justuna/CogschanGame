using System;
using UnityEngine;

public class AS_Idle : MonoBehaviour, IActionState
{
    [SerializeField] private PlayerMovementController _movementController;

    public CogschanSimpleEvent IdleIntoFiring;
    public CogschanSimpleEvent IdleIntoNextWeapon;
    public CogschanSimpleEvent IdleIntoPrevWeapon;
    public CogschanSimpleEvent IdleIntoReloading;
    public CogschanConditionEvent IdleIntoLocked;

    public void Behavior()
    {
        if (_movementController.CannotAct)
        {
            OnLock(() => !_movementController.CannotAct);
        }
        else if (CogschanInputSingleton.Instance.IsHoldingFire && !_movementController.IsSprinting)
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
