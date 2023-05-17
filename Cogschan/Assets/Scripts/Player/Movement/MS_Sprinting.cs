using UnityEngine;

public class MS_Sprinting : MonoBehaviour, IMovementState
{
    [SerializeField] private GroundChecker _groundChecker;
    [SerializeField] private PlayerMovementController _playerController;
    [SerializeField] private CogschanKinematicPhysics _movementHandler;
    [SerializeField] private PlayerCameraController _cameraController;
    [SerializeField] private GameObject _cogschanModel;
    [SerializeField] private float _sprintSpeed = 8;
    [SerializeField] private float _turnSpeed = 10;

    public CogschanSimpleEvent SprintingIntoAiming;
    public CogschanSimpleEvent SprintingIntoWalking;
    public CogschanSimpleEvent SprintingIntoDashing;

    public void Behavior()
    {
        Quaternion dir = Quaternion.Euler(_cameraController.CameraLateralDirection);
        Vector3 movement = new Vector3(CogschanInputSingleton.Instance.MovementDirection.x, 0, CogschanInputSingleton.Instance.MovementDirection.y);
        Vector3 movementDir = dir * movement;

        if (movementDir != Vector3.zero)
        {
            Quaternion movementDirQ = Quaternion.LookRotation(movementDir);
            _cogschanModel.transform.rotation = Quaternion.Lerp(_cogschanModel.transform.rotation, movementDirQ, _turnSpeed * Time.deltaTime);
        }

        movementDir *= _sprintSpeed;

        _movementHandler.DesiredVelocity = movementDir;

        if (!CogschanInputSingleton.Instance.IsHoldingSprint)
        {
            if (CogschanInputSingleton.Instance.IsHoldingAim)
            {
                SprintingIntoAiming.Invoke();
            }
            else
            {
                SprintingIntoWalking.Invoke();
            }
        }
    }

    public void OnDash()
    {
        if (_playerController.CanDash) SprintingIntoDashing.Invoke();
    }

    public void OnJump()
    {
        if (_groundChecker.IsGrounded) _movementHandler.AddImpulse(Vector3.up * _playerController.JumpImpulse);
        else Debug.Log("Not grounded!");
    }

    public float GetBaseSpeed()
    {
        return _sprintSpeed;
    }
}
