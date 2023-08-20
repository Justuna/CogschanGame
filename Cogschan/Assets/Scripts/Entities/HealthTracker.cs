using NaughtyAttributes;
using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// A script for keeping track of an entity's health.
/// </summary>
public class HealthTracker : MonoBehaviour
{
    [Tooltip("The maximum amount of health this entity can have.")]
    [SerializeField] private int _maxHealth;
    [Tooltip("The amount of time after last taking damage before this entity can begin regenerating health again.")]
    [SerializeField] private float _regenResetDuration;
    [Tooltip("The amount of time in between health regeneration ticks.")]
    [SerializeField] private float _regenTickRate;
    [Tooltip("The amount of health regenerated during a tick.")]
    [SerializeField] private int _regenAmount;

    public UnityEvent OnHealthReset;
    /// <summary>
    /// An event that is triggered when this entity takes damage.
    /// </summary>
    public UnityEvent<float> OnDamaged;
    /// <summary>
    /// An event that is triggered when this entity is healed by an outside source.
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

    [SerializeField] private float _regenTimer = 0f;

    private void Awake()
    {
        ResetHealth();
    }

    private void Update()
    {
        if (Time.timeScale == 0) return;

        _regenTimer -= Time.deltaTime;
        if (_regenTimer <= 0f)
        {
            Heal(_regenAmount);
            _regenTimer = _regenTickRate;
        }
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
    /// <returns>Whether or not the entity was actually damaged at all.</returns>
    public bool Damage(int amount)
    {
        if (Health == 0) return false;

        SetHealth(Health - amount);

        OnDamaged?.Invoke(amount);
        _regenTimer = _regenResetDuration;

        return true;
    }

    /// <summary>
    /// Heals the entity by a certain amount of health points.
    /// </summary>
    /// <param name="amount">The amount to increase health by.</param>
    /// <returns>Whether or not the entity was actually healed at all.</returns>
    public bool Heal(int amount)
    {
        if (Health == MaxHealth) return false;
        SetHealth(Health + amount);
        OnHealed?.Invoke(amount);

        return true;
    }
}
