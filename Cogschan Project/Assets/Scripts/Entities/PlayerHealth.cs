using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHealth : Entity
{
    [SerializeField]
    [Tooltip("Health regen rate.")]
    private float _regenRate;
    [SerializeField]
    [Tooltip("How long to stop healing after taking damage.")]
    private float _regenPauseLength;
    private float _regenTimer;

    void Start()
    {
        _healthbar = UIManager.Instance.PlayerHealthbar;
        _healthbar.UpdateValue(1);
    }

    private void Update()
    {
        if (_regenTimer > 0)
            _regenTimer -= Time.deltaTime;
        if (_regenTimer <= 0 && _health > 0)
            HealHealth(_regenRate * Time.deltaTime);
    }

    public override void DealDamage(float amount)
    {
        base.DealDamage(amount);
        _regenTimer = _regenPauseLength;
    }

    protected override void Kill()
    {
        // Ragdoll? Probably shouldnt just delete the player
        Debug.Log("Nothing implemented");
    }
}
