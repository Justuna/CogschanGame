using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ES_Confused : MonoBehaviour, IEnemyState
{
    [SerializeField] private EntityServiceLocator _services;
    [SerializeField] private float _minConfusionTime;
    [SerializeField] private float _maxConfusionTime;

    public CogschanSimpleEvent LOSMade;
    public CogschanSimpleEvent Bored;

    private float _confusionTimer = 0;

    public void Init()
    {
        _confusionTimer = UnityEngine.Random.Range(_minConfusionTime, _maxConfusionTime);
    }

    public void Behavior()
    {
        /*if (_services.LOSChecker.CanSee)
        {
            LOSMade?.Invoke();
            return;
        }*/

        if (_confusionTimer <= 0)
        {
            Bored?.Invoke();
            return;
        }

        _confusionTimer -= Time.deltaTime;
    }
}
