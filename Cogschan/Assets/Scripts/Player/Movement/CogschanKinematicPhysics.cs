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

                // Don't want to zero-out momentum if there's no directional input
                if (DesiredVelocity != Vector3.zero)
                {
                    // DesiredVelocity has a y component of 0
                    // Since we only want to lerp between the lateral velocity, set the Y velocities to be the same.
                    DesiredVelocity.y = actualVelocity.y;

                    actualVelocity = Vector3.Lerp(actualVelocity, DesiredVelocity, Time.deltaTime * _airSteering);
                }

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
}
