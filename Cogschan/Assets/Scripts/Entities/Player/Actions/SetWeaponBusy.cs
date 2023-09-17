using UnityEngine;

public class SetWeaponBusy : StateMachineBehaviour
{
    public override void OnStateMachineEnter(Animator animator, int stateMachinePathHash)
    {
        base.OnStateMachineEnter(animator, stateMachinePathHash);

        animator.SetBool("WeaponBusy", true);
        animator.GetComponentInParent<EntityServiceLocator>().RigController.AddGeneralAimLock();
    }

    public override void OnStateMachineExit(Animator animator, int stateMachinePathHash)
    {
        base.OnStateMachineExit(animator, stateMachinePathHash);

        animator.SetBool("WeaponBusy", false);
        animator.GetComponentInParent<EntityServiceLocator>().RigController.RemoveGeneralAimLock();
    }
}
