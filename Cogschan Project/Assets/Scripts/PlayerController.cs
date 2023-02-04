using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;

#region State Enums
/// <summary>
/// States for movement.
/// </summary>
public enum MovementState
{
    Jog,
    Run,
    ADS,
}

/// <summary>
/// States for non-movement related actions.
/// </summary>
public enum ActionState
{
    None,
    Fire,
    Reload,
    Dash,
    Interact,
}
#endregion

[RequireComponent(typeof(CharacterController))]
public class PlayerController : MonoBehaviour
{
    #region Starter Assets Stuff
    [Header("Player")]
    [Tooltip("Move speed of the character in m/s")]
    public float MoveSpeed = 2.0f;

    [Tooltip("Sprint speed of the character in m/s")]
    public float SprintSpeed = 5.335f;

    [Tooltip("How fast the character turns to face movement direction")]
    [Range(0.0f, 0.3f)]
    public float RotationSmoothTime = 0.12f;

    [Tooltip("Acceleration and deceleration")]
    public float SpeedChangeRate = 10.0f;

    public float Sensitivity = 1f;

    public AudioClip LandingAudioClip;
    public AudioClip[] FootstepAudioClips;
    [Range(0, 1)] public float FootstepAudioVolume = 0.5f;

    [Space(10)]
    [Tooltip("The height the player can jump")]
    public float JumpHeight = 1.2f;

    [Tooltip("The character uses its own gravity value. The engine default is -9.81f")]
    public float Gravity = -15.0f;

    [Space(10)]
    [Tooltip("Time required to pass before being able to jump again. Set to 0f to instantly jump again")]
    public float JumpTimeout = 0.50f;

    [Tooltip("Time required to pass before entering the fall state. Useful for walking down stairs")]
    public float FallTimeout = 0.15f;

    [Header("Player Grounded")]
    [Tooltip("If the character is grounded or not. Not part of the CharacterController built in grounded check")]
    public bool Grounded = true;

    [Tooltip("Useful for rough ground")]
    public float GroundedOffset = -0.14f;

    [Tooltip("The radius of the grounded check. Should match the radius of the CharacterController")]
    public float GroundedRadius = 0.28f;

    [Tooltip("What layers the character uses as ground")]
    public LayerMask GroundLayers;

    [Header("Cinemachine")]
    [Tooltip("The follow target set in the Cinemachine Virtual Camera that the camera will follow")]
    public GameObject CinemachineCameraTarget;

    [Tooltip("How far in degrees can you move the camera up")]
    public float TopClamp = 70.0f;

    [Tooltip("How far in degrees can you move the camera down")]
    public float BottomClamp = -30.0f;

    [Tooltip("Additional degress to override the camera. Useful for fine tuning camera position when locked")]
    public float CameraAngleOverride = 0.0f;

    [Tooltip("For locking the camera position on all axis")]
    public bool LockCameraPosition = false;

    // cinemachine
    private float _cinemachineTargetYaw;
    private float _cinemachineTargetPitch;

    // player
    private float _speed;
    private float _animationBlend;
    private float _targetRotation = 0.0f;
    private float _rotationVelocity;
    private float _verticalVelocity;
    private float _terminalVelocity = 53.0f;

    // timeout deltatime
    private float _jumpTimeoutDelta;
    private float _fallTimeoutDelta;

    // animation IDs
    private int _animIDSpeed;
    private int _animIDGrounded;
    private int _animIDJump;
    private int _animIDFreeFall;
    private int _animIDMotionSpeed;

    private Animator _animator;
    private CharacterController _controller;
    // All references to StarterAssetsInputs have been replaced with references to PlayerController to make using the state machine less complex.
    // private StarterAssetsInputs _input;
    private GameObject _mainCamera;
    private bool _rotateOnMove = true;

    private const float _threshold = 0.01f;

    private bool _hasAnimator;


