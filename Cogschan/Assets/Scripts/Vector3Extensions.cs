using UnityEngine;

/// <summary>
/// Provides extension methods for the <see cref="Vector3"/> struct.
/// </summary>
public static class Vector3Extensions
{
    /// <summary>
    /// Converts a spherical vector (r, theta, phi) to a cartesian vector (x, y, z).
    /// </summary>
    /// <remarks>
    /// In this case, r refers to the radius (r &gt;= 0), theta to the inclination (0 &lt;= theta &lt;= pi), and phi to the azimuth (0 &lt;= phi &lt;= 2pi). <br/>
    /// For more info, see https://en.wikipedia.org/wiki/Spherical_coordinate_system#Cartesian_coordinates.
    /// </remarks>
    public static Vector3 SphericalToCartesian(this Vector3 spherical)
    {
        float x = spherical.x * Mathf.Sin(spherical.y) * Mathf.Cos(spherical.z);
        float y = spherical.x * Mathf.Sin(spherical.y) * Mathf.Sin(spherical.z);
        float z = spherical.x * Mathf.Cos(spherical.y);
        return new(x, y, z);
    }
}