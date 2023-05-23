using System;
using UnityEngine;

public class AS_SwitchingWeapons : MonoBehaviour, IActionState
{
    [SerializeField] private PlayerMovementController _movementController;

    public CogschanSimpleEvent SwitchingIntoIdle;
    public CogschanConditionEvent SwitchingIntoLocked;

    public void Behavior()
    {
        if (_movementController.CannotAct)
        {
            OnLock(() => !_movementController.CannotAct);
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