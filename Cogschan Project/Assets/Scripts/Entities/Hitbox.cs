using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hitbox : MonoBehaviour
{
    public float Multiplier = 1f;

    private Entity _entity;

    private void Awake()
    {
        _entity = GetComponentInParent<Entity>();
    }

    public void TakeHit(float damage)
    {
        if (_entity != null) _entity.DealDamage(Multiplier * damage);
    }
}
