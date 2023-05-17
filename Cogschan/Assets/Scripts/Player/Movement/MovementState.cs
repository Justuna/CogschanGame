using System.Collections;
using System.Collections.Generic;

public interface IMachineState
{
    /// <summary>
    /// What this state should do every frame.
    /// </summary>
    public void Behavior();
}

public interface IMovementState : IMachineState
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
    /// Get the base travel speed of this state.
    /// </summary>
    /// <returns>
    /// Returns the base travel speed of this state.
    /// </returns>
    public float GetBaseSpeed();
}
