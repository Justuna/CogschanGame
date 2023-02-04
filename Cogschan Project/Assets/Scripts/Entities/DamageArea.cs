using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider))]
public class DamageArea : MonoBehaviour
{
    [SerializeField]
    private float DamageTime;
    [SerializeField]
    private float DamageAmount;
    [SerializeField]
    private bool AffectPlayer;
    [SerializeField]
    private bool AffectEnemy;

    private float _timer;

    private void OnTriggerStay(Collider other)
    {
        if (_timer > 0) return;

        if (AffectPlayer)
        {
            PlayerHealth playerHealth = other.GetComponent<PlayerHealth>();
            if (playerHealth != null)
            {
                playerHealth.DealDamage(DamageAmount);
                _timer = DamageTime;
            }
        }
        if (AffectEnemy)
        {
            Enemy enemy = other.GetComponent<Enemy>();
            if (enemy != null)
            {
                enemy.DealDamage(DamageAmount);
                _timer = DamageTime;
            }
        }
    }

    private void Update()
    {
        if (_timer > 0) _timer -= Time.deltaTime;
    }
}
