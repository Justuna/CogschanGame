using UnityEngine;

public class MS_Walking : MonoBehaviour, IMovementState
{
    [SerializeField] private EntityServiceLocator _services;
    [SerializeField] private GameObject _cogschanModel;
    [SerializeField] private float _walkSpeed = 4;
    [SerializeField] private float _turnSpeed = 10;
    [SerializeField] private float _turnSpeedFiring = 50;

    public CogschanSimpleEvent WalkingIntoAiming;
    public CogschanSimpleEvent WalkingIntoSprinting;
    public CogschanSimpleEvent WalkingIntoDashing;
    public CogschanFloatEvent WalkingIntoProne;

    public void Behavior()
    {
        Quaternion dir = Quaternion.Euler(_services.CameraController.CameraLateralDirection);
        Vector3 movement = new Vector3(CogschanInputSingleton.Instance.MovementDirection.x, 0, CogschanInputSingleton.Instance.MovementDirection.y);
        Vector3 movementDir = dir * movement;

        if (movementDir != Vector3.zero && !_services.ActionController.IsFiring)
        {
            Quaternion movementDirQ = Quaternion.LookRotation(movementDir);
            _cogschanModel.transform.rotation = Quaternion.Lerp(_cogschanModel.transform.rotation, movementDirQ, _turnSpeed * Time.deltaTime);
        }
        else if (_services.ActionController.IsFiring)
        {
            _cogschanModel.transform.rotation = Quaternion.Lerp(_cogschanModel.transform.rotation, dir, _turnSpeedFiring * Time.deltaTime);
        }

        movementDir *= _walkSpeed;

        _services.KinematicPhysics.DesiredVelocity = movementDir;

        if (CogschanInputSingleton.Instance.IsHoldingAim)
        {
            WalkingIntoAiming?.Invoke();
        }
        else if (CogschanInputSingleton.Instance.IsHoldingSprint && !_services.ActionController.IsFiring)
        {
            WalkingIntoSprinting?.Invoke();
        }
    }

    public void OnDash()
    {
        if (_services.MovementController.CanDash) WalkingIntoDashing?.Invoke();
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
        return _walkSpeed;
    }

    public void OnProne(float duration)
    {
        WalkingIntoProne.Invoke(duration);
    }
}
