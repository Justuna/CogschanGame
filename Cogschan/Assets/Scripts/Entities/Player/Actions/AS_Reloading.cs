using System;
using UnityEngine;

public class AS_Reloading : MonoBehaviour, IActionState
{
    [SerializeField] private EntityServiceLocator _services;

    public CogschanSimpleEvent ReloadingIntoIdle;
    public CogschanConditionEvent ReloadingIntoLocked;

    private float _timer;

    public void Init(float timer)
    {
        _services.Animator.SetTrigger("Test");
        _timer = timer;
    }

    public void Behavior()
    {
        if (_services.MovementController.CannotAct)
        {
            OnLock(() => !_services.MovementController.CannotAct);
        }

        _timer -= Time.deltaTime;
        if (_timer <= 0) 
        {
            IWeapon currentWeapon = _services.WeaponCache.CurrentWeapon;

            if (currentWeapon != null || currentWeapon.SufficientAmmo())
            {
                if (currentWeapon.CanReload())
                {
                    currentWeapon.Reload();
                }

                ReloadingIntoIdle?.Invoke();
            }
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
        ReloadingIntoLocked(unlockCondition);
    }
}
