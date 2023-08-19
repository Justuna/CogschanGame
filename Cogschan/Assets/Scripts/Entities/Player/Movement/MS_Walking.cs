using UnityEngine;

public class MS_Walking : MonoBehaviour, IMovementState, IMachineStateLateBehave
{
    [SerializeField] private EntityServiceLocator _services;
    [SerializeField] private GameObject _cogschanModel;
    [SerializeField] private float _walkSpeed = 4;
    [SerializeField] private float _turnSpeed = 50;
    [Tooltip("Amount of time before Cogschan switches from always pointing in the camera direction to pointing only in the movement direction.")]
    [SerializeField] private float _combatMovementCooldown = 1.5f;
    [SerializeField] private AS_Firing _firingState;

    public CogschanSimpleEvent WalkingIntoAiming;
    public CogschanSimpleEvent WalkingIntoSprinting;
    public CogschanSimpleEvent WalkingIntoDashing;
    public CogschanFloatEvent WalkingIntoProne;

    private float _combatMovementCooldownTime = 0f;
    private Quaternion CameraDirectionQuat => Quaternion.Euler(_services.CameraController.CameraLateralDirection);

    private void Start()
    {
        _firingState.Used += OnCombatAction;
    }

    public void OnCombatAction()
    {
        _combatMovementCooldownTime = _combatMovementCooldown;
    }

    public void OnLateBehave()
    {
        Quaternion camDir = CameraDirectionQuat;
        Vector3 movement = new Vector3(CogschanInputSingleton.Instance.MovementDirection.x, 0, CogschanInputSingleton.Instance.MovementDirection.y);
        Vector3 movementDir = camDir * movement;    // Movement direction is relative to the camera direction
        movementDir *= _walkSpeed;

        if (_combatMovementCooldownTime > 0)
        {
            _combatMovementCooldownTime -= Time.deltaTime;
            _cogschanModel.transform.rotation = Quaternion.Lerp(_cogschanModel.transform.rotation, camDir, _turnSpeed * Time.deltaTime);
        }
        else if (movementDir != Vector3.zero)
            _cogschanModel.transform.rotation = Quaternion.Lerp(_cogschanModel.transform.rotation, Quaternion.LookRotation(movementDir), _turnSpeed * Time.deltaTime);

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
            _services.Animator.SetTrigger("Jump");
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
