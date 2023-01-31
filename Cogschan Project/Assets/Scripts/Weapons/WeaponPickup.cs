using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponPickup : MonoBehaviour
{
    public GameObject GunPrefab;

    Collider _collider;

    private void Awake()
    {
        _collider = GetComponent<Collider>();
    }

    private void OnTriggerEnter(Collider other)
    {
        WeaponCache cache = other.GetComponentInParent<WeaponCache>();
        if (cache != null)
        {
            cache.AddGun(GunPrefab, true);
        }

        Destroy(gameObject);
    }
}
