using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using UnityEngine;

/// <summary>
/// A script that holds information about every interactive object.
/// </summary>
[RequireComponent (typeof(Rigidbody))]
[RequireComponent (typeof(Collider))]
public abstract class Interactable : MonoBehaviour
{
    [Tooltip("Whether or not the entity will be forced to interact upon entering the interaction area. If false, the entity must opt-in instead. For the player," +
        "this means pressing the interact button.")]
    [SerializeField] private bool _forcedInteraction;
    [Tooltip("Whether or not the interaction is limited to the player.")]
    [SerializeField] private bool _playerOnly;
    [Tooltip("The message displayed to the player when they overlap the interaction area. If the interaction is forced, this does not display.")]
    [SerializeField] private string _optInMessage = "Press E to interact.";

    /// <summary>
    /// Whether or not the player will be forced to interact upon entering the interaction area. If false, the player must opt-in instead by pressing 
    /// the interact button.
    /// </summary>
    public bool ForceInteraction => _forcedInteraction;
    /// <summary>
    /// The message displayed to the player when they overlap the interaction area. If the interaction is forced, this does not display.
    /// </summary>
    public string OptInMessage => _optInMessage;

    /// <summary>
    /// A method that triggers the interaction on an entity.
    /// </summary>
    /// <param name="services">The service locator of the entity.</param>
    public void Interact(EntityServiceLocator services)
    {
        if (_playerOnly && !services.IsPlayer) return;

        InteractInternal(services);
    }

    /// <summary>
    /// The method responsible for actually implementing the interaction with the entity.
    /// </summary>
    /// <param name="services">The service locator of the entity.</param>
    protected abstract void InteractInternal(EntityServiceLocator services);
}