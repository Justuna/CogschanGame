using System;
using UnityEngine;

public class AS_Idle : MonoBehaviour, IActionState, IMachineStateBehave, IMachineStateLateBehave
{
    [SerializeField] private EntityServiceLocator _services;
    [SerializeField] private float _weaponTurnSpeed = 50;

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
        else if (CogschanInputSingleton.Instance.IsHoldingFire)
        {
            IdleIntoFiring?.Invoke();
        }
    }

    public void OnLateBehave()
    {
        /*var targetPos = _services.CameraController.TargetPosition ?? transform.forward;

        var weaponTransform = _services.WeaponCache.CurrentWeapon.GetGameObject().transform;
        weaponTransform.rotation = Quaternion.Lerp(weaponTransform.rotation, Quaternion.LookRotation(targetPos - weaponTransform.position), _weaponTurnSpeed * Time.deltaTime);*/
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
