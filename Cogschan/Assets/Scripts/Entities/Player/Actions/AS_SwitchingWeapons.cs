using System;
using UnityEngine;

public class AS_SwitchingWeapons : MonoBehaviour, IActionState
{
    [SerializeField] private PlayerServiceLocator _services;

    public CogschanSimpleEvent SwitchingIntoIdle;
    public CogschanConditionEvent SwitchingIntoLocked;

    public void Behavior()
    {
        if (_services.MovementController.CannotAct)
        {
            OnLock(() => !_services.MovementController.CannotAct);
        }
        else if (true)
        {
            SwitchingIntoIdle.Invoke();
        }
    }

    public void OnNextWeapon()
    {
        
    }

    public void OnPrevWeapon()
    {
        
    }

    public void OnReload()
    {
        // Do nothing
    }

    public void OnLock(Func<bool> unlockCondition)
    {
        SwitchingIntoLocked(unlockCondition);
    }
}