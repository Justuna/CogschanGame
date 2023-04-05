using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Entity : MonoBehaviour
{
    [SerializeField]
    protected float MaxHealth = 100;
    public UnityEvent<GameObject> OnDeath;

    [SerializeField]
    protected float _health;
    protected Healthbar _healthbar;

    protected virtual void Awake()
    {
        _health = MaxHealth;
    }

    public virtual void DealDamage(float amount)
    {
        _health = Mathf.Max(_health - amount, 0);
        _healthbar.UpdateValue(_health / MaxHealth);

        if (_health == 0)
        {
            OnDeath.Invoke(gameObject);
            Kill();
        }
    }

    public virtual void HealHealth(float amount)
    {
        _health = Mathf.Min(_health + amount, MaxHealth);
        _healthbar.UpdateValue(_health / MaxHealth);
    }

    protected virtual void Kill()
    {
        Destroy(gameObject);
    }
}
