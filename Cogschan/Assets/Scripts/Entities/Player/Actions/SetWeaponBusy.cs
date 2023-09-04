using UnityEngine;

public class SetWeaponBusy : StateMachineBehaviour
{
    public override void OnStateMachineEnter(Animator animator, int stateMachinePathHash)
    {
        base.OnStateMachineEnter(animator, stateMachinePathHash);

        animator.SetBool("WeaponBusy", true);
        Debug.Log("firing is now true");
    }

    public override void OnStateMachineExit(Animator animator, int stateMachinePathHash)
    {
        base.OnStateMachineExit(animator, stateMachinePathHash);

        animator.SetBool("WeaponBusy", false);
        Debug.Log("firing is now false");
    }
}