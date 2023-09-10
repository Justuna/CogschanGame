using System;
using UnityEngine;

public class PlayerAirStateMachine : StateMachineBehaviour
{
    [SerializeField] private float _dashSpeed;
    [SerializeField] private float _dashCooldown;
    [SerializeField] private float _dashRetainedMomentum;
    [SerializeField] private float _dashJumpRetainedMomentum;
    public int MaxJumps = 1;
    public float JumpImpulse;

    private bool _wasGrounded = true;

    private int _jumps = 0;
    private int _jumpLocks = 0;

    private float _dashTimer;
    private int _dashLocks = 0;

    private EntityServiceLocator _services;
    private Animator _animator;
    private bool _initialized = false;

    public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        base.OnStateUpdate(animator, stateInfo, layerIndex);

        if (_services == null) _services = animator.GetComponentInParent<EntityServiceLocator>();
        if (_animator == null) _animator = animator;
        if (!_initialized && _services != null && _animator != null && CogschanInputSingleton.Instance != null)
        { 
            {
                _initialized = true;
                CogschanInputSingleton.Instance.OnJumpButtonPressed += AttemptJump;
                CogschanInputSingleton.Instance.OnDashButtonPressed += AttemptDash;
            }
        }

        if (_services == null || _animator == null || !_initialized) return;

        bool grounded = _services.GroundChecker.IsGrounded;
        if (_wasGrounded != grounded)
        {
            _wasGrounded = grounded;
            if (!grounded) animator.SetTrigger("BeginFall");
            else
            {
                animator.SetTrigger("BeginLand");
                ResetJumps();
            }
        }

        animator.SetBool("IsGrounded", grounded);

        if (_dashTimer > 0) _dashTimer -= Time.deltaTime;
    }

    /// <summary>
    /// Adds a lock on the jump action. If there is at least one lock on the jump action, jumps cannot be performed.
    /// </summary>
    public void AddJumpLock()
    {
        _jumpLocks++;
    }

    /// <summary>
    /// Removes a lock from the jump action. If there are no locks on the jump action, jumps can be performed.
    /// </summary>
    public void RemoveJumpLock()
    {
        if (_jumpLocks > 0) _jumpLocks--;
    }

    /// <summary>
    /// Partially refunds some of the jumps in the jump counter, allowing the player to use more jumps.
    /// </summary>
    /// <param name="jumps">The amount of jumps to refund.</param>
    public void RefundJumps(int jumps)
    {
        _jumps = Mathf.Max(_jumps - jumps, 0);
    }

    /// <summary>
    /// Resets the jump counter, allowing the player to use their maximum number of jumps again.
    /// </summary>
    public void ResetJumps()
    {
        _jumps = 0;
    }

    /// <summary>
    /// Attempts to make the player jump. Fails if the jump action is locked or if all of the player's jumps have been used.
    /// </summary>
    private void AttemptJump()
    {
        if (_jumps >= MaxJumps || _jumpLocks > 0) return;

        _animator.SetTrigger("Jump");
        _jumps++;

        _services.KinematicPhysics.RemoveComponent(Vector3.down);
        _services.KinematicPhysics.AddImpulse(Vector3.up * JumpImpulse, true, _dashJumpRetainedMomentum);

        // In case you cancelled your dash
        _animator.SetBool("IsDashing", false);
    }

    /// <summary>
    /// Adds a lock on the dash action. If there is at least one lock on the dash action, dashes cannot be performed.
    /// </summary>
    public void AddDashLock()
    {
        _dashLocks++;
    }

    /// <summary>
    /// Removes a lock from the dash action. If there are no locks on the dash action, dashes can be performed.
    /// </summary>
    public void RemoveDashLock()
    {
        if (_dashLocks > 0) _dashLocks--;
    }

    /// <summary>
    /// Resets the dash cooldown, allowing the player to dash once again.
    /// </summary>
    public void ResetDash()
    {
        _dashCooldown = 0;
    }

    /// <summary>
    /// Attempts to make the player dash. Fails if the dash action is locked, or if the dash cooldown has not expired.
    /// </summary>
    private void AttemptDash()
    {
        // Cannot dash if something is locking it, or if it's on cooldown
        if (_dashTimer > 0 || _dashLocks > 0) return;

        _animator.SetBool("IsDashing", true);
        _animator.GetBehaviour<PlayerActionStateMachine>().AddFireLock();
        _animator.GetBehaviour<PlayerActionStateMachine>().AddReloadLock();
        _animator.GetBehaviour<PlayerActionStateMachine>().AddSwitchLock();

        Quaternion camDir = Quaternion.Euler(_services.CameraController.CameraLateralDirection);
        Vector3 movement = new Vector3(CogschanInputSingleton.Instance.MovementDirection.x, 0, CogschanInputSingleton.Instance.MovementDirection.y);
        
        if (movement == Vector3.zero) movement = Vector3.forward;
        
        Vector3 movementDir = camDir * movement * _dashSpeed;

        ConstantVelocityOverride dash = new ConstantVelocityOverride(movementDir, () => !_services.Animator.GetBool("IsDashing"), _dashRetainedMomentum);
        _services.KinematicPhysics.OverrideVelocity(dash);

        _dashTimer = _dashCooldown;
    }
}
