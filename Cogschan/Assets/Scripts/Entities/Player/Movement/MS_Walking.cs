﻿using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;

public class MS_Walking : MonoBehaviour, IMovementState, IMachineStateLateBehave
{
    [SerializeField] private EntityServiceLocator _services;
    [SerializeField] private GameObject _cogschanModel;
    [SerializeField] private float _walkSpeed = 4;
    //[SerializeField] private float _turnSpeed = 50;
    [SerializeField] private float _turnLerpDuration = 0.25f;
    [SerializeField] private AnimationCurve _turnLerpCurve;
    [SerializeField] private float _turnAngleDeadzone = 10;
    [Tooltip("Amount of time before Cogschan switches from always pointing in the camera direction to pointing only in the movement direction.")]
    [SerializeField] private float _combatMovementCooldown = 1.5f;

    public CogschanSimpleEvent WalkingIntoAiming;
    public CogschanSimpleEvent WalkingIntoSprinting;
    public CogschanSimpleEvent WalkingIntoDashing;
    public CogschanFloatEvent WalkingIntoProne;

    private OneShotTask _turnLerpTask;
    private Quaternion CameraDirectionQuat => Quaternion.Euler(_services.CameraController.CameraLateralDirection);

    private void Awake()
    {
        _turnLerpTask = new OneShotTask(async (token) =>
        {
            var startRotation = _cogschanModel.transform.rotation;
            await DOVirtual.Float(0, 1, _turnLerpDuration, (value) =>
            {
                _cogschanModel.transform.rotation = Quaternion.Lerp(startRotation, CameraDirectionQuat, _turnLerpCurve.Evaluate(value));
            }).SetEase(Ease.Linear).WithCancellation(token);
        });
    }

    public void OnLateBehave()
    {
        Quaternion camDir = CameraDirectionQuat;

        if (!_turnLerpTask.IsRunning)
        {
            if (Mathf.Abs(Mathf.DeltaAngle(_cogschanModel.transform.eulerAngles.y, camDir.eulerAngles.y)) >= _turnAngleDeadzone)
                _turnLerpTask.Run();
            else
                _cogschanModel.transform.rotation = camDir;
        }

        Vector3 movement = new Vector3(CogschanInputSingleton.Instance.MovementDirection.x, 0, CogschanInputSingleton.Instance.MovementDirection.y);
        Vector3 movementDir = camDir * movement;    // Movement direction is relative to the camera direction
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
