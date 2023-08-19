using System.Collections.Generic;
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
    [Tooltip("The layer(s) considered to be ground.")]
    [SerializeField] private LayerMask _ground;
    [Tooltip("The time it takes for the last ground position to update.")]
    [SerializeField] private float _groundPosUpdate;
    [Tooltip("The angle above which ground is no longer considered stable. Entities will begin sliding off slopes at this angle.")]
    [SerializeField] private float _steepAngle = 45f;
    [Tooltip("The angle above which ground is no longer considered ground.")]
    [SerializeField] private float _wallAngle = 90f;

    [Header("Ground Cast Attributes")]
    [Tooltip("How far above/below Cogschan's feet the center of the ground checker sphere will be.")]
    [SerializeField] private float _yOffset = 0.1f;
    [Tooltip("The radius of the ground checker sphere.")]
    [SerializeField] private float _radius = 1f;
    [Tooltip("How far the ground checker sphere will travel downward before giving up on finding ground.")]
    [SerializeField] private float _castDistance = 0.1f;

    [Header("Mid Ground Cast Attributes")]
    [Tooltip("How far above/below Cogschan's feet the center of the ground checker sphere will be.")]
    [SerializeField] private float _midYOffset = 0.1f;
    [Tooltip("The radius of the ground checker sphere.")]
    [SerializeField] private float _midRadius = 1f;
    [Tooltip("How far the ground checker sphere will travel downward before giving up on finding ground.")]
    [SerializeField] private float _midCastDistance = 0.1f;

    [Header("Entity Services")]
    [Tooltip("The Entity Service Locator this script is a part of.")]
    [SerializeField] private EntityServiceLocator _services;

    [Header("Fall Damage Attributes")]
    [Tooltip("Whether or not the entity this ground checker is attached to should take fall damage. Requires a HealthTracker and KinematicPhysics component on the entity.")]
    [SerializeField] private bool _takesFallDamage = true;
    [Tooltip("The maximum velocity at which no fall damage is taken. Should be positive.")]
    [SerializeField] private float _maxNoHarm;
    [Tooltip("The amount of the Y velocity component converted to damage. Only applies to the portion of the velocity above the minimum fall speed.")]
    [SerializeField] private float _speedToDamageFactor;

    [Header("Debug Attributes")]
    [Tooltip("The color the ground checker sphere will be when not on the ground.")]
    [SerializeField] private Color _notGroundColor;
    [Tooltip("The color the ground checker sphere will be when sliding off a steep slope.")]
    [SerializeField] private Color _steepSlopeColor;
    [Tooltip("The color the ground checker sphere will be when on a walkable slope.")]
    [SerializeField] private Color _walkableSlopeColor;
    [Tooltip("The color the ground checker sphere will be when on flat ground.")]
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
    public Vector3? LastGroundPosition => _groundPosList.Count == 0 ? null : _groundPosList[0];

    private readonly List<Vector3> _groundPosList = new();
    private float _groundPosTimer;
    private RaycastHit[] _sphereCastHits = new RaycastHit[10];

    private void Start()
    {
        SurfaceType = SurfaceTypes.NOT_GROUND;
        SurfaceAngle = null;
        ZeroDirection = null;
        GradientDirection = null;
        SurfaceNormal = null;

        _groundPosTimer = _groundPosUpdate;
    }

    private void Update()
    {
        _groundPosTimer += Time.deltaTime;
        int hitCount = Physics.SphereCastNonAlloc(transform.position + new Vector3(0, _yOffset, 0), _radius, Vector3.down, _sphereCastHits, _castDistance, _ground);
        if (hitCount > 0)
        {
            var surfaceNormal = Vector3.zero;

            for (int i = 0; i < hitCount; i++)
                surfaceNormal += _sphereCastHits[i].normal;
            surfaceNormal = surfaceNormal.normalized;

            SurfaceNormal = surfaceNormal;
            if (surfaceNormal == Vector3.up)
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
                Vector3 temp = Vector3.Cross(surfaceNormal, Vector3.down).normalized;
                // An angle that is perpendicular to both the above vector and the normal vector.
                // Always the direction of greatest descent.
                Vector3 surfaceGradient = Vector3.Cross(temp, surfaceNormal).normalized;
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

            if (SurfaceType == SurfaceTypes.NOT_GROUND || SurfaceType == SurfaceTypes.STEEP_SLOPE)
            {
                // Perform mid cast to detect if we were miscalculating a walkable gap

                hitCount = Physics.SphereCastNonAlloc(transform.position + new Vector3(0, _midYOffset, 0), _midRadius, Vector3.down, _sphereCastHits, _midCastDistance, _ground);
                if (hitCount == 0)
                    SurfaceType = SurfaceTypes.WALKABLE_SLOPE;
            }

            if (_takesFallDamage && SurfaceType != SurfaceTypes.STEEP_SLOPE && SurfaceType != SurfaceTypes.NOT_GROUND)
            {
                var fallDamage = GetFallDamage();
                if (fallDamage > 0)
                    _services.HealthTracker?.Damage(fallDamage);
            }

            if (_groundPosTimer > _groundPosUpdate)
            {
                _groundPosList.Add(transform.position);
                if (_groundPosList.Count > 2)
                    _groundPosList.RemoveAt(0);
                _groundPosTimer = 0;
            }
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

        var color = Gizmos.color;
        color.a = 0.25f;
        Gizmos.color = color;
        initialSphere = new Vector3(0, _midYOffset, 0);
        finalSphere = new Vector3(0, _midYOffset - _midCastDistance, 0);
        Gizmos.DrawSphere(transform.position + initialSphere, _midRadius);
        Gizmos.DrawSphere(transform.position + finalSphere, _midRadius);

        if (GradientDirection.HasValue)
        {
            Gizmos.color = Color.blue;
            Gizmos.DrawLine(transform.position, transform.position + GradientDirection.Value);
        }

    }

    private int GetFallDamage()
    {
        if (!_services.KinematicPhysics) return 0;

        float velocityDown = -_services.KinematicPhysics.PreviousVelocity.y - _maxNoHarm;
        int damage = (int)Mathf.Max(0, velocityDown * _speedToDamageFactor);

        return damage;
    }
}
