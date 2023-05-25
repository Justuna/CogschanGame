using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MS_Dashing : MonoBehaviour, IMovementState
{
    [SerializeField] private PlayerServiceLocator _services;
    [SerializeField] private GameObject _cogschanModel;
    [SerializeField] private float _dashSpeed = 30;
    [SerializeField] private float _turnSpeed = 10;
    [SerializeField] private float _duration;
    [SerializeField] private float _momentumFactor;

    private float _timer;
    private Quaternion _movementDirQ;

    public CogschanSimpleEvent DashEnded;
    public CogschanFloatEvent DashingIntoProne;

    public void Initialize()
    {
        _timer = _duration;
        Quaternion dir = Quaternion.Euler(_services.CameraController.CameraLateralDirection);
        Vector3 movement = new Vector3(CogschanInputSingleton.Instance.MovementDirection.x, 0, CogschanInputSingleton.Instance.MovementDirection.y);
        if (movement == Vector3.zero) movement = Vector3.forward;
        Vector3 movementDir = dir * movement;

        movementDir *= _dashSpeed;
        ConstantVelocityOverride dash = new ConstantVelocityOverride(movementDir, () => !_services.MovementController.IsDashing, _momentumFactor);
        _services.KinematicPhysics.OverrideVelocity(dash);

        _movementDirQ = Quaternion.LookRotation(movementDir);
    }

    public void Behavior()
    {
        _timer -= Time.deltaTime;

        _cogschanModel.transform.rotation = Quaternion.Lerp(_cogschanModel.transform.rotation, _movementDirQ, _turnSpeed * Time.deltaTime);

        if (_timer <= 0 )
        {
            DashEnded.Invoke();
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
        return _dashSpeed;
    }

    public void OnProne(float duration)
    {
        DashingIntoProne.Invoke(duration);
    }
}
