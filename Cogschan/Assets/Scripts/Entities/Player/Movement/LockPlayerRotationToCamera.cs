using UnityEngine;

public class LockPlayerRotationToCamera : StateMachineBehaviour
{
    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        animator.GetBehaviour<RotatePlayer>().AddCameraLock();
    }

    public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        animator.GetBehaviour<RotatePlayer>().RemoveCameraLock();
    }
}
