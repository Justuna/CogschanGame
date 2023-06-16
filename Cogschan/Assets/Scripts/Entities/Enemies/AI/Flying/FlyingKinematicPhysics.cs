using UnityEngine;
using UnityEngine.InputSystem.WebGL;

public class FlyingKinematicPhysics : KinematicPhysics
{
    [SerializeField]
    [Tooltip("The acceleration of the object. Must be positive.")]
    private float _acceleration;
    [SerializeField]
    [Tooltip("The maximum speed of the object under normal conditions. Must be positive.")]
    private float _maxSpeed; // TODO: The AI has  aspeed, what should i do with this?
    [SerializeField]
    [Tooltip("The multiplier applied to the velocity of the object at high speeds every second. Must be between 0 (no drag) and 1 (complete drag).")]
    [Range(0, 1)]
    private float _dragMultiplier;

    private Vector3 _accelDir;

#if TESTING
    private float _timer;
#endif

    protected override Vector3 PickVelocity()
    {
#if TESTING && HIGH_SPEED
        if (_timer <= Time.deltaTime)
            return Vector3.right * 10 * _maxSpeed;
#endif
        _accelDir = (DesiredVelocity - _previousVelocity).normalized;
        Vector3 velocity = _previousVelocity;
        if (velocity.magnitude > _maxSpeed)
            velocity *= Mathf.Max(_maxSpeed / velocity.magnitude, Mathf.Pow(1 - _dragMultiplier, Time.deltaTime));
        velocity += _accelDir * _acceleration * Time.deltaTime;
        float maxVelThisFrame = Mathf.Max(_maxSpeed, _previousVelocity.magnitude);
        if (velocity.magnitude > maxVelThisFrame)
            velocity = velocity.normalized * Mathf.Max(_maxSpeed, maxVelThisFrame);
        _previousVelocity = velocity;
        return velocity;
    }

    protected override void MakeMove(Vector3 actualVelocity)
    {
        _services.CharacterController.Move(actualVelocity * Time.deltaTime);
    }

    // Since in air, this method should do nothing.
    protected override Vector3 ApplyForces(Vector3 actualVelocity)
    {
        Debug.LogWarning("The method ApplyForces was called on a Flying Kinematic Physics instance. However, this method does nothing. Was this call intended?");
        return actualVelocity;
    }

#if TESTING
    private void Update()
    {
        DesiredVelocity = Vector3.up * Mathf.Sin(_timer);
        _timer += Time.deltaTime;
    }
#endif
}
