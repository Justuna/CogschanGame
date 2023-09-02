using System;
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

        bool isFiring = _services.ActionController.IsFiring;
        _animator.SetBool("IsFiring", isFiring);

        Vector2 dir = CogschanInputSingleton.Instance.MovementDirection;

        bool isStill = dir == Vector2.zero;
        _animator.SetBool("IsStill", isStill);
        Debug.Log(isStill);
        
        if (!isFiring)
        {
            // If Cogschan is not firing, she will always face forward (for the sake of Jogging blend tree)

            _animator.SetFloat("X", 0);
            _animator.SetFloat("Y", 1);
        }
        else 
        {
            // Either -1, 0, or 1 to make the blend trees simpler

            _animator.SetFloat("X", dir.x == 0 ? 0 : dir.x/Mathf.Abs(dir.x));
            _animator.SetFloat("Y", dir.y == 0 ? 0 : dir.y/Mathf.Abs(dir.y));
        }
        
        Debug.Log(dir);
    }

    public void Jump()
    {
        _animator.SetTrigger("Jump");
    }

    public void StartSwitchWeapon()
    {
        _animator.SetTrigger("SwitchWeapon");
    }
    
    public void FinishSwitchWeapon()
    {
        //Do Stuff
    }
}