#define TESTING

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
    [Tooltip("The multiplier applied to the velocity of the object at high speeds every second. Must be between 0 (no drag) and 1 (complete drag).")]
    [Range(0, 1)]
    private float _dragMultiplier;

#if TESTING
    private float _timer;
#endif

    private Vector3 _accelDir;

    protected override Vector3 PickVelocity()
    {
#if TESTING
        if (_timer <= Time.deltaTime)
            return Vector3.right * 10 * _maxSpeed;
#endif
        Vector3 velocity = _previousVelocity + _accelDir * _maxAccel;
        if (velocity.magnitude > _maxSpeed * _accelDir.magnitude)
            velocity *= Mathf.Max(_maxSpeed * _accelDir.magnitude / velocity.magnitude, Mathf.Pow(1 -_dragMultiplier, Time.deltaTime));
        _previousVelocity = velocity;
        return velocity;
    }

    protected override void MakeMove(Vector3 actualVelocity)
    {
        _services.CharacterController.Move(actualVelocity);
    }

    // Since in air, this metho should do nothing.
    protected override Vector3 ApplyForces(Vector3 actualVelocity) => actualVelocity;

#if TESTING
    private void Start()
    {
        _previousVelocity = Vector3.right * 10 * _maxSpeed;
    }

    private void Update()
    {
        AccelerationDirection = Vector3.up * Mathf.Sin(_timer);
        _timer += Time.deltaTime;
    }

#endif
}
