using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// A hitbox that persists over an area that can only hit once per activation. Meant for melee attacks that want an active hitbox for X frames.
/// </summary>
[RequireComponent(typeof(Collider))]
public class MeleeHitbox : Hitbox
{
    [Tooltip("Whether or not the explosion imparts an impulse.")]
    [SerializeField] protected int _damage;
    [Tooltip("The impulse to be delivered to the owner of the hurtbox upon receiving damage. Does not trigger if the vector is zero.")]
    [SerializeField] private Vector3 _impulse = Vector3.zero;
    [Tooltip("Whether or not the impulse can cancel a velocity override on the owner of the hurtbox.")]
    [SerializeField] private bool _canCancelOverride = false;
    [Tooltip("How much of the momentum is maintained if this hitbox cancels the velocity override on a hurtbox.")]
    [SerializeField] private float _maintainedMomentum = 1f;
    [Tooltip("Prefabs to be spawned on impact (additional hitboxes, visual effects, etc).")]
    [SerializeField] private GameObject[] _spawnedOnImpact;

    /// <summary>
    /// A list for tracking which entities have already been damaged by the explosion.
    /// </summary>
    private List<EntityServiceLocator> _alreadyHurt = new List<EntityServiceLocator>();
    private bool _active;

    public void OnTriggerStay(Collider collider)
    {
        Hurtbox hurtbox = collider.GetComponent<Hurtbox>();
        if (_active && hurtbox != null && !_alreadyHurt.Contains(hurtbox.Services))
        {
            _alreadyHurt.Add(hurtbox.Services);

            if (_isHealing)
            {
                hurtbox.Services.HealthTracker.Heal(_damage);
            }
            else
            {
                hurtbox.Services.HealthTracker.Damage(_damage);
                hurtbox.Services.KinematicPhysics.AddImpulse(transform.rotation * _impulse, _canCancelOverride, _maintainedMomentum);
            }

            foreach (GameObject o in _spawnedOnImpact)
            {
                if (o != null)
                {
                    Instantiate(o, transform.position, Quaternion.identity);
                }
            }
        }
    }

    public void Activate()
    {
        _active = true;
    }

    public void Deactivate()
    {
        _active = false;
        _alreadyHurt.Clear();
    }
}
