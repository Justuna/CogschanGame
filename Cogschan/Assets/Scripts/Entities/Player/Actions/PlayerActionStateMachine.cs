using System;
using UnityEngine;
using UnityEngine.Animations;

public class PlayerActionStateMachine : StateMachineBehaviour
{
    [SerializeField] private float _tryReloadTime;
    
    private EntityServiceLocator _services;
    private Animator _animator;
    private bool _initialized = false;

    private int _fireLocks = 0;

    private int _reloadLocks = 0;
    private float _tryReloadTimer = 0;

    private int _switchLocks = 0;
    private int _interactLocks = 0;

    public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        base.OnStateUpdate(animator, stateInfo, layerIndex);

        if (_services == null) _services = animator.GetComponentInParent<EntityServiceLocator>();
        if (_animator == null) _animator = animator;
        if (!_initialized && _services != null && CogschanInputSingleton.Instance != null)
        {
            {
                _initialized = true;
                CogschanInputSingleton.Instance.OnReloadButtonPressed += AttemptReload;
                CogschanInputSingleton.Instance.OnSwitchNextWeapon += () => AttemptSwitchWeapon(false);
                CogschanInputSingleton.Instance.OnSwitchPrevWeapon += () => AttemptSwitchWeapon(true);
                CogschanInputSingleton.Instance.OnInteractButtonPressed += AttemptInteract;
            }
        }

        if (_services == null || _animator == null || !_initialized) return;

        IWeapon weapon = _services.WeaponCache.CurrentWeapon;

        animator.SetInteger("WeaponID", (int) weapon.GetAnimationType());
        animator.SetBool("IsFiring", CogschanInputSingleton.Instance.IsHoldingFire && _fireLocks == 0 && weapon.SufficientAmmo());

        if (CogschanInputSingleton.Instance.IsHoldingFire && weapon.CanReload() && !weapon.SufficientAmmo() && !animator.GetBool("WeaponReloading")
            && !animator.GetBool("IsSprinting") && _reloadLocks == 0)
        {
            _tryReloadTimer -= Time.deltaTime;
            if (_tryReloadTimer <= 0)
            {
                _tryReloadTimer = _tryReloadTime;
                AttemptReload();
            }
        }
        else
        {
            _tryReloadTimer = _tryReloadTime;
        }
    }

    public void AddFireLock()
    {
        _fireLocks++;
    }

    public void RemoveFireLock()
    {
        if ( _fireLocks > 0 ) _fireLocks--;
    }

    public void AddReloadLock()
    {
        _reloadLocks++;
    }

    public void RemoveReloadLock()
    {
        if (_reloadLocks > 0) _reloadLocks--;
    }

    private void AttemptReload()
    {
        if (_reloadLocks > 0 || !_services.WeaponCache.CurrentWeapon.CanReload()) return;

        _animator.SetTrigger("Reload");
    }

    public void Reload()
    {
        _services.WeaponCache.CurrentWeapon.Reload();
    }

    public void AddSwitchLock()
    {
        _switchLocks++;
    }

    public void RemoveSwitchLock()
    {
        if (_switchLocks > 0) _switchLocks--;
    }

    private void AttemptSwitchWeapon(bool backwards)
    {
        if (_switchLocks > 0) return;

        if (backwards) _services.WeaponCache.PrevWeapon();
        else _services.WeaponCache.NextWeapon();

        _animator.SetTrigger("SwitchWeapon");
    }

    public void AddInteractLock()
    {
        _interactLocks++;
    }

    public void RemoveInteractLock()
    {
        if (_interactLocks > 0) _interactLocks--;
    }

    private void AttemptInteract()
    {
        if (_interactLocks > 0) return;
    }
}
