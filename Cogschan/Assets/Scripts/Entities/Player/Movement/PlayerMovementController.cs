using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor.Rendering;
using UnityEngine;

/// <summary>
/// Script responsible for translating player input into Cogschan's movement.
/// </summary>
/// <remarks>
/// Under the hood, player movement is actually calculated by the different states
/// of a state machine, which itself has to go through the <c>CogschanMovementHandler</c>
/// script that is responsible for actually managing forces and moving the character controller.
/// </remarks>
public class PlayerMovementController : MonoBehaviour
{
    [SerializeField] private MS_Walking ms_Walking;
    [SerializeField] private MS_Sprinting ms_Sprinting;
    [SerializeField] private MS_Aiming ms_Aiming;
    [SerializeField] private MS_Dashing ms_Dashing;
    [SerializeField] private MS_Prone ms_Prone;
    [SerializeField] private float _dashCooldown;
    public float JumpImpulse;

    /// <summary>
    /// The current state of Cogschan. Decides her movement behavior.
    /// </summary>
    private IMovementState _currentState;
    private float _dashTimer;

    /// <summary>
    /// Whether or not Cogschan is mid-dash.
    /// </summary>
    public bool IsWalking { get { return _currentState.Equals(ms_Walking); } }
    /// <summary>
    /// Whether or not Cogschan is aiming down sights.
    /// </summary>
    public bool IsAiming { get { return _currentState.Equals(ms_Aiming); } }
    /// <summary>
    /// Whether or not Cogschan is sprinting.
    /// </summary>
    public bool IsSprinting { get { return _currentState.Equals(ms_Sprinting); } }
    /// <summary>
    /// Whether or not Cogschan is mid-dash.
    /// </summary>
    public bool IsDashing { get { return _currentState.Equals(ms_Dashing); } }
    /// <summary>
    /// Whether or not Cogschan is mid-dash.
    /// </summary>
    public bool IsProne { get { return _currentState.Equals(ms_Prone); } }
    /// <summary>
    /// Whether or not the current movement state should restrict actions from being taken or cancel actions in progress.
    /// Is true if <c>IsDashing</c> or <c>IsProne</c> are true.
    /// </summary>
    public bool CannotAct { get { return IsProne || IsDashing; } }
    /// <summary>
    /// Whether or not Cogschan can dash.
    /// </summary>
    public bool CanDash { get { return _dashTimer <= 0; } }
    /// <summary>
    /// What the base speed of the current state is.
    /// </summary>
    public float CurrentBaseSpeed { get { return _currentState.GetBaseSpeed(); } }

    private void Start()
    {
        _currentState = ms_Walking;

        // Cannot just bind the events to "_movementState.OnDash" etc.
        // because that binds it to the initial state, NOT the current state.
        // This lambda function will force it to reevaluate every time instead.
        CogschanInputSingleton.Instance.OnDashButtonPressed += () => { _currentState.OnDash(); };
        CogschanInputSingleton.Instance.OnJumpButtonPressed += () => { _currentState.OnJump(); };

        ms_Walking.WalkingIntoAiming += WalkingIntoAiming;
        ms_Walking.WalkingIntoSprinting += WalkingIntoSprinting;
        ms_Walking.WalkingIntoDashing += XIntoDashing;
        ms_Walking.WalkingIntoProne += XIntoProne;
        ms_Sprinting.SprintingIntoAiming += SprintingIntoAiming;
        ms_Sprinting.SprintingIntoWalking += SprintingIntoWalking;
        ms_Sprinting.SprintingIntoDashing += XIntoDashing;
        ms_Sprinting.SprintingIntoProne += XIntoProne;
        ms_Aiming.AimingIntoSprinting += AimingIntoSprinting;
        ms_Aiming.AimingIntoWalking += AimingIntoWalking;
        ms_Aiming.AimingIntoProne += XIntoProne;
        ms_Dashing.DashEnded += DashEnded;
        ms_Dashing.DashingIntoProne += DashIntoProne;
        ms_Prone.ProneEnded += ProneEnded;
    }

    private void Update()
    {
        _currentState.Behavior();
        if (_dashTimer > 0) _dashTimer -= Time.deltaTime;
    }

    public void ResetDashCooldown()
    {
        _dashTimer = 0;
    }

    public void KnockProne(float duration)
    {
        _currentState.OnProne(duration);
    }

    #region Glue Methods

    private void WalkingIntoAiming()
    {
        _currentState = ms_Aiming;
    }

    private void WalkingIntoSprinting()
    {
        _currentState = ms_Sprinting;
    }

    private void SprintingIntoAiming()
    {
        _currentState = ms_Aiming;
    }

    private void SprintingIntoWalking()
    {
        _currentState = ms_Walking;
    }

    private void AimingIntoSprinting()
    {
        _currentState = ms_Sprinting;
    }

    private void AimingIntoWalking()
    {
        _currentState = ms_Walking;
    }

    private void XIntoDashing()
    {
        ms_Dashing.Initialize();
        _currentState = ms_Dashing;
    }

    private void XIntoProne(float duration)
    {
        ms_Prone.Initialize(duration);
        _currentState = ms_Prone;
    }

    private void DashIntoProne(float duration)
    {
        ms_Prone.Initialize(duration);
        _currentState = ms_Prone;
        _dashTimer = _dashCooldown;
    }

    private void DashEnded()
    {
        _currentState = ms_Walking;
        _dashTimer = _dashCooldown;
    }

    private void ProneEnded()
    {
        _currentState = ms_Walking;
    }

    #endregion
}
