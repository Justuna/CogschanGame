public interface IMachineStateEnter
{
    /// <summary>
    /// Called when this state is transitioned into.
    /// </summary>
    void OnEnter();
}

public interface IMachineStateExit
{
    /// <summary>
    /// Called when this state is transitioned out. 
    /// /// </summary>
    void OnExit();
}

public interface IMachineStateLateBehave
{
    /// <summary>
    /// Called on LateUpdate
    /// </summary>
    void OnLateBehave();
}

public interface IMachineStateFixedBehave
{
    /// <summary>
    /// Called on FixedUpdate
    /// </summary>
    void OnFixedBehave();
}

/// <summary>
/// An interface that defines a basic state to be used by a state machine. Provides a <c>Behaviour</c> function, meant to be called every update while
/// the current state is active.
/// </summary>
public interface IMachineStateBehave
{
    /// <summary>
    /// What this state should do every frame.
    /// </summary>
    void OnBehave();
}
