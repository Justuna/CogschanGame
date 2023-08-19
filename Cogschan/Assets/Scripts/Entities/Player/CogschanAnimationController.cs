using UnityEngine;

/// <summary>
/// A wrapper script that handles interactions between the behavioral state machines and the animator's state machine as well as dynamic animation rigs.
/// </summary>
public class CogschanAnimationController : MonoBehaviour
{
    [SerializeField]
    private EntityServiceLocator _services;
    [SerializeField]
    private Animator _animator;

    private void LateUpdate()
    {
        _animator.SetBool("IsAirborne", !_services.GroundChecker.IsGrounded);
        _animator.SetBool("IsAiming", _services.MovementController.IsAiming);
        _animator.SetBool("IsSprinting", _services.MovementController.IsSprinting);
        _animator.SetBool("IsStill", _services.KinematicPhysics.DesiredVelocity == Vector3.zero);
        _animator.SetBool("IsFiring", _services.ActionController.IsFiring);
    }
}