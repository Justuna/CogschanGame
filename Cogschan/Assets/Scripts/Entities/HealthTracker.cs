using NaughtyAttributes;
using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// A script for keeping track of an entity's health.
/// </summary>
public class HealthTracker : MonoBehaviour
{
    [SerializeField] private int _maxHealth;

    public UnityEvent OnHealthReset;
    /// <summary>
    /// An event that is triggered when this entity takes damage.
    /// </summary>
    public UnityEvent<float> OnDamaged;
    /// <summary>
    /// An event that is triggered when this entity is healed.
    /// </summary>
    public UnityEvent<float> OnHealed;
    /// <summary>
    /// An event that is triggered when this entity reaches 0 health, but before it is set to be destroyed.
    /// </summary>
    public UnityEvent BeforeDefeat;
    /// <summary>
    /// An event that is triggered when this entity has been set to be destroyed following its health reaching 0.
    /// </summary>
    public UnityEvent OnDefeat;
    [field: ReadOnly]
    [field: SerializeField]
    /// <summary>
    /// The current health of this entity.
    /// </summary>
    public int Health { get; private set; }
    /// <summary>
    /// The maximum health that this entity can (normally) have.
    /// </summary>
    public int MaxHealth => _maxHealth;

    private void Awake()
    {
        ResetHealth();
    }

    public void ResetHealth()
    {
        Health = _maxHealth;
        OnHealthReset?.Invoke();
    }

    /// <summary>
    /// Sets the entity's health to a certain value.
    /// </summary>
    /// <param name="health">The value to set health to.</param>
    public void SetHealth(int health)
    {
        // If dead, should not be able to do more damage or heal
        if (Health <= 0) return;

        Health = Mathf.Clamp(health, 0, _maxHealth);

        if (Health == 0)
        {
            BeforeDefeat?.Invoke();

            // Just in case an on-death listener restores some health
            if (Health == 0)
            {
                OnDefeat?.Invoke();
            }
        }
    }

    /// <summary>
    /// Damages the entity by a certain amount of health points.
    /// </summary>
    /// <param name="amount">The amount to decrease health by.</param>
    public void Damage(int amount)
    {
        if (Health == 0) return;
        SetHealth(Health - amount);
        OnDamaged?.Invoke(amount);
    }

    /// <summary>
    /// Heals the entity by a certain amount of health points.
    /// </summary>
    /// <param name="amount">The amount to increase health by.</param>
    public void Heal(int amount)
    {
        if (Health == MaxHealth) return;
        SetHealth(Health + amount);
        OnHealed?.Invoke(amount);
    }
}
