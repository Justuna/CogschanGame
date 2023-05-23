using System;
using UnityEngine;

public class AS_Firing : MonoBehaviour, IActionState
{
    [SerializeField] private PlayerMovementController _movementController;

    public CogschanSimpleEvent FiringIntoIdle;
    public CogschanConditionEvent FiringIntoLocked;

    public void Behavior()
    {
        if (_movementController.CannotAct)
        {
            OnLock(() => !_movementController.CannotAct);
        }
        else if (!CogschanInputSingleton.Instance.IsHoldingFire)
        {
            FiringIntoIdle.Invoke();
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
        FiringIntoLocked.Invoke(unlockCondition);
    }
}
