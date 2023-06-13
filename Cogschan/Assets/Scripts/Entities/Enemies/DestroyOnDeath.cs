using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyOnDeath : MonoBehaviour
{
    [SerializeField] private EntityServiceLocator _services;

    void Start()
    {
        _services.HealthTracker.OnDefeat += DeathThroes;
    }

    private void DeathThroes()
    {
        // TODO: Add some sort of death animation that plays first

        Destroy(_services.gameObject);
    }
}