    private void StarterAssetsInit()
    {
        // get a reference to our main camera
        if (_mainCamera == null)
        {
            _mainCamera = GameObject.FindGameObjectWithTag("MainCamera");
        }
    }

    private void Start()
    {
        _cinemachineTargetYaw = CinemachineCameraTarget.transform.rotation.eulerAngles.y;

        _hasAnimator = TryGetComponent(out _animator);
        _controller = GetComponent<CharacterController>();

        AssignAnimationIDs();

        // reset our timeouts on start
        _jumpTimeoutDelta = JumpTimeout;
        _fallTimeoutDelta = FallTimeout;
    }

    private void StarterAssetesUpdate()
    {
        _hasAnimator = TryGetComponent(out _animator);

        JumpAndGravity();
        GroundedCheck();
        Move();
    }

    private void LateUpdate()
    {
        CameraRotation();
    }

    private void AssignAnimationIDs()
    {
        _animIDSpeed = Animator.StringToHash("Speed");
        _animIDGrounded = Animator.StringToHash("Grounded");
        _animIDJump = Animator.StringToHash("Jump");
        _animIDFreeFall = Animator.StringToHash("FreeFall");
        _animIDMotionSpeed = Animator.StringToHash("MotionSpeed");
    }

    private void GroundedCheck()
    {
        // set sphere position, with offset
        Vector3 spherePosition = new Vector3(transform.position.x, transform.position.y - GroundedOffset,
            transform.position.z);
        Grounded = Physics.CheckSphere(spherePosition, GroundedRadius, GroundLayers,
            QueryTriggerInteraction.Ignore);

        // update animator if using character
        if (_hasAnimator)
        {
            _animator.SetBool(_animIDGrounded, Grounded);
        }
    }

    private void CameraRotation()
    {
        // if there is an input and camera position is not fixed
        if (Singleton.InputLook.sqrMagnitude >= _threshold && !LockCameraPosition)
        {
            //Don't multiply mouse input by Time.deltaTime;
            //float deltaTimeMultiplier = IsCurrentDeviceMouse ? 1.0f : Time.deltaTime;
            float deltaTimeMultiplier = 1.0f;

            _cinemachineTargetYaw += Singleton.InputLook.x * deltaTimeMultiplier * Sensitivity;
            _cinemachineTargetPitch += Singleton.InputLook.y * deltaTimeMultiplier * Sensitivity;
        }

        // clamp our rotations so our values are limited 360 degrees
        _cinemachineTargetYaw = ClampAngle(_cinemachineTargetYaw, float.MinValue, float.MaxValue);
        _cinemachineTargetPitch = ClampAngle(_cinemachineTargetPitch, BottomClamp, TopClamp);

        // Cinemachine will follow this target
        CinemachineCameraTarget.transform.rotation = Quaternion.Euler(_cinemachineTargetPitch + CameraAngleOverride,
            _cinemachineTargetYaw, 0.0f);
    }

