using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Reflection.Emit;
using Unity.VisualScripting;
using UnityEngine;

/// <summary>
/// A script for checking if a player is grounded. Technically <c>CharacterController</c> has this feature built-in, but it's inconsistent
/// so we're redoing it and adding some extra features.
/// </summary>
[ExecuteInEditMode]
public class GroundChecker : MonoBehaviour
{
    /// <summary>
    /// An enumerated type for classifying potential ground surfaces.
    /// </summary>
    public enum SurfaceTypes { FLAT_GROUND, WALKABLE_SLOPE, STEEP_SLOPE, NOT_GROUND }

    [Header("Ground Attributes")]
    [SerializeField] private LayerMask _ground;
    [SerializeField] private float _steepAngle = 45f;
    [SerializeField] private float _wallAngle = 90f;

    [Header("Sphere Attributes")]
    [SerializeField] private float _yOffset = 0.1f;
    [SerializeField] private float _radius = 1f;

    [Header("Cast Attributes")]
    [SerializeField] private float _castDistance = 0.1f;

    [Header("Debug Attributes")]
    [SerializeField] private Color _notGroundColor;
    [SerializeField] private Color _steepSlopeColor;
    [SerializeField] private Color _walkableSlopeColor;
    [SerializeField] private Color _flatGroundColor;

    /// <summary>
    /// The kind of surface detected by raycast. Differentiates between flat ground, a walkable slope, a steep slope, and not ground.
    /// A slope is steep if the angle is above the steep angle, and it is not even considered ground if the angle is above the wall angle.
    /// </summary>
    /// <remarks>
    /// Ideally, no angle detected will ever be above the wall angle since you generally raycast downwards to find ground, but it exists just in case.
    /// </remarks>
    public SurfaceTypes SurfaceType { get; private set; }
    /// <summary>
    /// Whether or not the surface detected is considered ground (either flat or a walkable slope).
    /// </summary>
    public bool IsGrounded { get { return SurfaceType == SurfaceTypes.FLAT_GROUND || SurfaceType == SurfaceTypes.WALKABLE_SLOPE; } }
    /// <summary>
    /// The angle of the detected surface with respect to flat ground. If there is no ground, returns null.
    /// </summary>
    public float? SurfaceAngle { get; private set; }
    /// <summary>
    /// The direction perpendicular to both the normal and the gradient direction (and also perpendicular to the Y axis). Represents a direction in 
    /// which elevation will not increase or decrease, AKA a rate of change of zero. If there is no ground or if the ground is flat, returns null.
    /// </summary>
    /// <remarks>
    /// Since it is perpendicular to both the normal and the gradient, this direction is useful because you can rotate XZ-plane directions around it to
    /// map them to the slope.
    /// </remarks>
    public Vector3? ZeroDirection { get; private set; }
    /// <summary>
    /// The direction in which the slope is decreasing the greatest (at the point of contact). If there is no ground or if the ground is flat, returns null.
    /// </summary>
    public Vector3? GradientDirection { get; private set; }
    /// <summary>
    /// The normal of the surface at the point of contact. IF there is no ground, returns null.
    /// </summary>
    public Vector3? SurfaceNormal { get; private set; }
    /// <summary>
    /// The last grounded position of Cogschan.
    /// </summary>
    public Vector3? LastGroundPosition { get; private set; }

    private void Start()
    {
        SurfaceType = SurfaceTypes.NOT_GROUND;
        SurfaceAngle = null;
        ZeroDirection = null;
        GradientDirection = null;
        SurfaceNormal = null;
        LastGroundPosition = null;
    }

    private void Update()
    {
        Ray sphereRay = new Ray(transform.position + new Vector3(0, _yOffset, 0), Vector3.down);
        if (Physics.SphereCast(sphereRay, _radius, out RaycastHit sphereHit, _castDistance, _ground))
        {
            SurfaceNormal = sphereHit.normal;
            if (sphereHit.normal == Vector3.up)
            {
                SurfaceType = SurfaceTypes.FLAT_GROUND;
                SurfaceAngle = 0;
                ZeroDirection = null;
                GradientDirection = null;
            }
            else
            {
                // An angle perpendicular to both the normal vector and the y axis.
                // Since it's perpendicular to the y-axis, it has a y component of zero.
                Vector3 temp = Vector3.Cross(sphereHit.normal, Vector3.down).normalized;
                // An angle that is perpendicular to both the above vector and the normal vector.
                // Always the direction of greatest descent.
                Vector3 surfaceGradient = Vector3.Cross(temp, sphereHit.normal).normalized;
                Vector3 surfaceGradientFlat = new Vector3(surfaceGradient.x, 0, surfaceGradient.z);
                SurfaceAngle = Vector3.Angle(surfaceGradient, surfaceGradientFlat);
                ZeroDirection = temp;
                GradientDirection = surfaceGradient;

                if (SurfaceAngle >= _wallAngle)
                {
                    SurfaceType = SurfaceTypes.NOT_GROUND;
                }
                else if (SurfaceAngle >= _steepAngle)
                {
                    SurfaceType = SurfaceTypes.STEEP_SLOPE;
                }
                else
                {
                    SurfaceType = SurfaceTypes.WALKABLE_SLOPE;
                }
            }
            LastGroundPosition = transform.position;
        }
        else
        {
            SurfaceType = SurfaceTypes.NOT_GROUND;
            SurfaceAngle = null;
            ZeroDirection = null;
            GradientDirection = null;
            SurfaceNormal = null;
        }
    }

    private void OnDrawGizmos()
    {
        if (SurfaceType == SurfaceTypes.FLAT_GROUND)
        {
            Gizmos.color = _flatGroundColor;
        }
        else if (SurfaceType == SurfaceTypes.WALKABLE_SLOPE)
        {
            Gizmos.color = _walkableSlopeColor;
        }
        else if (SurfaceType == SurfaceTypes.STEEP_SLOPE)
        {
            Gizmos.color = _steepSlopeColor;
        }
        else
        {
            Gizmos.color = _notGroundColor;
        }

        Vector3 initialSphere = new Vector3(0, _yOffset, 0);
        Vector3 finalSphere = new Vector3(0, _yOffset - _castDistance, 0);
        Gizmos.DrawSphere(transform.position + initialSphere, _radius);
        Gizmos.DrawSphere(transform.position + finalSphere, _radius);

        if (GradientDirection.HasValue)
        {
            Gizmos.color = Color.blue;
            Gizmos.DrawLine(transform.position, transform.position + GradientDirection.Value);
        }

    }
}
