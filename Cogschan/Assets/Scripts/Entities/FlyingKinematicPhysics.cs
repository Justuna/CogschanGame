using UnityEngine;

public class FlyingKinematicPhysics : KinematicPhysics
{
    /// <summary>
    /// The current acceleration direction of the object. Must have a magnitude of less than one
    /// </summary>
    public Vector3 AccelerationDirection
    {
        get => _accelDir;
        set => _accelDir = value / Mathf.Max(value.magnitude, 1);
    }

    [SerializeField]
    [Tooltip("The maximum acceleration of the object. Must be positive.")]
    private float _maxAccel;
    [SerializeField]
    [Tooltip("The maximum speed of the object under normal conditions. Must be positive.")]
    private float _maxSpeed;
    [SerializeField]
    [Tooltip("The multiplier applied to the velocity of the object at high speeds. Must be between 0 and 1.")]
    [Range(0, 1)]
    private float _dragMultiplier;

    private Vector3 _accelDir;

    protected override Vector3 PickVelocity()
    {
        Vector3 velocity = _previousVelocity + _accelDir * _maxAccel;
        if (velocity.magnitude > _maxSpeed)
            velocity *= Mathf.Min(_previousVelocity.magnitude / velocity.magnitude, _dragMultiplier);
        _previousVelocity = velocity;
        return velocity;
    }

    protected override void MakeMove(Vector3 actualVelocity)
    {
        _services.CharacterController.Move(actualVelocity);
    }

    // Since in air, this metho should do nothing.
    protected override Vector3 ApplyForces(Vector3 actualVelocity) => actualVelocity;
}
