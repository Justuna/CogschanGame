using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ES_Waiting : MonoBehaviour, EnemyState
{
    public float WaitRange;

    public LOSCalculator LOS;

    public event Action OutOfRange;

    public void Behavior()
    {
        if (LOS.CanSee || Vector3.Distance(transform.position, LOS.LastSeenPosition) > WaitRange)
        {
            OutOfRange?.Invoke();
        }
    }
}
