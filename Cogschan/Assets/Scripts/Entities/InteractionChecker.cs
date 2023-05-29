using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// A script that allows entities to interact with interactive objects. Keeps track of which interactables are currently overlapped and provides a pointer to
/// the closest one, or immediately fires the interactable upon entering the trigger if forced.
/// </summary>
[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(Collider))]
public class InteractionChecker : MonoBehaviour
{
    [SerializeField] private EntityServiceLocator _services;

    private List<Interactable> _optInable = new List<Interactable>();

    public Interactable OptIn { get {
        if (_optInable.Count == 0) return null;

        // Multiply by 1000 for more precision before int cast
        // The actual distance doesn't matter, just the relative distances
        // Sort by distance, highest to lowest
        _optInable.Sort((i1, i2) => (int) (1000 * (Vector3.Distance(i1.transform.position, transform.position) - 
                Vector3.Distance(i2.transform.position, transform.position))));
        return _optInable[0];
    }}

    private void OnTriggerEnter(Collider other)
    {
        Interactable interactable = other.GetComponent<Interactable>();
        if (interactable != null)
        {
            if (interactable.ForceInteraction) interactable.Interact(_services);
            else _optInable.Add(interactable);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        Interactable interactable = other.GetComponent<Interactable>();
        if (interactable != null)
        {
            _optInable.Remove(interactable);
        }
    }
}