using FMOD.Studio;
using FMODUnity;
using System;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

[Serializable]
public class KeyData
{
    public Jangling Jangling { get; set; }
    [field: SerializeField]
    public Color Color { get; set; }
}

public class RuntimeKeyData
{
    public KeyData KeyData { get; set; }
    public bool Collected { get; set; }
}

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

    public int KeysNeeded => Keys.Length;

    [SerializeField] private Jangling[] _janglings;
    [SerializeField] private EventReference _backgroundMusic;

    /// <summary>
    /// Current keys in a level
    /// </summary>
    public RuntimeKeyData[] Keys { get; private set; }
    /// <summary>
    /// The current amount of keys the player has.
    /// </summary>
    /// <remarks>
    /// Wasn't sure if this should be attached to the player or not, but I decided to make it global
    /// on the off-chance that this game is ever multiplayer :eyes:
    /// </remarks>
    public int KeyCount => Keys.Count(x => x.Collected);
    /// <summary>
    /// An event that fires when Cogschan acquires all of the keys necessary to progress.
    /// </summary>
    /// <remarks>
    /// Does not use <c>CogschanSimpleEvent</c> because we want to be able to define the listeners in the inspector for this event.
    /// </remarks>
    public UnityEvent GotAllKeys = new UnityEvent();
    /// <summary>
    /// An event that fires when Cogschan collects a key
    /// </summary>
    public UnityEvent KeyCollected = new UnityEvent();
    /// <summary>
    /// An event that fires when Cogschan actually deposits all of the keys.
    /// </summary>
    /// <remarks>
    /// Does not use <c>CogschanSimpleEvent</c> because we want to be able to define the listeners in the inspector for this event.
    /// </remarks>
    public UnityEvent LevelClear = new UnityEvent();

    private bool _levelCleared = false;
    private EventInstance _bgmInstance;

    public void Start()
    {
        Keys = _janglings.Select(x => new RuntimeKeyData()
        {
            KeyData = x.KeyData
        }).ToArray();

        _bgmInstance = AudioSingleton.Instance.PlayInstance(_backgroundMusic);
        _bgmInstance.start();
    }

    /// <summary>
    /// Collects a key.
    /// </summary>
    public void CollectKey(KeyData instance)
    {
        KeyCollected.Invoke();
        if (KeyCount == KeysNeeded) GotAllKeys.Invoke();
    }

    /// <summary>
    /// Initiates game victory handlers.
    /// </summary>
    public void ClearLevel()
    {
        if (_levelCleared) return;

        Debug.Log("Victory!~");
        _levelCleared = true;
        LevelClear.Invoke();
    }
}