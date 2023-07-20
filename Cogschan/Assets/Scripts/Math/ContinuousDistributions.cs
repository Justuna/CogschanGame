using UnityEngine;

/// <summary>
/// Provides a collection of static methods that can be used to model continuous distributions.
/// </summary>
/// <remarks>
/// Unlike finite distributions, continuous distributions can be easily modeled on a case-by-case basis, so instead we'll just code some common ones that show up in this project.
/// </remarks>
public static class ContinuousDistributions
{
    /// <summary>
    /// Returns a random point in the annulus perpendicular to the y axis.
    /// </summary>
    /// <remarks>
    /// The radius is distributed such that the joint distribution is uniform. 
    /// </remarks>
    public static Vector3 GetRandomPointInAnnulus(float innerRadius, float outerRadius, Vector3 center)
    {
        float radius = Mathf.Sqrt(Random.Range(Mathf.Pow(innerRadius, 2), Mathf.Pow(outerRadius, 2)));
        float theta = Random.Range(0, 2 * Mathf.PI);
        return new Vector3(radius, theta).CylindricalToCartesian() + center;
    }

    /// <summary>
    /// Returns a random point in the square perpendicular to the y axis.
    /// </summary>
    /// <remarks> The distribution of points is unifrom. </remarks>
    public static Vector3 GetRandomPointInSquare(float minX, float maxX, float minZ, float maxZ, Vector3 center)
        => new Vector3(Random.Range(minX, maxX), 0, Random.Range(minZ, maxZ)) + center;
}