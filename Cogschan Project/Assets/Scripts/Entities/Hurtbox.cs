using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Hurtbox : MonoBehaviour
{
    public float Multiplier = 1f;
    public UnityEvent<float> OnHit = new UnityEvent<float>();

    public void TakeHit(float damage)
    {
        OnHit.Invoke(Multiplier * damage);
    }
}
