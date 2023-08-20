using UnityEngine;

public class HealthPickup : Pickup
{
    [Tooltip("The amount to heal the entity by.")]
    [SerializeField] private int _amount;

    protected override bool PickupAction(EntityServiceLocator services)
    {
        return services.HealthTracker.Heal(_amount);
    }
}