    private void Move()
    {
        // set target speed based on move speed, sprint speed and if sprint is pressed
        float targetSpeed = Singleton.MoveState == MovementState.Run ? SprintSpeed : MoveSpeed;

        // a simplistic acceleration and deceleration designed to be easy to remove, replace, or iterate upon

        // note: Vector2's == operator uses approximation so is not floating point error prone, and is cheaper than magnitude
        // if there is no input, set the target speed to 0
        if (Singleton.InputMove == Vector2.zero) targetSpeed = 0.0f;

        // a reference to the players current horizontal velocity
        float currentHorizontalSpeed = new Vector3(_controller.velocity.x, 0.0f, _controller.velocity.z).magnitude;

        float speedOffset = 0.1f;
        float inputMagnitude = /*_input.analogMovement ? _input.move.magnitude : */1f;

        // accelerate or decelerate to target speed
        if (currentHorizontalSpeed < targetSpeed - speedOffset ||
            currentHorizontalSpeed > targetSpeed + speedOffset)
        {
            // creates curved result rather than a linear one giving a more organic speed change
            // note T in Lerp is clamped, so we don't need to clamp our speed
            _speed = Mathf.Lerp(currentHorizontalSpeed, targetSpeed * inputMagnitude,
                Time.deltaTime * SpeedChangeRate);

            // round speed to 3 decimal places
            _speed = Mathf.Round(_speed * 1000f) / 1000f;
        }
        else
        {
            _speed = targetSpeed;
        }

        _animationBlend = Mathf.Lerp(_animationBlend, targetSpeed, Time.deltaTime * SpeedChangeRate);
        if (_animationBlend < 0.01f) _animationBlend = 0f;

        // normalise input direction
        Vector3 inputDirection = new Vector3(Singleton.InputMove.x, 0.0f, Singleton.InputMove.y).normalized;

        // note: Vector2's != operator uses approximation so is not floating point error prone, and is cheaper than magnitude
        // if there is a move input rotate player when the player is moving
        if (Singleton.InputMove != Vector2.zero)
        {
            _targetRotation = Mathf.Atan2(inputDirection.x, inputDirection.z) * Mathf.Rad2Deg +
                              _mainCamera.transform.eulerAngles.y;
            float rotation = Mathf.SmoothDampAngle(transform.eulerAngles.y, _targetRotation, ref _rotationVelocity,
                RotationSmoothTime);

            // rotate to face input direction relative to camera position
            if (_rotateOnMove)
            {
                transform.rotation = Quaternion.Euler(0.0f, rotation, 0.0f);
            }
        }


        Vector3 targetDirection = Quaternion.Euler(0.0f, _targetRotation, 0.0f) * Vector3.forward;

        // move the player
        _controller.Move(targetDirection.normalized * (_speed * Time.deltaTime) +
                         new Vector3(0.0f, _verticalVelocity, 0.0f) * Time.deltaTime);

        // update animator if using character
        if (_hasAnimator)
        {
            _animator.SetFloat(_animIDSpeed, _animationBlend);
            _animator.SetFloat(_animIDMotionSpeed, inputMagnitude);
        }
    }

    private void JumpAndGravity()
    {
        if (Grounded)
        {
            // reset the fall timeout timer
            _fallTimeoutDelta = FallTimeout;

            // update animator if using character
            if (_hasAnimator)
            {
                _animator.SetBool(_animIDJump, false);
                _animator.SetBool(_animIDFreeFall, false);
            }

            // stop our velocity dropping infinitely when grounded
            if (_verticalVelocity < 0.0f)
            {
                _verticalVelocity = -2f;
            }

            // Jump
            if (Singleton.InputJump && _jumpTimeoutDelta <= 0.0f)
            {

                // the square root of H * -2 * G = how much velocity needed to reach desired height
                _verticalVelocity = Mathf.Sqrt(JumpHeight * -2f * Gravity);

                // update animator if using character
                if (_hasAnimator)
                {
                    _animator.SetBool(_animIDJump, true);
                }
            }

            // jump timeout
            if (_jumpTimeoutDelta >= 0.0f)
            {
                _jumpTimeoutDelta -= Time.deltaTime;
            }
        }
        else
        {
            // reset the jump timeout timer
            _jumpTimeoutDelta = JumpTimeout;

            // fall timeout
            if (_fallTimeoutDelta >= 0.0f)
            {
                _fallTimeoutDelta -= Time.deltaTime;
            }
            else
            {
                // update animator if using character
                if (_hasAnimator)
                {
                    _animator.SetBool(_animIDFreeFall, true);
                }
            }

            // if we are not grounded, do not jump
            Singleton.InputJump = false;
        }

        // apply gravity over time if under terminal (multiply by delta time twice to linearly speed up over time)
        if (_verticalVelocity < _terminalVelocity)
        {
            _verticalVelocity += Gravity * Time.deltaTime;
        }
    }

