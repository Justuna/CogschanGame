using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CogschanKinematicPhysics : MonoBehaviour
{
    [SerializeField] private CharacterController _cc;
    [SerializeField] private GroundChecker _groundChecker;
    [SerializeField] private PlayerMovementController _playerMovementController;
    [SerializeField] private float _gravityAcceleration;
    [SerializeField] private float _normalForceAcceleration;
    [SerializeField] private float _terminalVelocity;
    [SerializeField] private float _airSteerFactor;
    [SerializeField] private float _mass;

    /// <summary>
    /// The vector that represents Cogschan's attempted movement.
    /// </summary>
    [HideInInspector] public Vector3 DesiredVelocity;

    private VelocityOverride _velocityOverride = null;
    private Queue<Vector3> _impulses = new Queue<Vector3>();
    private Vector3 _previousVelocity = Vector3.zero;

    private void LateUpdate()
    {
        if (Time.timeScale == 0f) return;

        Vector3 actualVelocity;

        // If something is overriding Cogschan's velocity, let it take over
        if (_velocityOverride != null)
        {
            actualVelocity = _velocityOverride.GetVelocity();

            if (_velocityOverride.IsFinished())
            {
                _previousVelocity = actualVelocity * _velocityOverride.MaintainMomentumFactor();
                _velocityOverride = null;
            }
        }
        else
        {
            // Grounded movement is based entirely on input
            if (_groundChecker.IsGrounded && _impulses.Count == 0)
            {
                actualVelocity = DesiredVelocity;
                if (_groundChecker.SurfaceType == GroundChecker.SurfaceTypes.WALKABLE_SLOPE)
                {
                    actualVelocity = Quaternion.AngleAxis(_groundChecker.SurfaceAngle.Value, _groundChecker.ZeroDirection.Value) * actualVelocity;
                }
                _previousVelocity = actualVelocity;
            }
            // Either you're being knocked into the air, or you already were in the air
            // Use previous velocity to calculate momentum, and add new impulses
            // Accelerate in the direction of desired movement at a constant rate
            else
            {
                while (_impulses.Count > 0)
                {
                    _previousVelocity += _impulses.Dequeue() / _mass;
                }

                actualVelocity = _previousVelocity;
                actualVelocity = ApplyAirSteering(actualVelocity);
                actualVelocity = ApplyForces(actualVelocity);

                _previousVelocity = actualVelocity;
            }
        }

        _cc.Move(actualVelocity * Time.deltaTime);

        DesiredVelocity = Vector3.zero;
    }

    /// <summary>
    /// Adds a velocity override to the character controller.
    /// </summary>
    /// <param name="velocityOverride">
    /// The velocity override to add.
    /// </param>
    public void OverrideVelocity(VelocityOverride velocityOverride)
    {
        _velocityOverride = velocityOverride;
    }

    /// <summary>
    /// Removes a velocity override from the character controller, if present.
    /// </summary>
    /// <param name="maintainMomentum">
    /// How much of the velocity to maintain as momentum.
    /// </param>
    public void RemoveOverrideVelocity(float maintainMomentum)
    {
        _previousVelocity = _velocityOverride.GetVelocity() * maintainMomentum;
        _velocityOverride = null;
    }

    /// <summary>
    /// Adds an impulse force to be simulated by the character controller.
    /// </summary>
    /// <param name="impulse">
    /// The force of the impulse.
    /// </param>
    /// <param name="cancelOverride">Whether or not the new impulse should cancel any velocity overrides.</param>
    /// <param name="maintainCancelledMomentum">How much of the previous velocity should be maintained by momentum. Only meaningful if <c>cancelOverride</c> is true.</param>
    public void AddImpulse(Vector3 impulse, bool cancelOverride, float maintainCancelledMomentum)
    {
        if (cancelOverride || _velocityOverride == null)
        {
            _impulses.Enqueue(impulse);
            if (_velocityOverride != null)
            {
                RemoveOverrideVelocity(maintainCancelledMomentum);
            }
        }
    }

    // Converts a Vector3 to a Vector2 using the horizontal components (x and z).
    private Vector2 HorizontalVector(Vector3 vec)
    {
        return new(vec.x, vec.z);
    }

    // Converts a horizontal Vector2 and a y component to a Vector3.
    private Vector3 HorizontalTo3Dim(Vector2 vec, float y)
    {
        return new(vec.x, y, vec.y);
    }

    // Accelerates velocity in the direction of the input; if the resulting speed is faster than the max speed of the current state,
    // also clamps the velocity to match the previous frame's speed.
    // Basically, you can never speed up past the max speed of the state without an outside impulse.
    private Vector3 ApplyAirSteering(Vector3 actualVelocity)
    {
        Vector2 actualVelocityHorizontal = HorizontalVector(actualVelocity);
        Vector2 previousVelocityHorizontal = HorizontalVector(_previousVelocity);
        Vector2 desiredMovementHorizontal = HorizontalVector(DesiredVelocity.normalized);

        float maxDelta = _airSteerFactor * Time.deltaTime;
        
        // Prevents steering from overriding gravity on slopes by cancelling out component of velocity going into slope
        if (_groundChecker.SurfaceType == GroundChecker.SurfaceTypes.STEEP_SLOPE)
        {
            float slopePenalty = Mathf.Clamp01(Mathf.Cos(Mathf.Deg2Rad * Vector3.Angle(_groundChecker.SurfaceNormal.Value, DesiredVelocity)) + 1);
            maxDelta *= slopePenalty;
        }

        actualVelocityHorizontal += desiredMovementHorizontal * maxDelta;
        if (actualVelocityHorizontal.magnitude > _playerMovementController.CurrentBaseSpeed)
        {
            actualVelocityHorizontal = Vector2.ClampMagnitude(actualVelocityHorizontal, previousVelocityHorizontal.magnitude);
        }

        actualVelocity = HorizontalTo3Dim(actualVelocityHorizontal, actualVelocity.y);
        return actualVelocity;
    }

    // Accelerates velocity downward based on strength of gravity.
    // If on a steep slope, also applies normal force.
    private Vector3 ApplyForces(Vector3 actualVelocity)
    {
        Vector3 gravity = Vector3.down * _gravityAcceleration * Time.deltaTime;
        actualVelocity += gravity;

        // Simulates normal force of the slope acting on the character, pushing them out of the surface.
        // Makes gravity work correctly on steep slopes instead of catching on the surface.
        // Also prevents player from getting lodged on slope if they are thrown at it at high speed.
        // Kind of wonky math but it works well enough in practice.
        if (_groundChecker.SurfaceType == GroundChecker.SurfaceTypes.STEEP_SLOPE)
        {
            Vector3 normalForce = _groundChecker.SurfaceNormal.Value * _normalForceAcceleration * actualVelocity.magnitude * 
                Mathf.Clamp01(-Mathf.Cos(Mathf.Deg2Rad * Vector3.Angle(_groundChecker.SurfaceNormal.Value, actualVelocity))) * Time.deltaTime;
            actualVelocity += normalForce;
        }

        actualVelocity.y = Mathf.Max(-_terminalVelocity, actualVelocity.y);
        return actualVelocity;
    }
}
