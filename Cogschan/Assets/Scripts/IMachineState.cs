/// <summary>
/// An interface that defines a basic state to be used by a state machine. Provides a <c>Behaviour</c> function, meant to be called every update while
/// the current state is active.
/// </summary>
public interface IMachineState
{
    /// <summary>
    /// What this state should do every frame.
    /// </summary>
    public void Behavior();
}
