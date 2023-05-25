using System;
using UnityEngine;

public class AS_Firing : MonoBehaviour, IActionState
{
    [SerializeField] private PlayerServiceLocator _services;

    public CogschanSimpleEvent FiringIntoIdle;
    public CogschanConditionEvent FiringIntoLocked;

    public void Behavior()
    {
        if (_services.MovementController.CannotAct)
        {
            OnLock(() => !_services.MovementController.CannotAct);
        }
        else if (!CogschanInputSingleton.Instance.IsHoldingFire)
        {
            FiringIntoIdle.Invoke();
        }
        
        _services.WeaponCache.CurrentWeapon.Use();
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

    public void OnLock(Func<bool> unlockCondition)
    {
        FiringIntoLocked.Invoke(unlockCondition);
    }
}
