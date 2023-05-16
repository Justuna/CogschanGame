using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// A class dedicated to keeping track of the state of input triggers, and also just generally making the input map provided by the auto-generated 
/// <c>CogschanMapping</c> class more accessible.
/// </summary>
public class CogschanInputSingleton : MonoBehaviour
{
    #region Singleton Stuff
    /// <summary>
    /// The only instance of the CogschanInputSingleton class that is allowed to exist.
    /// </summary>
    public static CogschanInputSingleton Instance { get; private set; }

    void Awake()
    {
        if (Instance != null)
        {
            Destroy(this);
        }

        Instance = this;
    }
    #endregion

    private CogschanMapping _inputMapping;

    /// <summary>
    /// The change in horizontal mouse position since the last frame.
    /// </summary>
    public float MouseDeltaHorizontal { get; private set; }
    /// <summary>
    /// The change in vertical mouse position since the last frame.
    /// </summary>
    public float MouseDeltaVertical { get; private set; }
    /// <summary>
    /// The direction in which the player is trying to move.
    /// </summary>
    public Vector2 MovementDirection { get; private set; }
    /// <summary>
    /// Whether or not the player is currently holding down the aiming control.
    /// </summary>
    public bool IsHoldingAim { get; private set; }
    /// <summary>
    /// Whether or not the player is currently holding down the fire control.
    /// </summary>
    public bool IsHoldingFire { get; private set; }
    public bool IsHoldingSprint { get; private set; }
    /// <summary>
    /// Event that fires when the pause button is pressed.
    /// </summary>
    public CogschanSimpleEvent OnPauseButtonPressed;
    /// <summary>
    /// Event that fires when the jump button is pressed.
    /// </summary>
    public CogschanSimpleEvent OnJumpButtonPressed;
    /// <summary>
    /// Event that fires when the dash button is pressed.
    /// </summary>
    public CogschanSimpleEvent OnDashButtonPressed;

    private void Start()
    {
        _inputMapping = new CogschanMapping();
        _inputMapping.Enable();

        _inputMapping.Movement.Jump.performed += _ => { OnJumpButtonPressed.Invoke(); };
        _inputMapping.Movement.BurstDash.performed += _ => { OnDashButtonPressed.Invoke(); };
        _inputMapping.Movement.Sprint.performed += _ => { IsHoldingSprint = true; };
        _inputMapping.Movement.Sprint.canceled += _ => { IsHoldingSprint = false; };

        _inputMapping.Camera.Aim.performed += _ => { IsHoldingAim = true; };
        _inputMapping.Camera.Aim.canceled += _ => { IsHoldingAim = false; };
        
        _inputMapping.Weapon.Fire.performed += _ => { IsHoldingFire = true; };
        _inputMapping.Weapon.Fire.canceled += _ => { IsHoldingFire = false; };

        _inputMapping.Menus.Pause.performed += _ => { OnPauseButtonPressed.Invoke(); };
    }

    private void Update()
    {
        Vector2 mouseDelta = _inputMapping.Camera.LookAround.ReadValue<Vector2>();
        MouseDeltaHorizontal = mouseDelta.x;
        MouseDeltaVertical = mouseDelta.y;

        MovementDirection = _inputMapping.Movement.Direction.ReadValue<Vector2>();
    }
}
