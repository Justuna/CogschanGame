using UnityEngine;

public class JanglingKeyGrab : Interactable
{
    [SerializeField] private EntityServiceLocator _services;
    [SerializeField] private Jangling _jangling;

    // Quick note: the service locator passed in is of the interacting entity (the player)
    // We don't need to operate on the player objects, so it does not get used
    protected override void InteractInternal(EntityServiceLocator _)
    {
        GameStateSingleton.Instance.CollectKey(_jangling.KeyData);

        // Potentially preceded by some sort of an animation in the future
        Destroy(_services.Jangling.gameObject);
    }
}
