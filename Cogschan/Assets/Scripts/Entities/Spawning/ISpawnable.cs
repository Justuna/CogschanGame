/// <summary>
/// An interface for all spawnable objects.
/// </summary>
public interface ISpawnable
{
    /// <summary>
    /// The spawn information associated with this object.
    /// </summary>
    public SpawnInfo SpawnInfo { get; }
}
