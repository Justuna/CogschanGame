using Cysharp.Threading.Tasks;
using DG.Tweening;
using FMODUnity;
using UnityEngine;

public class MS_Sprinting : MonoBehaviour, IMovementState, IMachineStateLateBehave
{
    [SerializeField] private EntityServiceLocator _services;
    [SerializeField] private EventReference _runningSound;
    [SerializeField] private GameObject _cogschanModel;
    [SerializeField] private float _sprintSpeed = 8;
    [SerializeField] private float _turnLerpDuration = 0.25f;
    [SerializeField] private AnimationCurve _turnLerpCurve;
    [SerializeField] private float _turnAngleDeadzone = 10;

    public CogschanSimpleEvent SprintingIntoAiming;
    public CogschanSimpleEvent SprintingIntoWalking;
    public CogschanSimpleEvent SprintingIntoDashing;
    public CogschanFloatEvent SprintingIntoProne;

    private OneShotTask _turnLerpTask;

    private Vector3 MovementDirection
    {
        get
        {
            Quaternion camDir = Quaternion.Euler(_services.CameraController.CameraLateralDirection);
            Vector3 movement = new Vector3(CogschanInputSingleton.Instance.MovementDirection.x, 0, CogschanInputSingleton.Instance.MovementDirection.y);
            Vector3 movementDir = camDir * movement;    // Movement direction is relative to the camera direction
            return movementDir;
        }
    }

    private void Awake()
    {
        _turnLerpTask = new OneShotTask(async (token) =>
        {
            var startRotation = _cogschanModel.transform.rotation;
            await DOVirtual.Float(0, 1, _turnLerpDuration, (value) =>
            {
                _cogschanModel.transform.rotation = Quaternion.Lerp(startRotation, Quaternion.LookRotation(MovementDirection), _turnLerpCurve.Evaluate(value));
            }).SetEase(Ease.Linear).WithCancellation(token);
        });
    }

    public void OnLateBehave()
    {
        bool audible = false;
        var movementDir = MovementDirection;

        if (movementDir != Vector3.zero)
        {
            Quaternion movementDirQuat = Quaternion.LookRotation(movementDir);
            if (!_turnLerpTask.IsRunning)
            {
                if (Mathf.Abs(Mathf.DeltaAngle(_cogschanModel.transform.eulerAngles.y, movementDirQuat.eulerAngles.y)) >= _turnAngleDeadzone)
                    _turnLerpTask.Run();
                else
                    _cogschanModel.transform.rotation = movementDirQuat;
            }

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
