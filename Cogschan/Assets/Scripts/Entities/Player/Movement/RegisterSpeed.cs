using UnityEngine;
/// <summary>
/// Class that tells the <c>MovePlayerWithInput</c> script to move the player at this speed.
/// </summary>
public class RegisterSpeed : StateMachineBehaviour
{
    [SerializeField] private float _desiredSpeed;

    public float DesiredSpeed => _desiredSpeed;

    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        animator.GetBehaviour<MovePlayerWithInput>().RegisterSpeed(this);
    }

    public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        animator.GetBehaviour<MovePlayerWithInput>().UnregisterSpeed(this);
    }
}
