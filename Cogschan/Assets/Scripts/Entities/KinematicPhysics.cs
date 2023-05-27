using System.Collections.Generic;
using UnityEngine;

public abstract class KinematicPhysics : MonoBehaviour
{
    [SerializeField] protected EntityServiceLocator _services;
    [SerializeField] protected float _mass;

    protected IVelocityOverride _velocityOverride = null;
    protected Queue<Vector3> _impulses = new Queue<Vector3>();
    protected Vector3 _previousVelocity = Vector3.zero;

    /// <summary>
    /// The vector that represents Cogschan's attempted movement.
    /// </summary>
    [HideInInspector] public Vector3 DesiredVelocity;

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
            actualVelocity = _previousVelocity = PickVelocity();
        }

        MakeMove(actualVelocity);

        DesiredVelocity = Vector3.zero;
    }

    protected abstract Vector3 PickVelocity();
    protected abstract void MakeMove(Vector3 actualVelocity);

    /// <summary>
    /// Adds a velocity override to the character controller.
    /// </summary>
    /// <param name="velocityOverride">
    /// The velocity override to add.
    /// </param>
    public void OverrideVelocity(IVelocityOverride velocityOverride)
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
    public virtual void AddImpulse(Vector3 impulse, bool cancelOverride, float maintainCancelledMomentum)
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
    protected Vector2 HorizontalVector(Vector3 vec)
    {
        return new(vec.x, vec.z);
    }

    // Converts a horizontal Vector2 and a y component to a Vector3.
    protected Vector3 HorizontalTo3Dim(Vector2 vec, float y)
    {
        return new(vec.x, y, vec.y);
    }

    // Accelerates velocity downward based on strength of gravity.
    // If on a steep slope, also applies normal force.
    protected Vector3 ApplyForces(Vector3 actualVelocity)
    {
        Vector3 gravity = Vector3.down * PhysicsConstantSingleton.Instance.GravityAcceleration * Time.deltaTime;
        actualVelocity += gravity;

        // Simulates normal force of the slope acting on the character, pushing them out of the surface.
        // Makes gravity work correctly on steep slopes instead of catching on the surface.
        // Also prevents player from getting lodged on slope if they are thrown at it at high speed.
        // Kind of wonky math but it works well enough in practice.
        if (_services.GroundChecker.SurfaceType == GroundChecker.SurfaceTypes.STEEP_SLOPE)
        {
            Vector3 normalForce = _services.GroundChecker.SurfaceNormal.Value * actualVelocity.magnitude * PhysicsConstantSingleton.Instance.NormalForceAcceleration *
                Mathf.Clamp01(-Mathf.Cos(Mathf.Deg2Rad * Vector3.Angle(_services.GroundChecker.SurfaceNormal.Value, actualVelocity))) * Time.deltaTime;
            actualVelocity += normalForce;
        }

        actualVelocity.y = Mathf.Max(-PhysicsConstantSingleton.Instance.TerminalVelocity, actualVelocity.y);
        return actualVelocity;
    }
}
