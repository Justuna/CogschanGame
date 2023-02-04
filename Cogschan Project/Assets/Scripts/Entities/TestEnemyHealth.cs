using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Script for testing health without guns
// Damages the enemy for 10 HP every second
public class TestEnemyHealth : MonoBehaviour
{
    public Enemy Enemy;

    void Start()
    {
        StartCoroutine(DamageEnemy());
    }

    IEnumerator DamageEnemy()
    {
        yield return new WaitForSeconds(1f);
        Enemy.DealDamage(10);
        StartCoroutine(DamageEnemy());
    }
}
