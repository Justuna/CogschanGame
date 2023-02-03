using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : Entity
{
    public Healthbar Healthbar;

    void Awake()
    {
        _health = MaxHealth;
        Healthbar.UpdateValue(1);
    }

    public override void DealDamage(float amount)
    {
        base.DealDamage(amount);
        Healthbar.UpdateValue(_health / MaxHealth);
    }

    public override void HealHealth(float amount)
    {
        base.HealHealth(amount);
        Healthbar.UpdateValue(_health / MaxHealth);
    }
}
