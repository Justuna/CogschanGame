using UnityEngine;

public class LeftHandFollowAim : StateMachineBehaviour
{
    public override void OnStateEnter(UnityEngine.Animator animator, UnityEngine.AnimatorStateInfo animatorStateInfo, int layerIndex)
    {
        base.OnStateEnter(animator, animatorStateInfo, layerIndex);

        animator.GetComponentInParent<EntityServiceLocator>().RigController.AddLeftHandAimLock();
    }

    public override void OnStateExit(UnityEngine.Animator animator, UnityEngine.AnimatorStateInfo animatorStateInfo, int layerIndex)
    {
        base.OnStateExit(animator, animatorStateInfo, layerIndex);

        animator.GetComponentInParent<EntityServiceLocator>().RigController.RemoveLeftHandAimLock();
    }
}