using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using StarterAssets;

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

public class PlayerInputController : MonoBehaviour
{
    private static PlayerInputController s_singleton;

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
    [SerializeField]
    private float ScrollBuffer;

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
    /// The singleton instance of the <see cref="PlayerInputController"/>.
    /// </summary>
    public static PlayerInputController Singleton
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
    private void Awake()
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
        inputMappings.Movement.Dash.performed += _ => Dash();
        inputMappings.Movement.Interact.performed += _ => Interact();
    }

    // Use the inputs to set the action states and other values.
    private void Update()
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
            

        InputMove = inputMappings.Movement.Move.ReadValue<Vector2>();
        InputLook = inputMappings.Weapon.Look.ReadValue<Vector2>();
        InputLook = new Vector2(InputLook.x, -InputLook.y);
        
        
        Vector2 inputScroll = inputMappings.Weapon.SwitchWeapon.ReadValue<Vector2>();
        if (inputScrollTimer <= 0)
        {
            if (inputScroll.y != 0)
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
    private void Dash() => throw new System.NotImplementedException("The method \"Dash\" is not yet implemented.");
    private void Interact() => throw new System.NotImplementedException("The method \"Interact\" is not yet implemented.");
}