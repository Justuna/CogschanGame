using System;
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
    public bool IsIdle { get { return CurrentState.Equals(as_Idle); } }
    /// <summary>
    /// Whether or not Cogschan is currently locked out of performing actions.
    /// </summary>
    public bool IsLocked { get { return CurrentState.Equals(as_Locked); } }
    /// <summary>
    /// Whether or not Cogschan is currently firing a weapon.
    /// </summary>
    public bool IsFiring { get { return CurrentState.Equals(as_Firing); } }
    /// <summary>
    /// Whether or not Cogschan is currently reloading her weapon.
    /// </summary>
    public bool IsReloading { get { return CurrentState.Equals(as_Reloading); } }
    /// <summary>
    /// Whether or not Cogschan is currently switching weapons.
    /// </summary>
    public bool IsSwitchingWeapons { get { return CurrentState.Equals(as_SwitchingWeapons); } }

    public IActionState CurrentState
    {
        get => _currentState;
        set
        {
            if (value != _currentState)
            {
                if (_currentState is IMachineStateExit exitableState)
                    exitableState.OnExit();
                _currentState = value;
                if (_currentState is IMachineStateEnter enterableState)
                    enterableState.OnEnter();
            }
        }
    }

    private void Start()
    {
        CurrentState = as_Idle;

        CogschanInputSingleton.Instance.OnReloadButtonPressed += OnReloadPressed;
        CogschanInputSingleton.Instance.OnSwitchNextWeapon += OnSwitchNextWeapon;
        CogschanInputSingleton.Instance.OnSwitchPrevWeapon += OnSwitchPrevWeapon;
        CogschanInputSingleton.Instance.OnInteractButtonPressed += OnInteractPressed;

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
        if (CurrentState is IMachineStateBehave behaveState)
            behaveState.OnBehave();
    }

    private void LateUpdate()
    {
        if (CurrentState is IMachineStateLateBehave lateBehaveState)
            lateBehaveState.OnLateBehave();
    }

    private void FixedUpdate()
    {
        if (CurrentState is IMachineStateFixedBehave fixedBehaveState)
            fixedBehaveState.OnFixedBehave();
    }

    private void OnDestroy()
    {
        if (CogschanInputSingleton.Instance != null)
        {
            CogschanInputSingleton.Instance.OnReloadButtonPressed -= OnReloadPressed;
            CogschanInputSingleton.Instance.OnSwitchNextWeapon -= OnSwitchNextWeapon;
            CogschanInputSingleton.Instance.OnSwitchPrevWeapon -= OnSwitchPrevWeapon;
            CogschanInputSingleton.Instance.OnInteractButtonPressed -= OnInteractPressed;
        }
    }

    private void OnReloadPressed()
    {
        if (gameObject.activeSelf) CurrentState.OnReload();
    }

    private void OnSwitchNextWeapon()
    {
        if (gameObject.activeSelf) CurrentState.OnNextWeapon();
    }

    private void OnSwitchPrevWeapon()
    {
        if (gameObject.activeSelf) CurrentState.OnPrevWeapon();
    }

    private void OnInteractPressed()
    {
        if (gameObject.activeSelf) CurrentState.OnInteract();
    }

    public void LockActions(Func<bool> unlockCondition)
    {
        CurrentState.OnLock(unlockCondition);
    }

    #region Glue Methods

    private void IdleIntoFiring()
    {
        CurrentState = as_Firing;
    }

    private void IdleIntoReloading()
    {
        if (_services.WeaponCache.CurrentWeapon.CanReload())
        {
            as_Reloading.Init(_services.WeaponCache.CurrentWeapon.GetReloadTime());
            CurrentState = as_Reloading;
        }
    }

    private void IdleIntoNextWeapon()
    {
        as_SwitchingWeapons.Init(true);
        CurrentState = as_SwitchingWeapons;
    }

    private void IdleIntoPrevWeapon()
    {
        as_SwitchingWeapons.Init(false);
        CurrentState = as_SwitchingWeapons;
    }

    private void FiringIntoIdle()
    {
        CurrentState = as_Idle;
    }

    private void ReloadingIntoIdle()
    {
        CurrentState = as_Idle;
    }

    private void SwitchingIntoIdle()
    {
        CurrentState = as_Idle;
    }

    private void XIntoLocked(Func<bool> unlockCondition)
    {
        as_Locked.Initialize(unlockCondition);
        CurrentState = as_Locked;
    }

    private void ActionsUnlocked()
    {
        CurrentState = as_Idle;
    }

    #endregion
}
