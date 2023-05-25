using System;
using UnityEngine;

public class AS_Reloading : MonoBehaviour, IActionState
{
    [SerializeField] private PlayerServiceLocator _services;

    public CogschanSimpleEvent ReloadingIntoIdle;
    public CogschanConditionEvent ReloadingIntoLocked;

    public void Behavior()
    {
        if (_services.MovementController.CannotAct)
        {
            OnLock(() => !_services.MovementController.CannotAct);
        }
        else if (true)
        {
            ReloadingIntoIdle.Invoke();
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

    public void OnLock(Func<bool> unlockCondition)
    {
        ReloadingIntoLocked(unlockCondition);
    }
}
