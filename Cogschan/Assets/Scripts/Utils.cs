using UnityEngine;

public static class Utils
{
    public static int MaskForLayer(int layer)
    {
        int mask = 0;
        for (int i = 0; i < 32; i++)
        {
            if (!Physics.GetIgnoreLayerCollision(i, layer))
            {
                mask |= 1 << i;
            }
        }
        return mask;
    }

    /// <summary>
    /// Converts a cylindrical vector (rho, phi, z) to a cartesian vector (x, y, z).
    /// </summary>
    /// <remarks>
    /// In this case, rho refers to the radial distance (rho &gt;= 0), phi to the azimuth (0 &lt;= phi &lt; 2pi), and z to the height. <br/>
    /// Note that, in Unity's cartesian coordinates, y refers to the height, not z.
    /// For more info, see https://en.wikipedia.org/wiki/Cylindrical_coordinate_system#Cartesian_coordinates.
    /// </remarks>
    public static Vector3 CylindricalToCartesian(this Vector3 cylindrical)
    {
        float x = cylindrical.x * Mathf.Cos(cylindrical.y);
        float y = cylindrical.x * Mathf.Sin(cylindrical.y);
        return new(x, cylindrical.z, y);
    }

    /// <summary>
    /// Converts a spherical vector (r, theta, phi) to a cartesian vector (x, y, z).
    /// </summary>
    /// <remarks>
    /// In this case, r refers to the radius (r &gt;= 0), theta to the inclination (0 &lt;= theta &lt;= pi), and phi to the azimuth (0 &lt;= phi &lt; 2pi). <br/>
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
