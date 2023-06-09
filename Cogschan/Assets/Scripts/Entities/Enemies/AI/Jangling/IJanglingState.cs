
/// <summary>
/// An interface that defines a behavioral state to be used by enemy AI.
/// </summary>
/// <summary>
/// An interface that defines a behavioral state to be used by Janglings.
/// </summary>
public interface IJanglingState : IMachineState
{
    public void OnStun();

    public void OnDamaged();
}