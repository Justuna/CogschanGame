using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AmmoPickup : MonoBehaviour
{
    public AmmoType Type;
    public int Amount;

    Collider _collider;

    private void Awake()
    {
        _collider = GetComponent<Collider>();
    }

    private void OnTriggerStay(Collider other)
    {
        WeaponCache cache = other.GetComponentInParent<WeaponCache>();
        if (cache != null)
        {
            if (cache.AddAmmo(Type, Amount)) Destroy(gameObject);
        }
    }
}
