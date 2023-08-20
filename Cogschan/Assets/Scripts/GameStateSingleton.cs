using FMOD.Studio;
using FMODUnity;
using NaughtyAttributes;
using System;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

#if UNITY_EDITOR
using UnityEditor;
#endif

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
    /// <summary>
    /// The only instance of this class that is allowed to exist.
    /// </summary>
    public static GameStateSingleton Instance { get; private set; }

    public int KeysNeeded => Keys.Length;

    [SerializeField] private Jangling[] _janglings;
    [SerializeField] private KeyDeposit _keyDeposit;
    [SerializeField] private EventReference _backgroundMusic;
    [SerializeField] private int _maxEnemies;

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
    public UnityEvent GotAllKeys;
    /// <summary>
    /// An event that fires when Cogschan collects a key
    /// </summary>
    public UnityEvent<KeyData> KeyCollected;
    /// <summary>
    /// An event that fires when Cogschan actually deposits all of the keys.
    /// </summary>
    /// <remarks>
    /// Does not use <c>CogschanSimpleEvent</c> because we want to be able to define the listeners in the inspector for this event.
    /// </remarks>
    public UnityEvent LevelClear = new UnityEvent();
    /// <summary>
    /// The maximum number of enemies that can be spawned at one time under normal circumstances.
    /// </summary>
    public int MaxEnemies => _maxEnemies;

    private bool _levelCleared = false;
    private EventInstance _bgmInstance;

    private void Awake()
    {
        if (!Application.isPlaying) return;

        if (Instance != null)
        {
            Destroy(this);
            return;
        }

        Instance = this;
        Keys = _janglings.Select(x => new RuntimeKeyData()
        {
            KeyData = x.KeyData
        }).ToArray();

        _keyDeposit.gameObject.SetActive(false);
    }

    private void Start()
    {
        if (!Application.isPlaying) return;

        _bgmInstance = AudioSingleton.Instance.PlayInstance(_backgroundMusic);
        _bgmInstance.start();
    }

    /// <summary>
    /// Collects a key.
    /// </summary>
    public void CollectKey(KeyData instance)
    {
        KeyCollected.Invoke(instance);
        var runtimeKeyData = Keys.FirstOrDefault(x => x.KeyData == instance);
        if (runtimeKeyData == null)
        {
            Debug.LogError("Could not collect key because it does not exist! Is the jangling registered to the GameStateSingleton?");
            return;
        }
        runtimeKeyData.Collected = true;
        if (KeyCount == KeysNeeded)
        {
            _keyDeposit.gameObject.SetActive(true);
            GotAllKeys.Invoke();
        }
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

#if UNITY_EDITOR
    [Button("Autofetch Janglings and Key Deposit")]
    private void AutofetchJanglings()
    {
        Undo.RecordObject(this, "Autofetch Janglings and Key Deposit");
        _janglings = FindObjectsOfType<Jangling>();
        _keyDeposit = FindObjectOfType<KeyDeposit>();
    }
#endif
}