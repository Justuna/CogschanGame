using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// A hitbox that destroys itself and its object immediately after overlapping a hurtbox or level object. Cannot trigger more than once.
/// </summary>
public class HitOnceHitbox : Hitbox
{
    [Tooltip("The damage that this hitbox should deal.")]
    [SerializeField] protected int _damage;
    [Tooltip("The impulse to be delivered to the owner of the hurtbox upon receiving damage. Does not trigger if the vector is zero.")]
    [SerializeField] private Vector3 _impulse = Vector3.zero;
    [Tooltip("Whether or not the impulse can cancel a velocity override on the owner of the hurtbox.")]
    [SerializeField] private bool _canCancelOverride = false;
    [Tooltip("How much of the momentum is maintained if this hitbox cancels the velocity override on a hurtbox.")]
    [SerializeField] private float _maintainedMomentum = 1f;
    [Tooltip("Prefabs to be spawned on impact (additional hitboxes, visual effects, etc).")]
    [SerializeField] private GameObject[] _spawnedOnImpact;

    private bool _hit = false;

    private void OnTriggerEnter(Collider other)
    {
        if (_hit) return;

        Hurtbox hurtbox = other.GetComponent<Hurtbox>();
        if (hurtbox != null)
        {
            if (_isHealing) hurtbox.Services.HealthTracker.Heal(_damage);
            else hurtbox.Services.HealthTracker.Damage(_damage);

            // There is a nonzero possibility that we will have enemies with no physics, so the kinematic physics component might be null

            if (_impulse != Vector3.zero && hurtbox.Services.KinematicPhysics != null) 
                hurtbox.Services.KinematicPhysics?.AddImpulse(_impulse, _canCancelOverride, _maintainedMomentum);
        }

        foreach (GameObject o in _spawnedOnImpact)
        {
            if (o != null)
            {
                Instantiate(o, transform.position, Quaternion.identity);
            }
        }

        // Since destroy does not necessarily fire immediately, we have to make sure manually that this will only run once.
        _hit = true;

        Destroy(gameObject);
    }
}