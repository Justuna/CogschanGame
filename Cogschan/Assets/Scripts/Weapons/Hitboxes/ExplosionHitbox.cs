using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// A hitbox that destroys itself and its object immediately after spawning. Affects a whole area, with its effects being inverse to distance from the center.
/// </summary>
public class ExplosionHitbox : Hitbox
{
    [Tooltip("The radius of the explosion.")]
    [SerializeField] protected float _radius;
    [Tooltip("The minimum damage that this hitbox should deal.")]
    [SerializeField] protected int _minDamage;
    [Tooltip("The maximum damage that this hitbox should deal.")]
    [SerializeField] protected int _maxDamage;
    [Tooltip("Whether or not the explosion imparts an impulse.")]
    [SerializeField] protected bool _hasImpulse;
    [Tooltip("The minimum impulse to be delivered to all overlapping hurtboxes upon receiving damage.")]
    [SerializeField] private float _minImpulse = 0;
    [Tooltip("The maximum impulse to be delivered to all overlapping hurtboxes upon receiving damage.")]
    [SerializeField] private float _maxImpulse = 0;
    [Tooltip("Whether or not the impulse is always upward. Recommended to guarantee some amount of airtime, since being grounded immediately stops entities.")]
    [SerializeField] private bool _alwaysUpwards = false;
    [Tooltip("An additional vertical component to be added to the impulse. Again, recommended to guarantee some amount of airtime.")]
    [SerializeField] private float _bonusLift = 0;
    [Tooltip("Whether or not the impulse can cancel a velocity override on the owner of the hurtbox.")]
    [SerializeField] private bool _canCancelOverride = false;
    [Tooltip("How much of the momentum is maintained if this hitbox cancels the velocity override on a hurtbox.")]
    [SerializeField] private float _maintainedMomentum = 1f;

    /// <summary>
    /// A list for tracking which entities have already been damaged by the explosion.
    /// </summary>
    private List<EntityServiceLocator> _alreadyHurt = new List<EntityServiceLocator>();

    public void Update()
    {
        Collider[] inRange = Physics.OverlapSphere(transform.position, _radius, Utils.MaskForLayer(gameObject.layer));
        foreach (Collider collider in inRange)
        {
            Hurtbox hurtbox = collider.GetComponent<Hurtbox>();
            if (hurtbox != null && !_alreadyHurt.Contains(hurtbox.Services))
            {
                _alreadyHurt.Add(hurtbox.Services);

                // Turn the distance from the center into a value from 0 to 1
                // 0 for out of range, 1 for directly in center
                float dist = Vector3.Distance(hurtbox.transform.position, transform.position);
                float t = 1 - Mathf.Clamp01(dist/_radius);

                int damage = (int) Mathf.Lerp(_minDamage, _maxDamage, t);
                if (_isHealing) hurtbox.Services.HealthTracker.Heal(damage);
                else hurtbox.Services.HealthTracker.Damage(damage);

                if (_hasImpulse && hurtbox.Services.KinematicPhysics != null)
                {
                    Vector3 impulse = Mathf.Lerp(_minImpulse, _maxImpulse, t) * (hurtbox.transform.position - transform.position).normalized;
                    if (_alwaysUpwards && impulse.y < 0)
                    {
                        impulse.y *= -1;
                    }
                    impulse.y += _bonusLift;

                    hurtbox.Services.KinematicPhysics.AddImpulse(impulse, _canCancelOverride, _maintainedMomentum);
                }
            }
        }

        Destroy(gameObject);
    }

    public void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, _radius);
    }
}
