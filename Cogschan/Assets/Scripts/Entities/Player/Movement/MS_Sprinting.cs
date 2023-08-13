using FMOD.Studio;
using FMODUnity;
using UnityEngine;

public class MS_Sprinting : MonoBehaviour, IMovementState
{
    [SerializeField] private EntityServiceLocator _services;
    [SerializeField] private EventReference _runningSound;
    [SerializeField] private GameObject _cogschanModel;
    [SerializeField] private float _sprintSpeed = 8;
    [SerializeField] private float _turnSpeed = 10;

    public CogschanSimpleEvent SprintingIntoAiming;
    public CogschanSimpleEvent SprintingIntoWalking;
    public CogschanSimpleEvent SprintingIntoDashing;
    public CogschanFloatEvent SprintingIntoProne;

    public void Behavior()
    {
        bool audible = false;

        Quaternion dir = Quaternion.Euler(_services.CameraController.CameraLateralDirection);
        Vector3 movement = new Vector3(CogschanInputSingleton.Instance.MovementDirection.x, 0, CogschanInputSingleton.Instance.MovementDirection.y);
        Vector3 movementDir = dir * movement;

        if (movementDir != Vector3.zero)
        {
            Quaternion movementDirQ = Quaternion.LookRotation(movementDir);
            _cogschanModel.transform.rotation = Quaternion.Lerp(_cogschanModel.transform.rotation, movementDirQ, _turnSpeed * Time.deltaTime);

            if (_services.GroundChecker.IsGrounded)
            {
                audible = true;
            }
        }

        movementDir *= _sprintSpeed;

        _services.KinematicPhysics.DesiredVelocity = movementDir;

        if (audible)
        {
            _services.MovementController.RunningSoundInstance.start();
        }
        else
        {
            _services.MovementController.RunningSoundInstance.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
        }

        if (!CogschanInputSingleton.Instance.IsHoldingSprint)
        {
            if (CogschanInputSingleton.Instance.IsHoldingAim)
            {
                SprintingIntoAiming?.Invoke();
            }
            else
            {
                SprintingIntoWalking?.Invoke();
            }
        }
    }

    public void OnDash()
    {
        if (_services.MovementController.CanDash) SprintingIntoDashing?.Invoke();
    }

    public void OnJump()
    {
        if (_services.GroundChecker.IsGrounded)
        {
            _services.KinematicPhysics.AddImpulse(Vector3.up * _services.MovementController.JumpImpulse, false, 0);
        }
    }

    public float GetBaseSpeed()
    {
        return _sprintSpeed;
    }

    public void OnProne(float duration)
    {
        SprintingIntoProne.Invoke(duration);
    }
}
