using UnityEngine;

public class MS_Walking : MonoBehaviour, IMovementState
{
    [SerializeField] private GroundChecker _groundChecker;
    [SerializeField] private CogschanKinematicPhysics _movementHandler;
    [SerializeField] private PlayerMovementController _movementController;
    [SerializeField] private PlayerActionController _actionController;
    [SerializeField] private PlayerCameraController _cameraController;
    [SerializeField] private GameObject _cogschanModel;
    [SerializeField] private float _walkSpeed = 4;
    [SerializeField] private float _turnSpeed = 10;

    public CogschanSimpleEvent WalkingIntoAiming;
    public CogschanSimpleEvent WalkingIntoSprinting;
    public CogschanSimpleEvent WalkingIntoDashing;
    public CogschanFloatEvent WalkingIntoProne;

    public void Behavior()
    {
        Quaternion dir = Quaternion.Euler(_cameraController.CameraLateralDirection);
        Vector3 movement = new Vector3(CogschanInputSingleton.Instance.MovementDirection.x, 0, CogschanInputSingleton.Instance.MovementDirection.y);
        Vector3 movementDir = dir * movement;

        if (movementDir != Vector3.zero && !_actionController.IsFiring)
        {
            Quaternion movementDirQ = Quaternion.LookRotation(movementDir);
            _cogschanModel.transform.rotation = Quaternion.Lerp(_cogschanModel.transform.rotation, movementDirQ, _turnSpeed * Time.deltaTime);
        }
        else if (_actionController.IsFiring)
        {
            _cogschanModel.transform.rotation = Quaternion.Lerp(_cogschanModel.transform.rotation, dir, _turnSpeed * Time.deltaTime);
        }

        movementDir *= _walkSpeed;

        _movementHandler.DesiredVelocity = movementDir;

        if (CogschanInputSingleton.Instance.IsHoldingAim)
        {
            WalkingIntoAiming.Invoke();
        }
        else if (CogschanInputSingleton.Instance.IsHoldingSprint && !_actionController.IsFiring)
        {
            WalkingIntoSprinting.Invoke();
        }
    }

    public void OnDash()
    {
        if (_movementController.CanDash) WalkingIntoDashing.Invoke();
    }

    public void OnJump()
    {
        if (_groundChecker.IsGrounded)
        {
            _movementHandler.AddImpulse(Vector3.up * _movementController.JumpImpulse, false, 0);
        }
    }

    public float GetBaseSpeed()
    {
        return _walkSpeed;
    }

    public void OnProne(float duration)
    {
        WalkingIntoProne.Invoke(duration);
    }
}
