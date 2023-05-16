﻿using UnityEngine;

public class MS_Walking : MonoBehaviour, IMovementState
{
    [SerializeField] private GroundChecker _groundChecker;
    [SerializeField] private PlayerMovementController _playerController;
    [SerializeField] private CogschanKinematicPhysics _movementHandler;
    [SerializeField] private PlayerCameraController _cameraController;
    [SerializeField] private GameObject _cogschanModel;
    [SerializeField] private float WalkSpeed = 4;
    [SerializeField] private float TurnSpeed = 10;

    public CogschanSimpleEvent WalkingIntoAiming;
    public CogschanSimpleEvent WalkingIntoSprinting;
    public CogschanSimpleEvent WalkingIntoDashing;

    public void Behavior()
    {
        Quaternion dir = Quaternion.Euler(_cameraController.CameraLateralDirection);
        Vector3 movement = new Vector3(CogschanInputSingleton.Instance.MovementDirection.x, 0, CogschanInputSingleton.Instance.MovementDirection.y);
        Vector3 movementDir = dir * movement;

        if (movementDir != Vector3.zero)
        {
            Quaternion movementDirQ = Quaternion.LookRotation(movementDir);
            _cogschanModel.transform.rotation = Quaternion.Lerp(_cogschanModel.transform.rotation, movementDirQ, TurnSpeed * Time.deltaTime);
        }

        movementDir *= WalkSpeed;

        _movementHandler.DesiredVelocity = movementDir;

        if (CogschanInputSingleton.Instance.IsHoldingAim)
        {
            WalkingIntoAiming.Invoke();
        }
        else if (CogschanInputSingleton.Instance.IsHoldingSprint)
        {
            WalkingIntoSprinting.Invoke();
        }
    }

    public void OnDash()
    {
        if (_playerController.CanDash) WalkingIntoDashing.Invoke();
    }

    public void OnJump()
    {
        if (_groundChecker.IsGrounded) _movementHandler.AddImpulse(Vector3.up * _playerController.JumpImpulse);
        else Debug.Log("Not grounded!");
    }
}
