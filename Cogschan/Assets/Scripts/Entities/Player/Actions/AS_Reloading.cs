using System;
using UnityEngine;

public class AS_Reloading : MonoBehaviour, IActionState
{
    [SerializeField] private EntityServiceLocator _services;

    public CogschanSimpleEvent ReloadingIntoIdle;
    public CogschanConditionEvent ReloadingIntoLocked;

    public void Behavior()
    {
        if (_services.MovementController.CannotAct)
        {
            OnLock(() => !_services.MovementController.CannotAct);
        }
        else if (CurrentWeaponSufficientAmmo())
        {
            ReloadingIntoIdle?.Invoke();
        }
    }

    private bool CurrentWeaponSufficientAmmo()
    {
        IWeapon currentWeapon = _services.WeaponCache.CurrentWeapon;
        return currentWeapon == null || currentWeapon.SufficientAmmo();
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
        if (_services.MovementController.CannotAct)
            return;

        IWeapon currentWeapon = _services.WeaponCache.CurrentWeapon;
        if (currentWeapon != null && currentWeapon.CanReload())
        {
            currentWeapon.Reload();
        }
    }

    public void OnInteract()
    {
        // Do nothing
    }

    public void OnLock(Func<bool> unlockCondition)
    {
        ReloadingIntoLocked(unlockCondition);
    }
}
