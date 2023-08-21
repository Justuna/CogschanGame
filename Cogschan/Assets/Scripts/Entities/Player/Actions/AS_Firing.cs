using System;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class AS_Firing : MonoBehaviour, IActionState, IMachineStateBehave
{
    public Action Used;

    [SerializeField] private EntityServiceLocator _services;

    public CogschanSimpleEvent FiringIntoIdle;
    public CogschanSimpleEvent FiringIntoReload;
    public CogschanConditionEvent FiringIntoLocked;

    public void OnBehave()
    {
        IWeapon weapon = _services.WeaponCache.CurrentWeapon;

        Used?.Invoke();
        weapon.Use();

        if (_services.MovementController.CannotAct)
        {
            OnLock(() => !_services.MovementController.CannotAct);
        }
        else if (!weapon.InUse() && !CogschanInputSingleton.Instance.IsHoldingFire)
        {
            FiringIntoIdle?.Invoke();
        }
        else if (!weapon.InUse() && weapon.GetLoadedAmmoCount().HasValue && weapon.GetLoadedAmmoCount().Value == 0)
        {
            FiringIntoReload?.Invoke();
        }
    }

    public void OnNextWeapon()
    {
        // Do nothing
    }

    public void OnPrevWeapon()
    {
        // Do nothing
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
        FiringIntoLocked.Invoke(unlockCondition);
    }
}
