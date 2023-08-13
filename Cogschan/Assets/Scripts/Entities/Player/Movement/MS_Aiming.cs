﻿using UnityEngine;

public class MS_Aiming : MonoBehaviour, IMovementState, IMachineStateBehave
{
    [SerializeField] private EntityServiceLocator _services;
    [SerializeField] private GameObject _cogschanModel;
    [SerializeField] private float _aimSpeed = 2;
    [SerializeField] private float _turnSpeed = 50;

    public CogschanSimpleEvent AimingIntoSprinting;
    public CogschanSimpleEvent AimingIntoWalking;
    public CogschanFloatEvent AimingIntoProne;

    public void OnBehave()
    {
        Quaternion dir = Quaternion.Euler(_services.CameraController.CameraLateralDirection);
        Vector3 movement = new Vector3(CogschanInputSingleton.Instance.MovementDirection.x, 0, CogschanInputSingleton.Instance.MovementDirection.y);
        Vector3 movementDir = dir * movement;

        // When aiming, always look forward, not in the direction of movement
        _cogschanModel.transform.rotation = Quaternion.Lerp(_cogschanModel.transform.rotation, dir, _turnSpeed * Time.deltaTime);

        movementDir *= _aimSpeed;

        _services.KinematicPhysics.DesiredVelocity = movementDir;

        if (!CogschanInputSingleton.Instance.IsHoldingAim)
        {
            if (CogschanInputSingleton.Instance.IsHoldingSprint)
            {
                AimingIntoSprinting?.Invoke();
            }
            else
            {
                AimingIntoWalking?.Invoke();
            }
        }
    }

    public void OnDash()
    {
        // Do nothing
    }

    public void OnJump()
    {
        // Do nothing
    }

    public float GetBaseSpeed()
    {
        return _aimSpeed;
    }

    public void OnProne(float duration)
    {
        AimingIntoProne.Invoke(duration);
    }
}
