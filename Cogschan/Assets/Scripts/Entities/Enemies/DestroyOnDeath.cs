using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class DestroyOnDeath : MonoBehaviour
{
    public UnityEvent AfterDeathAnimation = new UnityEvent();

    [SerializeField] private EntityServiceLocator _services;

    void Start()
    {
        _services.HealthTracker.OnDefeat += DeathThroes;
    }

    private void DeathThroes()
    {
        // TODO: Add some sort of death animation that plays first

        Death();
    }

    private void Death()
    {
        AfterDeathAnimation.Invoke();
        Destroy(_services.gameObject);
    }
}