    private static float ClampAngle(float lfAngle, float lfMin, float lfMax)
    {
        if (lfAngle < -360f) lfAngle += 360f;
        if (lfAngle > 360f) lfAngle -= 360f;
        return Mathf.Clamp(lfAngle, lfMin, lfMax);
    }

    private void OnDrawGizmosSelected()
    {
        Color transparentGreen = new Color(0.0f, 1.0f, 0.0f, 0.35f);
        Color transparentRed = new Color(1.0f, 0.0f, 0.0f, 0.35f);

        if (Grounded) Gizmos.color = transparentGreen;
        else Gizmos.color = transparentRed;

        // when selected, draw a gizmo in the position of, and matching radius of, the grounded collider
        Gizmos.DrawSphere(
            new Vector3(transform.position.x, transform.position.y - GroundedOffset, transform.position.z),
            GroundedRadius);
    }

    public void SetSensitivity(float NewSensitivity)
    {
        Sensitivity = NewSensitivity;
    }

    public void SetRotateOnMove(bool newRotateOnMove)
    {
        _rotateOnMove = newRotateOnMove;
    }

    private void OnFootstep(AnimationEvent animationEvent)
    {
        if (animationEvent.animatorClipInfo.weight > 0.5f)
        {
            if (FootstepAudioClips.Length > 0)
            {
                var index = Random.Range(0, FootstepAudioClips.Length);
                AudioSource.PlayClipAtPoint(FootstepAudioClips[index], transform.TransformPoint(_controller.center), FootstepAudioVolume);
            }
        }
    }

    private void OnLand(AnimationEvent animationEvent)
    {
        if (animationEvent.animatorClipInfo.weight > 0.5f)
        {
            AudioSource.PlayClipAtPoint(LandingAudioClip, transform.TransformPoint(_controller.center), FootstepAudioVolume);
        }
    }
    #endregion

    #region State Machine Stuff
    private static PlayerController s_singleton;

    /// <summary>
    /// Don't modify directly. Use <see cref="MoveState"/> instead.
    /// </summary>
    private MovementState _movementState;
    /// <summary>
    /// Don't modify directly. Use <see cref="ActState"/> instead.
    /// </summary>
    private ActionState _actionState;
    private CogschanInputMapping inputMappings;
    private WeaponCache weapons;

    // The booleans that determine running, firing, and aiming based on inputs.
    private bool inputRun = false;
    private bool inputFire = false;
    private bool inputAim = false;

    // A float counter that determines whether or not to scroll.
    private float inputScrollTimer = 0;

    // The amount of time in seconds that the mouse wheel has to be scrolling in one direction for to count as a complete input.
    [Space(10)]
    [Header("Input")]
    [SerializeField]
    private float ScrollBuffer;

    [Space(10)]
    [Header("Dash Settings")]
    [SerializeField]
    [Tooltip("The length of time the dash lasts.")]
    private float dashTimer;
    [SerializeField]
    [Tooltip("The speed of the dash.")]
    private float dashSpeed;

    /// <summary>
    /// The Vector2 that determines the movement direction based on input.
    /// </summary>
    public Vector2 InputMove { get; private set; }
    /// <summary>
    /// The Vector2 that determines the change in camera angle based on input.
    /// </summary>
    public Vector2 InputLook { get; private set; }
    /// <summary>
    /// The bool that determines if the player will jump based on input.
    /// </summary>
    [HideInInspector]
    public bool InputJump;

    /// <summary>
    /// The singleton instance of the <see cref="PlayerController"/>.
    /// </summary>
    public static PlayerController Singleton
    {
        get { return s_singleton; }
        private set
        {
            if (s_singleton == null)
                s_singleton = value;
            else if (s_singleton != value)
                Destroy(value);
        }
    }

