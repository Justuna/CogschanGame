using System;
using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// A hitbox that destroys itself and its object immediately after overlapping a hurtbox or level object. Cannot trigger more than once.
/// </summary>
[RequireComponent(typeof(Collider))]
public class HitOnceHitbox : Hitbox
{
    [Serializable]
    public class GameObjectAction
    {
        public GameObject gameObject;
        public bool instantiate;
        public bool rotateToImpactNormal;
    }

    public UnityEvent Impacted;

    [Tooltip("The damage that this hitbox should deal.")]
    [SerializeField] protected int _damage;
    [Tooltip("The impulse to be delivered to the owner of the hurtbox upon receiving damage. Does not trigger if the vector is zero.")]
    [SerializeField] private float _impulseMagnitude = 0;
    [Tooltip("Whether or not the impulse can cancel a velocity override on the owner of the hurtbox.")]
    [SerializeField] private bool _canCancelOverride = false;
    [Tooltip("How much of the momentum is maintained if this hitbox cancels the velocity override on a hurtbox.")]
    [SerializeField] private float _maintainedMomentum = 1f;
    [Tooltip("Actions on gameobjects to be performed on impact (additional hitboxes, visual effects, etc).")]
    [SerializeField] private GameObjectAction[] _impactGameObjectActions;

    private bool _hit = false;

    private void OnCollisionEnter(Collision collision)
    {
        var contactPoint = Vector3.zero;
        var contactNormal = Vector3.zero;
        foreach (var contact in collision.contacts)
        {
            contactPoint += contact.point;
            contactNormal += contact.normal;
        }
        contactPoint /= collision.contactCount;
        contactNormal.Normalize();

        ProcessHit(collision.gameObject, contactPoint, contactNormal);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!(GetComponent<Rigidbody>()?.isKinematic ?? false)) return;
        var hitPoint = other.ClosestPointOnBounds(transform.position);
        var hitNormal = (transform.position - hitPoint).normalized;
        ProcessHit(other.gameObject, hitPoint, hitNormal);
    }

    private void ProcessHit(GameObject other, Vector3 hitPoint, Vector3 hitNormal)
    {
        if (_hit) return;

        Hurtbox hurtbox = other.GetComponent<Hurtbox>();
        if (hurtbox != null)
        {
            if (_isHealing) hurtbox.Services.HealthTracker.Heal(_damage);
            else hurtbox.Services.HealthTracker.Damage(_damage);

            // There is a nonzero possibility that we will have enemies with no physics, so the kinematic physics component might be null

            if (_impulseMagnitude != 0 && hurtbox.Services.KinematicPhysics != null)
                hurtbox.Services.KinematicPhysics?.AddImpulse(_impulseMagnitude * GetComponent<Rigidbody>().velocity.normalized, _canCancelOverride, _maintainedMomentum);
        }

        foreach (var action in _impactGameObjectActions)
            if (action != null && action.gameObject != null)
            {
                var gameObject = action.gameObject;
                if (action.instantiate)
                    gameObject = Instantiate(action.gameObject, transform.position, Quaternion.identity);
                if (action.rotateToImpactNormal)
                {
                    transform.position = hitPoint;
                    gameObject.transform.LookAt(gameObject.transform.position + hitNormal);
                }
            }

        // Since destroy does not necessarily fire immediately, we have to make sure manually that this will only run once.
        _hit = true;
        Impacted.Invoke();

        Destroy(gameObject);
    }
}