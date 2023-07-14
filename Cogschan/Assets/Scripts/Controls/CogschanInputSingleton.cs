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
    /// The only instance of this class that is allowed to exist.
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

    [SerializeField] private float _inputScrollCooldown;

    private CogschanMapping _inputMapping;
    private float _inputScrollTimer;

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
    /// <summary>
    /// Event that fires when the reload button is pressed.
    /// </summary>
    public CogschanSimpleEvent OnReloadButtonPressed;
    /// <summary>
    /// Event that fires when the interact button is pressed.
    /// </summary>
    public CogschanSimpleEvent OnInteractButtonPressed;
    /// <summary>
    /// Event that fires when forward scrolling passes a certain threshold (mouse and keyboard) or next weapon button is pressed (controllers).
    /// </summary>
    public CogschanSimpleEvent OnSwitchNextWeapon;
    /// <summary>
    /// Event that fires when backward scrolling passes a certain threshold (mouse and keyboard). Not available for controllers (currently).
    /// </summary>
    public CogschanSimpleEvent OnSwitchPrevWeapon;

    private void Start()
    {
        _inputMapping = new CogschanMapping();
        _inputMapping.Enable();

        _inputMapping.Movement.Jump.performed += _ => { if (Time.timeScale > 0) OnJumpButtonPressed?.Invoke(); };
        _inputMapping.Movement.BurstDash.performed += _ => { if (Time.timeScale > 0) OnDashButtonPressed?.Invoke(); };
        _inputMapping.Movement.Sprint.performed += _ => { if (Time.timeScale > 0) IsHoldingSprint = true; };
        _inputMapping.Movement.Sprint.canceled += _ => { IsHoldingSprint = false; };
        _inputMapping.Movement.Interact.performed += _ => { if (Time.timeScale > 0) OnInteractButtonPressed?.Invoke(); };

        _inputMapping.Camera.Aim.performed += _ => { if (Time.timeScale > 0) IsHoldingAim = true; };
        _inputMapping.Camera.Aim.canceled += _ => { IsHoldingAim = false; };

        _inputMapping.Weapon.Fire.performed += _ => { if (Time.timeScale > 0) IsHoldingFire = true; };
        _inputMapping.Weapon.Fire.canceled += _ => { IsHoldingFire = false; };
        _inputMapping.Weapon.Reload.performed += _ => { if (Time.timeScale > 0) OnReloadButtonPressed?.Invoke(); };
        _inputMapping.Weapon.NextWeapon.performed += _ => { if (Time.timeScale > 0) OnSwitchNextWeapon?.Invoke(); };

        _inputMapping.Menus.Pause.performed += _ => { OnPauseButtonPressed?.Invoke(); };

        _inputScrollTimer = 0;
    }

    private void Update()
    {
        Vector2 mouseDelta = _inputMapping.Camera.LookAround.ReadValue<Vector2>();
        MouseDeltaHorizontal = mouseDelta.x;
        MouseDeltaVertical = mouseDelta.y;

        Vector2 scrollDelta = _inputMapping.Weapon.SwitchWeapons.ReadValue<Vector2>();
        if (_inputScrollTimer <= 0 && scrollDelta.y != 0)
        {
            _inputScrollTimer = _inputScrollCooldown;
            if (scrollDelta.y > 0)
            {
                OnSwitchNextWeapon?.Invoke();
            }
            else
            {
                OnSwitchPrevWeapon?.Invoke();
            }
        }
        else _inputScrollTimer -= Time.deltaTime;

        MovementDirection = _inputMapping.Movement.Direction.ReadValue<Vector2>();
    }
}
