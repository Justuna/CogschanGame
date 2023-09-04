using UnityEngine;

public class EndDash : StateMachineBehaviour
{
    public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        animator.SetBool("IsDashing", false);
        animator.GetBehaviour<PlayerActionStateMachine>().RemoveFireLock();
    }
}
