using UnityEngine;

public class KeepLeftHandOnWeapon : StateMachineBehaviour
{
    public override void OnStateEnter(UnityEngine.Animator animator, UnityEngine.AnimatorStateInfo animatorStateInfo, int layerIndex)
    {
        base.OnStateEnter(animator, animatorStateInfo, layerIndex);

        animator.GetComponentInParent<EntityServiceLocator>().RigController.AddLeftHandIKLock();
    }

    public override void OnStateExit(UnityEngine.Animator animator, UnityEngine.AnimatorStateInfo animatorStateInfo, int layerIndex)
    {
        base.OnStateExit(animator, animatorStateInfo, layerIndex);

        animator.GetComponentInParent<EntityServiceLocator>().RigController.RemoveLeftHandIKLock();
    }
}
