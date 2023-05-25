using UnityEngine;

public abstract class Hitbox : MonoBehaviour
{
    [SerializeField] protected int _damage;
    [SerializeField] protected bool _isHealing = false;
    [SerializeField] protected bool _canCrit = false;
    [SerializeField] protected bool _hasImpulse = false;

    // Public accessors for serialized private fields.
    // Basically, we want them to be publicly readable and changeable from the inspector,
    // but we don't want other scripts to be able to edit their values.

    /// <summary>
    /// The damage that this hitbox deals. If <c>IsHealing</c> is <c>true</c>, then this is the amount of healing instead.
    /// </summary>
    public int Damage => _damage;
    /// <summary>
    /// Whether or not this hitbox heals instead of damaging.
    /// </summary>
    public bool IsHealing => _isHealing;
    /// <summary>
    /// Whether or not this hitbox can benefit from the crit multiplier of a hurtbox.
    /// </summary>
    public bool CanCrit => _canCrit;
    /// <summary>
    /// Whether or not this hitbox should impart an impulse upon the entity it hits.
    /// </summary>
    public bool HasImpulse => _hasImpulse;

    /// <summary>
    /// The impulse that this hitbox should impart.
    /// </summary>
    /// <returns></returns>
    public abstract Vector3 GetImpulse();
}
