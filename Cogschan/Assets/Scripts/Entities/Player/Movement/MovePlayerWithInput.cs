using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Class that moves the player at the speed it is told to move at. 
/// If it is told to move at multiple speeds, moves at the average speed.
/// </summary>
public class MovePlayerWithInput : StateMachineBehaviour
{
    private EntityServiceLocator _services;
    HashSet<RegisterSpeed> _speeds = new HashSet<RegisterSpeed>();

    /// <summary>
    /// Tells the script to move at the speed defined by the registrant.
    /// If multiple registrants are registered, the speeds will be averaged.
    /// </summary>
    /// <param name="registrant">The registrant who is requesting to move at a certain speed.</param>
    public void RegisterSpeed(RegisterSpeed registrant)
    {
        _speeds.Add(registrant);
    }

    /// <summary>
    /// Tells the script to stop considering the registrant's speed during movement.
    /// If no registrants are registered, the speed will be zero.
    /// </summary>
    /// <param name="registrant"></param>
    public void UnregisterSpeed(RegisterSpeed registrant)
    {
        _speeds.Remove(registrant);
    }

    public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        base.OnStateUpdate(animator, stateInfo, layerIndex);

        if (_services == null) _services = animator.GetComponentInParent<EntityServiceLocator>();
        if (_services == null) return;

        float averageSpeed = 0;
        foreach (RegisterSpeed registrar in _speeds)
        {
            averageSpeed += registrar.DesiredSpeed;
        }
        if (_speeds.Count > 0) averageSpeed /= _speeds.Count;

        Vector2 dir = CogschanInputSingleton.Instance.MovementDirection;
        animator.SetFloat("X", dir.x);
        animator.SetFloat("Y", dir.y);

        Quaternion camDir = Quaternion.Euler(_services.CameraController.CameraLateralDirection);
        Vector3 movement = new Vector3(dir.x, 0, dir.y);
        Vector3 movementDir = camDir * movement;
        movementDir *= averageSpeed;

        animator.SetFloat("BaseSpeed", averageSpeed);

        _services.KinematicPhysics.DesiredVelocity = movementDir;
    }
}
