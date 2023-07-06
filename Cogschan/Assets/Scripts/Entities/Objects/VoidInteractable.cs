using UnityEngine;

/// <summary>
/// Returns Cogschan to her last grounded position.
/// </summary>
public class VoidInteractable : Interactable
{
    [SerializeField]
    [Tooltip("The default increase in the y coordinate, in case the last ground position can not be obtained.")]
    private float _defaultBoost;

    protected override void InteractInternal(EntityServiceLocator services)
    {
        services.CharacterController.enabled = false;
        services.transform.position = services.GroundChecker.LastGroundPosition ?? (services.transform.position + _defaultBoost * Vector3.up);
        services.CharacterController.enabled = true;
    }
}