    /// <summary>
    /// The movement state of the player. Changes based on player inputs and the state machine.
    /// </summary>
    /// <remarks> Jump is not part of the movement states</remarks>
    public MovementState MoveState
    {
        get { return _movementState; }
        private set
        {
            switch (value)
            {
                case MovementState.Jog:
                    break;
                case MovementState.Run:
                    if (_movementState == MovementState.ADS || _actionState == ActionState.Fire
                        || _actionState == ActionState.Dash || _actionState == ActionState.Interact)
                        return;
                    break;
                case MovementState.ADS:
                    if (_movementState == MovementState.Run || _actionState == ActionState.Dash
                        || _actionState == ActionState.Interact)
                        return;
                    break;
            }
            _movementState = value;
        }
    }

    /// <summary>
    /// The action state of the player. Changes based on player inputs and the state machine.
    /// </summary>
    public ActionState ActState
    {
        get { return _actionState; }
        private set
        {
            switch (value)
            {
                case ActionState.None:
                    break;
                case ActionState.Fire:
                    if (_actionState == ActionState.Reload || _movementState == MovementState.Run
                        || _actionState == ActionState.Dash || _actionState == ActionState.Interact)
                        return;
                    break;
                case ActionState.Reload:
                    if (_actionState == ActionState.Dash || _actionState == ActionState.Interact)
                        return;
                    break;
                case ActionState.Dash:
                    if (_movementState == MovementState.ADS || _actionState == ActionState.Interact)
                        return;
                    _movementState = MovementState.Jog;
                    break;
                case ActionState.Interact:
                    if (_actionState == ActionState.Dash)
                        return;
                    _movementState = MovementState.Jog;
                    break;
            }
            _actionState = value;
        }
    }

    // Initialize the singleton and gun and set up the input map.
    private void StateInit()
    {
        Singleton = this;
        weapons = GetComponent<WeaponCache>();

        inputMappings = new CogschanInputMapping();
        inputMappings.Enable();
        inputMappings.Movement.Run.started += _ => inputRun = true;
        inputMappings.Movement.Run.canceled += _ => inputRun = false;
        inputMappings.Weapon.Reload.performed += _ => weapons.StartReload();
        inputMappings.Weapon.Shoot.started += _ => inputFire = true;
        inputMappings.Weapon.Shoot.canceled += _ => inputFire = false;
        inputMappings.Weapon.Aim.started += _ => inputAim = true;
        inputMappings.Weapon.Aim.canceled += _ => inputAim = false;
        inputMappings.Movement.Jump.started += _ => { if (!inputAim) { InputJump = true; } };
        inputMappings.Movement.Jump.canceled += _ => InputJump = false;
        inputMappings.Movement.Dash.performed += _ => ActState = ActionState.Dash;
        inputMappings.Movement.Interact.performed += _ => Interact();
    }

    // Use the inputs to set the action states and other values.
    private void StateUpdate()
    {
        // Set the movement and action states.
        if (inputRun)
            MoveState = MovementState.Run;
        else if (MoveState == MovementState.Run)
            MoveState = MovementState.Jog;

        if (inputFire)
            ActState = ActionState.Fire;
        else if (ActState == ActionState.Fire)
            ActState = ActionState.None;

        if (inputAim)
        {
            MoveState = MovementState.ADS;
            InputJump = false;
        }
        else if (MoveState == MovementState.ADS)
            MoveState = MovementState.Jog;

        if (weapons.IsReloading)
            ActState = ActionState.Reload;
        else if (ActState == ActionState.Reload)
        {
            weapons.FinishReload();
            ActState = ActionState.None;
        }
        if (ActState == ActionState.Dash)
            StartCoroutine(Dash());

        InputMove = ActState == ActionState.Dash ? Vector2.zero : inputMappings.Movement.Move.ReadValue<Vector2>();
        InputLook = inputMappings.Weapon.Look.ReadValue<Vector2>();
        InputLook = new Vector2(InputLook.x, -InputLook.y);


        Vector2 inputScroll = inputMappings.Weapon.SwitchWeapon.ReadValue<Vector2>();
        if (inputScrollTimer <= 0)
        {
            if (inputScroll.y != 0 && ActState == ActionState.None && weapons.CanFire && !weapons.IsReloading)
            {
                inputScrollTimer = ScrollBuffer;
                if (inputScroll.y > 0) weapons.NextWeapon();
                else weapons.PrevWeapon();
            }
        }
        else inputScrollTimer -= Time.deltaTime;
    }

