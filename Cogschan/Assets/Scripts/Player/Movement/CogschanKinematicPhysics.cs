using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CogschanKinematicPhysics : MonoBehaviour
{
    [SerializeField] private CharacterController _cc;
    [SerializeField] private GroundChecker _groundChecker;
    [SerializeField] private float _gravityAcceleration;
    [SerializeField] private float _terminalVelocity;
    [SerializeField] private float _airSteering;
    [SerializeField] private float _mass;

    /// <summary>
    /// The vector that represents Cogschan's attempted movement.
    /// </summary>
    [HideInInspector] public Vector3 DesiredVelocity;

    private VelocityOverride _velocityOverride = null;
    private Queue<Vector3> _impulses = new Queue<Vector3>();
    private Vector3 _previousVelocity = Vector3.zero;

    // The acceleration of the player in the air ignoring forces/impulses.
    /* TODO: Provide a better way of finding the acceleration. It should probably vary based on player state/input.
     * For example, if the player is pushed forward, maybe they should slow down slower if they are holding forward and faster if they are holding backwards.
     */
    private float Acceleration => _airSteering;

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
                _previousVelocity = DesiredVelocity;
            }
            // Either you're being knocked into the air, or you already were in the air
            // Use previous velocity to calculate momentum
            else
            {
                actualVelocity = _previousVelocity;
                actualVelocity.y -= _gravityAcceleration * Time.deltaTime;
                actualVelocity.y = Mathf.Max(-_terminalVelocity, actualVelocity.y);

                while (_impulses.Count > 0)
                {
                    actualVelocity += _impulses.Dequeue() / _mass;
                }

                /* Going to comment this out for now
                // Don't want to zero-out momentum if there's no directional input
                if (DesiredVelocity != Vector3.zero)
                {
                    // DesiredVelocity has a y component of 0
                    // Since we only want to lerp between the lateral velocity, set the Y velocities to be the same.
                    DesiredVelocity.y = actualVelocity.y;

                    actualVelocity = Vector3.Lerp(actualVelocity, DesiredVelocity, Time.deltaTime * _airSteering);
                } */

                CalculateActualVelocity(ref actualVelocity);

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
    /// Adds an impulse force to be simulated by the character controller.
    /// </summary>
    /// <param name="impulse">
    /// The force of the impulse.
    /// </param>
    public void AddImpulse(Vector3 impulse)
    {
        _impulses.Enqueue(impulse);
    }

    // Calculates the new velocity by returning the actual velocity to the desired velocity at a constant rate.
    private void CalculateActualVelocity(ref Vector3 actualVelocity)
    {
        Vector2 actualVelocityHorizontal = HorizontalVector(actualVelocity);
        Vector2 desiredVelocityHorizontal = HorizontalVector(DesiredVelocity);

        Vector2 diff = desiredVelocityHorizontal - actualVelocityHorizontal;
        float maxDelta = Acceleration * Time.deltaTime;
        if (diff.magnitude > maxDelta)
            diff = diff.normalized * maxDelta;
        actualVelocityHorizontal += diff;

        actualVelocity = HorizontalTo3Dim(actualVelocityHorizontal, actualVelocity.y);
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
}
