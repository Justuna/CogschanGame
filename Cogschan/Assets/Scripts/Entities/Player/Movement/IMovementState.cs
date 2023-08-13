
/// <summary>
/// An interface that defines a state to be used by the movement state machine. Provides additional hooks for handling 
/// dashing/jumping inputs, being knocked prone, and fetching a base speed value.
/// </summary>
public interface IMovementState
{
    /// <summary>
    /// What this state should do in response to the dash button.
    /// </summary>
    public void OnDash();

    /// <summary>
    /// What this state should do in response to the jump button.
    /// </summary>
    public void OnJump();

    /// <summary>
    /// What this state should do in response to being knocked prone.
    /// </summary>
    /// <param name="duration">
    /// How long the caller would like to knock Cogschan prone for. <para></para>
    /// <remarks>
    /// Whether or not the prone state is actually triggered, or whether or not it actually lasts <c>duration</c> seconds, depends on the state.
    /// Typically, though, the state will comply by triggering the prone state for <c>duration</c> seconds.
    /// </remarks>
    /// </param>
    public void OnProne(float duration);

    /// <summary>
    /// Get the base travel speed of this state.
    /// </summary>
    /// <returns>
    /// Returns the base travel speed of this state.
    /// </returns>
    public float GetBaseSpeed();
}