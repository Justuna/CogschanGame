using UnityEngine;

/// <summary>
/// Interface to represent an object that holds onto and progresses a recoil pattern.
/// </summary>
public interface IRecoilEvent
{
    /// <summary>
    /// Gets the recoil at the current time.
    /// </summary>
    /// <remarks>
    /// Should be called together with Iterate().
    /// </remarks>
    /// <returns>
    /// Returns the recoil at the current time.
    /// </returns>
    Vector2 GetRecoil();

    /// <summary>
    /// Progresses time on the recoil event.
    /// </summary>
    /// <returns>
    /// Returns true if the recoil event has finished, and false otherwise.
    /// </returns>
    bool StepTime();
}