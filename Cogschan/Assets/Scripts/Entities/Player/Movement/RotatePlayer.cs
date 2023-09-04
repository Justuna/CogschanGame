using UnityEngine;

/// <summary>
/// Class that rotates the player in the direction of movement, or
/// in the direction of the camera when requested.
/// </summary>
public class RotatePlayer : StateMachineBehaviour
{
    [SerializeField] private float _turnSpeed = 50;
    private float _aimLocks = 0;

    private EntityServiceLocator _services;

    /// <summary>
    /// Adds a lock to the script to force it to rotate the player with the camera.
    /// While there is at least one lock, the player will rotate with the camera.
    /// </summary>
    public void AddCameraLock()
    {
        _aimLocks++;
    }

    /// <summary>
    /// Removes a lock from the script forcing it to rotate the player with the camera.
    /// If there are no locks remaining, the player will rotate in the direction of movement
    /// and only while moving.
    /// </summary>
    public void RemoveCameraLock() 
    {
        if (_aimLocks > 0) _aimLocks--;
    }

    public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (_services == null) _services = animator.GetComponentInParent<EntityServiceLocator>();
        if (_services == null) return;

        Quaternion camDir = Quaternion.Euler(_services.CameraController.CameraLateralDirection);
        Vector3 movement = new Vector3(CogschanInputSingleton.Instance.MovementDirection.x, 0, CogschanInputSingleton.Instance.MovementDirection.y);
        Vector3 movementDir = camDir * movement;

        GameObject model = _services.Model;

        if (_aimLocks > 0)
        {
            model.transform.rotation = Quaternion.Lerp(model.transform.rotation,
                camDir, _turnSpeed * Time.deltaTime);
        }
        else
        {
            if (movementDir != Vector3.zero)
            {
                model.transform.rotation = Quaternion.Lerp(model.transform.rotation,
                    Quaternion.LookRotation(movementDir), _turnSpeed * Time.deltaTime);
            }
        }
    }
}
