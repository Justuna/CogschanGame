using UnityEngine;

public class ReloadWeapon : StateMachineBehaviour
{
    public override void OnStateMachineEnter(Animator animator, int stateMachinePathHash)
    {
        base.OnStateMachineEnter(animator, stateMachinePathHash);

        animator.SetBool("WeaponReloading", true);
        animator.GetBehaviour<PlayerAirStateMachine>().AddDashLock();
    }

    public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        base.OnStateExit(animator, stateInfo, layerIndex);

        animator.GetBehaviour<PlayerActionStateMachine>().Reload();
        animator.SetBool("WeaponReloading", false);
        animator.GetBehaviour<PlayerAirStateMachine>().RemoveDashLock();
    }
}