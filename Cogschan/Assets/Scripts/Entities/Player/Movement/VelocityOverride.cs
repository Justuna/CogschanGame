using UnityEditor;
using UnityEngine;

public interface VelocityOverride
{
    /// <summary>
    /// The velocity of the override.
    /// </summary>
    /// <returns>
    /// Returns the velocity of the override.
    /// </returns>
    public Vector3 GetVelocity();

    /// <summary>
    /// Whether or not the velocity override can be removed.
    /// </summary>
    /// <returns>
    /// True if the velocity override is done.
    /// False if the velocity override is still operating.
    /// </returns>
    public bool IsFinished();

    /// <summary>
    /// How much of the velocity is maintained as momentum after the velocity override ends.
    /// </summary>
    /// <returns>
    /// Returns a float representing the factor of velocity that should be maintained as momentum.
    /// </returns>
    public float MaintainMomentumFactor();
}