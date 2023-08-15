using System;
using UnityEngine;

public class AS_Idle : MonoBehaviour, IActionState, IMachineStateBehave
{
    [SerializeField] private EntityServiceLocator _services;

    public CogschanSimpleEvent IdleIntoFiring;
    public CogschanSimpleEvent IdleIntoNextWeapon;
    public CogschanSimpleEvent IdleIntoPrevWeapon;
    public CogschanSimpleEvent IdleIntoReloading;
    public CogschanConditionEvent IdleIntoLocked;

    public void OnBehave()
    {
        if (_services.MovementController.CannotAct)
        {
            OnLock(() => !_services.MovementController.CannotAct);
        }
        else if (CogschanInputSingleton.Instance.IsHoldingFire && !_services.MovementController.IsSprinting)
        {
            IdleIntoFiring?.Invoke();
        }

        var targetPos = _services.CameraController.TargetPosition ?? transform.forward;
        _services.WeaponCache.CurrentWeapon.GetGameObject().transform.LookAt(targetPos);
    }

    public void OnNextWeapon()
    {
        IdleIntoNextWeapon?.Invoke();
    }

    public void OnPrevWeapon()
    {
        IdleIntoPrevWeapon?.Invoke();
    }

    public void OnReload()
    {
        IdleIntoReloading?.Invoke();
    }

    public void OnInteract()
    {
        Interactable optIn = _services.InteractionChecker.OptIn;
        if (optIn != null) optIn.Interact(_services);
    }

    public void OnLock(Func<bool> unlockCondition)
    {
        IdleIntoLocked.Invoke(unlockCondition);
    }
}