    // Enable input mappings.
    private void OnEnable()
    {
        inputMappings.Enable();
    }

    // Disable input mappings.
    private void OnDisable()
    {
        inputMappings.Disable();
    }

    // Lock the cursor upon focusing the application.
    private void OnApplicationFocus(bool hasFocus)
    {
        Cursor.lockState = CursorLockMode.Locked;
    }

    // TODO: Actually write these methods (maybe in a seperate script?).
    private IEnumerator Dash()
    {
        Vector3 vel = _controller.velocity;
        vel = new Vector3(vel.x, 0, vel.z);
        print(vel);
        if (vel == Vector3.zero)
        {
            vel = Vector3.forward; // TODO: Why is this not working? it seems like no matter the vector here, the direction is the same.
            print(vel);
        }
        vel = dashSpeed * vel.normalized;
        float timer = 0;
        while (timer < dashTimer)
        {
            _controller.Move(vel * Time.deltaTime);
            timer += Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }
        ActState = ActionState.None;
    }

    private void Interact() => throw new System.NotImplementedException("The method \"Interact\" is not yet implemented.");
    #endregion

    #region Camera Stuff
    [Space(10)]
    [Header("Camera Stuff")]
    [SerializeField]
    [Tooltip("The camera that the ADS mode uses.")]
    protected CinemachineVirtualCamera aimVirtualCamera;
    [SerializeField]
    [Tooltip("The camera sensitivity when out of ADS.")]
    protected float normalSensitivity;
    [SerializeField]
    [Tooltip("The camera sensitivity when in ADS mode.")]
    protected float aimSensitivity;
    [SerializeField]
    [Tooltip("How far ahead of the camera raycasts should start (to avoid obstacles).")]
    public float forwardCameraDisplacement;

    private void CameraUpdate()
    {
        if (MoveState == MovementState.ADS || ActState == ActionState.Fire)
        {
            if (MoveState == MovementState.ADS)
            {
                aimVirtualCamera.gameObject.SetActive(true);
                SetSensitivity(aimSensitivity);
            }

            SetRotateOnMove(false);
            Vector3 worldAimTarget = Vector3.zero;
            Vector2 screenCenterPoint = new Vector2(Screen.width / 2f, Screen.height / 2f);
            Ray cameraRay = new Ray(Camera.main.ScreenToWorldPoint(screenCenterPoint) + (Camera.main.transform.forward) * forwardCameraDisplacement, Camera.main.transform.forward);
            if (Physics.Raycast(cameraRay, out RaycastHit raycastHit, Mathf.Infinity))
            {
                worldAimTarget = raycastHit.point;
                worldAimTarget.y = transform.position.y;
            }

            Vector3 aimDirection = (worldAimTarget - transform.position).normalized;

            transform.forward = Vector3.Lerp(transform.forward, aimDirection, Time.deltaTime * 20f);
        }
        else
        {
            aimVirtualCamera.gameObject.SetActive(false);
            SetSensitivity(normalSensitivity);
            SetRotateOnMove(true);
        }
    }

    public void SetAimCamera(CinemachineVirtualCamera camera)
    {
        aimVirtualCamera = camera;
    }
    #endregion

    private void Awake()
    {
        StarterAssetsInit();
        StateInit();
    }

    private void Update()
    {
        StarterAssetesUpdate();
        StateUpdate();
        CameraUpdate();
    }
}