using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// A script for keeping track of an entity's health.
/// </summary>
public class HealthTracker : MonoBehaviour
{
    [SerializeField] private int _maxHealth;

    /// <summary>
    /// An event that is triggered when this entity reaches 0 health.
    /// </summary>
    public CogschanSimpleEvent OnDeath;
    /// <summary>
    /// The current health of this entity.
    /// </summary>
    public int Health { get; private set; }

    private void Start()
    {
        Health = _maxHealth;
    }

    /// <summary>
    /// Sets the entity's health to a certain value.
    /// </summary>
    /// <param name="health">The value to set health to.</param>
    public void SetHealth(int health)
    {
        Health = Mathf.Clamp(health, 0, _maxHealth);

        if (Health == 0) OnDeath?.Invoke();
    }

    /// <summary>
    /// Damages the entity by a certain amount of health points.
    /// </summary>
    /// <param name="amount">The amount to decrease health by.</param>
    public void Damage(int amount)
    {
        SetHealth(Health - amount);
    }

    /// <summary>
    /// Heals the entity by a certain amount of health points.
    /// </summary>
    /// <param name="amount">The amount to increase health by.</param>
    public void Heal(int amount)
    {
        SetHealth(Health + amount);
    }
}
