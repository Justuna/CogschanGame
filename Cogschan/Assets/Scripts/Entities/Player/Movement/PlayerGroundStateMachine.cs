using FMOD.Studio;
using System.Collections;
using UnityEngine;

public class PlayerGroundStateMachine : StateMachineBehaviour
{
    private EntityServiceLocator _services;

    public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        base.OnStateUpdate(animator, stateInfo, layerIndex);

        if (_services == null) _services = animator.GetComponentInParent<EntityServiceLocator>();
        if (_services == null) return;

        animator.SetBool("IsAiming", CogschanInputSingleton.Instance.IsHoldingAim && !animator.GetBool("IsSprinting"));
        animator.SetBool("IsSprinting", CogschanInputSingleton.Instance.IsHoldingSprint && !animator.GetBool("IsAiming") && !animator.GetBool("WeaponBusy"));
    }
}
