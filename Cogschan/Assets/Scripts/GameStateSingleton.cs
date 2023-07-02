using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// Singleton holding all of the global game state.
/// </summary>
/// <remarks>
/// Not sure if this needs to exist or not.
/// </remarks>
public class GameStateSingleton : MonoBehaviour
{
    #region Singleton Stuff
    /// <summary>
    /// The only instance of this class that is allowed to exist.
    /// </summary>
    public static GameStateSingleton Instance { get; private set; }

    void Awake()
    {
        if (Instance != null)
        {
            Destroy(this);
        }

        Instance = this;
    }
    #endregion

    [SerializeField] private int _keysNeeded;

    /// <summary>
    /// The current amount of keys the player has.
    /// </summary>
    /// <remarks>
    /// Wasn't sure if this should be attached to the player or not, but I decided to make it global
    /// on the off-chance that this game is ever multiplayer :eyes:
    /// </remarks>
    public int KeyCount { get; private set; }
    /// <summary>
    /// An event that fires when Cogschan acquires all of the keys necessary to progress.
    /// </summary>
    /// <remarks>
    /// Does not use <c>CogschanSimpleEvent</c> because we want to be able to define the listeners in the inspector for this event.
    /// </remarks>
    public UnityEvent GotAllKeys = new UnityEvent();
    /// <summary>
    /// An event that fires when Cogschan actually deposits all of the keys.
    /// </summary>
    /// <remarks>
    /// Does not use <c>CogschanSimpleEvent</c> because we want to be able to define the listeners in the inspector for this event.
    /// </remarks>
    public UnityEvent LevelClear = new UnityEvent();

    private bool _levelCleared = false;

    /// <summary>
    /// Increments the amount of keys that the player has in possession.
    /// </summary>
    public void AddKey()
    {
        KeyCount++;
        if (KeyCount == _keysNeeded) GotAllKeys?.Invoke();
    }

    /// <summary>
    /// Initiates game victory handlers.
    /// </summary>
    public void ClearLevel()
    {
        if (_levelCleared) return;

        Debug.Log("Victory!~");
        _levelCleared = true;
        LevelClear?.Invoke();
    }
}