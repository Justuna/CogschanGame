using System;
using System.Collections.Generic;
using UnityEngine;

public class AS_Locked : MonoBehaviour, IActionState
{
    private List<Func<bool>> _unlockConditions = new List<Func<bool>>();

    public CogschanSimpleEvent ActionsUnlocked;

    public void Initialize(Func<bool> unlockCondition)
    {
        _unlockConditions.Add(unlockCondition);
    }

    public void Behavior()
    {
        _unlockConditions.RemoveAll(condition => condition());
        if (_unlockConditions.Count == 0 )
        {
            ActionsUnlocked?.Invoke();
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
        Initialize(unlockCondition);
    }
}
