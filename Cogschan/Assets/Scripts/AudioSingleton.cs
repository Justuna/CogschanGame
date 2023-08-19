using FMOD.Studio;
using FMODUnity;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// A class that wraps FMOD functionality and keeps track of sound event references, if necessary. See https://youtu.be/rcBHIOjZDpk
/// </summary>
public class AudioSingleton : MonoBehaviour
{
    #region Singleton Stuff
    /// <summary>
    /// The only instance of this class that is allowed to exist.
    /// </summary>
    public static AudioSingleton Instance { get; private set; }

    void Awake()
    {
        if (Instance != null)
        {
            Destroy(this);
        }

        Instance = this;
    }
    #endregion

    private List<EventInstance> _eventInstances = new List<EventInstance>();

    /// <summary>
    /// Calls on FMOD's version of this function to play a sound event in 3D space.
    /// </summary>
    /// <param name="fmodEvent">The FMOD sound event to play.</param>
    /// <param name="worldPos">The point in space to play the sound at.</param>
    public void PlayOneShot(EventReference fmodEvent, Vector3 worldPos = default)
    {
        RuntimeManager.PlayOneShot(fmodEvent, worldPos);
    }

    /// <summary>
    /// Creates an instance of a continuous sound event that will be cleaned up after by <c>AudioSingleton</c> during scene switches.
    /// </summary>
    /// <param name="fmodEvent">The FMOD sound event to instantiate.</param>
    /// <returns>
    /// Returns a reference to instance of the sound event that can be used to control it.
    /// </returns>
    public EventInstance PlayInstance(EventReference fmodEvent)
    {
        EventInstance instance = RuntimeManager.CreateInstance(fmodEvent);
        _eventInstances.Add(instance);

        return instance;
    }

    /// <summary>
    /// Stops all instantiated sound events and releases their memory.
    /// </summary>
    public void Cleanup()
    {
        foreach (EventInstance instance in _eventInstances)
        {
            instance.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
            instance.release();
        }
        _eventInstances.Clear();
    }

    private void OnDestroy()
    {
        Cleanup();
    }
}
