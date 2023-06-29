using System.Collections;
using UnityEngine;

public class PlayerKinematicPhysics : KinematicPhysics
{
    [SerializeField] private float _airSteerFactor;

    protected override Vector3 PickVelocity()
    {
        Vector3 actualVelocity;

        // Grounded movement is based entirely on input
        if (_services.GroundChecker.IsGrounded && _impulses.Count == 0)
        {
            actualVelocity = DesiredVelocity;
            if (_services.GroundChecker.SurfaceType == GroundChecker.SurfaceTypes.WALKABLE_SLOPE)
            {
                actualVelocity = Quaternion.AngleAxis(_services.GroundChecker.SurfaceAngle.Value, _services.GroundChecker.ZeroDirection.Value) * actualVelocity;
            }
            PreviousVelocity = actualVelocity;
        }
        // Either you're being knocked into the air, or you already were in the air
        // Use previous velocity to calculate momentum, and add new impulses
        // Accelerate in the direction of desired movement at a constant rate
        else
        {
            while (_impulses.Count > 0)
            {
                PreviousVelocity += _impulses.Dequeue() / _mass;
            }

            actualVelocity = ApplyForces(ApplyAirSteering(PreviousVelocity));
        }

        return actualVelocity;
    }

    protected override void MakeMove(Vector3 actualVelocity)
    {
        _services.CharacterController.Move(actualVelocity * Time.deltaTime);
    }

    // Accelerates velocity in the direction of the input; if the resulting speed is faster than the max speed of the current state,
    // also clamps the velocity to match the previous frame's speed.
    // Basically, you can never speed up past the max speed of the state without an outside impulse.
    private Vector3 ApplyAirSteering(Vector3 actualVelocity)
    {
        Vector2 actualVelocityHorizontal = HorizontalVector(actualVelocity);
        Vector2 previousVelocityHorizontal = HorizontalVector(PreviousVelocity);
        Vector2 desiredMovementHorizontal = HorizontalVector(DesiredVelocity.normalized);

        float maxDelta = _airSteerFactor * Time.deltaTime;

        // Prevents steering from overriding gravity on slopes by cancelling out component of velocity going into slope
        if (_services.GroundChecker.SurfaceType == GroundChecker.SurfaceTypes.STEEP_SLOPE)
        {
            float slopePenalty = Mathf.Clamp01(Mathf.Cos(Mathf.Deg2Rad * Vector3.Angle(_services.GroundChecker.SurfaceNormal.Value, DesiredVelocity)) + 1);
            maxDelta *= slopePenalty;
        }

        actualVelocityHorizontal += desiredMovementHorizontal * maxDelta;
        if (actualVelocityHorizontal.magnitude > _services.MovementController.CurrentBaseSpeed)
        {
            actualVelocityHorizontal = Vector2.ClampMagnitude(actualVelocityHorizontal, previousVelocityHorizontal.magnitude);
        }

        actualVelocity = HorizontalTo3Dim(actualVelocityHorizontal, actualVelocity.y);
        return actualVelocity;
    }
}
