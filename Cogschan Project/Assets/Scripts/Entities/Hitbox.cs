using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hitbox : MonoBehaviour
{
    public Entity Entity;
    public float Multiplier = 1f;

    public void TakeHit(float damage)
    {
        Entity.DealDamage(Multiplier * damage);
    }
}
