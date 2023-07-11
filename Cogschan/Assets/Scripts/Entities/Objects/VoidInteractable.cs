using UnityEngine;

/// <summary>
/// Returns Cogschan to her last grounded position.
/// </summary>
public class VoidInteractable : Interactable
{
    [SerializeField]
    [Tooltip("The increase in the y coordinate to guarentee the player spawns above the ground.")]
    private float _boost;
    [SerializeField]
    [Tooltip("The increase in the y coordinate in the case the last ground position can not be obtained.")]
    private float _nullBoost;

    protected override void InteractInternal(EntityServiceLocator services)
    {
        if (services.IsPlayer)
        {
            services.CharacterController.enabled = false;
            services.transform.position = (services.GroundChecker.LastGroundPosition + _boost * Vector3.up) ?? (services.transform.position + _nullBoost * Vector3.up);
            services.CharacterController.enabled = true;
            services.KinematicPhysics.DesiredVelocity = Vector3.Project(services.KinematicPhysics.DesiredVelocity, Vector3.up);
        }
        else
        {
            Destroy(services.gameObject);
        }
    }
}