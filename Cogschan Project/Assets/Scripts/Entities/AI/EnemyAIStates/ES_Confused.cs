using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ES_Confused : MonoBehaviour, EnemyState
{
    public float MinConfusionTime, MaxConfusionTime;
    public LOSCalculator LOS;

    public event Action LOSMade, Bored;

    private float _confusionTimer = 0;

    public void Behavior()
    {
        if (LOS.CanSee)
        {
            LOSMade?.Invoke();
            return;
        }

        if (_confusionTimer <= 0)
        {
            Bored?.Invoke();
            return;
        }

        _confusionTimer -= Time.deltaTime;
    }

    public void ResetTimer()
    {
        _confusionTimer = UnityEngine.Random.Range(MinConfusionTime, MaxConfusionTime);
    }
}
