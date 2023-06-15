using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// A hitbox that persists over an area, dealing damage on a timer.
/// </summary>
[RequireComponent(typeof(Collider))]
public class AOEHitbox : Hitbox
{
    [Tooltip("Whether or not the explosion imparts an impulse.")]
    [SerializeField] protected int _damage;
    [Tooltip("The length of time between two ticks of damage.")]
    [SerializeField] protected float _tickTime;

    /// <summary>
    /// A list for tracking which entities have already been damaged by the explosion.
    /// </summary>
    private List<EntityServiceLocator> _alreadyHurt = new List<EntityServiceLocator>();
    private float _tickTimer = 0;

    public void OnTriggerStay(Collider collider)
    {
        Hurtbox hurtbox = collider.GetComponent<Hurtbox>();
        if (_tickTimer <= 0)
        {
            _tickTimer = _tickTime;
            if (hurtbox != null && !_alreadyHurt.Contains(hurtbox.Services))
            {
                _alreadyHurt.Add(hurtbox.Services);

                if (_isHealing) hurtbox.Services.HealthTracker.Heal(_damage);
                else hurtbox.Services.HealthTracker.Damage(_damage);
            }
        }
        else
        {
            _tickTimer -= Time.fixedDeltaTime;
            _alreadyHurt.Clear();
        }
    }
}
