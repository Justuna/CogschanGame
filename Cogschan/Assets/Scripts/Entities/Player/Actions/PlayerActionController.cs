using System;
using TMPro;
using UnityEngine;

/// <summary>
/// Script responsible for translating player input into actions, like firing and shooting.
/// </summary>
public class PlayerActionController : MonoBehaviour
{
    [SerializeField] private EntityServiceLocator _services;
    [SerializeField] private AS_Idle as_Idle;
    [SerializeField] private AS_Locked as_Locked;
    [SerializeField] private AS_Firing as_Firing;
    [SerializeField] private AS_Reloading as_Reloading;
    [SerializeField] private AS_SwitchingWeapons as_SwitchingWeapons;

    private IActionState _currentState;

    /// <summary>
    /// Whether or not Cogschan is currently idle.
    /// </summary>
    public bool IsIdle { get { return _currentState.Equals(as_Idle); } }
    /// <summary>
    /// Whether or not Cogschan is currently locked out of performing actions.
    /// </summary>
    public bool IsLocked { get { return _currentState.Equals(as_Locked); } }
    /// <summary>
    /// Whether or not Cogschan is currently firing a weapon.
    /// </summary>
    public bool IsFiring { get { return _currentState.Equals(as_Firing); } }
    /// <summary>
    /// Whether or not Cogschan is currently reloading her weapon.
    /// </summary>
    public bool IsReloading { get { return _currentState.Equals(as_Reloading); } }
    /// <summary>
    /// Whether or not Cogschan is currently switching weapons.
    /// </summary>
    public bool IsSwitchingWeapons { get { return _currentState.Equals(as_SwitchingWeapons); } }

    private void Start()
    {
        _currentState = as_Idle;

        CogschanInputSingleton.Instance.OnReloadButtonPressed += () => { _currentState.OnReload(); };
        CogschanInputSingleton.Instance.OnSwitchNextWeapon += () => { _currentState.OnNextWeapon(); };
        CogschanInputSingleton.Instance.OnSwitchPrevWeapon += () => { _currentState.OnPrevWeapon(); };
        CogschanInputSingleton.Instance.OnInteractButtonPressed += () => { _currentState.OnInteract(); };

        as_Idle.IdleIntoFiring += IdleIntoFiring;
        as_Idle.IdleIntoReloading += IdleIntoReloading;
        as_Idle.IdleIntoNextWeapon += IdleIntoNextWeapon;
        as_Idle.IdleIntoPrevWeapon += IdleIntoPrevWeapon;
        as_Idle.IdleIntoLocked += XIntoLocked;
        as_Locked.ActionsUnlocked += ActionsUnlocked;
        as_Firing.FiringIntoIdle += FiringIntoIdle;
        as_Firing.FiringIntoLocked += XIntoLocked;
        as_Reloading.ReloadingIntoIdle += ReloadingIntoIdle;
        as_Reloading.ReloadingIntoLocked += XIntoLocked;
        as_SwitchingWeapons.SwitchingIntoIdle += SwitchingIntoIdle;
        as_SwitchingWeapons.SwitchingIntoLocked += XIntoLocked;
    }

    private void Update()
    {
        _currentState.Behavior();
    }

    public void LockActions(Func<bool> unlockCondition)
    {
        _currentState.OnLock(unlockCondition);
    }

    #region Glue Methods

    private void IdleIntoFiring()
    {
        _currentState = as_Firing;
    }

    private void IdleIntoReloading()
    {
        if (_services.WeaponCache.CurrentWeapon.CanReload())
        {
            as_Reloading.Init(_services.WeaponCache.CurrentWeapon.GetReloadTime());
            _currentState = as_Reloading;
        }
    }

    private void IdleIntoNextWeapon()
    {
        as_SwitchingWeapons.Init(true);
        _currentState = as_SwitchingWeapons;
    }

    private void IdleIntoPrevWeapon()
    {
        as_SwitchingWeapons.Init(false);
        _currentState = as_SwitchingWeapons;
    }

    private void FiringIntoIdle()
    {
        _currentState = as_Idle;
    }

    private void ReloadingIntoIdle()
    {
        _currentState = as_Idle;
    }

    private void SwitchingIntoIdle()
    {
        _currentState = as_Idle;
    }

    private void XIntoLocked(Func<bool> unlockCondition)
    {
        as_Locked.Initialize(unlockCondition);
        _currentState = as_Locked;
    }

    private void ActionsUnlocked()
    {
        _currentState = as_Idle;
    }

    #endregion
}
