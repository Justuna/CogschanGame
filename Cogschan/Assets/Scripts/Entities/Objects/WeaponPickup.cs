using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponPickup : Pickup
{
    [Tooltip("Whether or not this weapon should be placed immediately in hand on pickup.")]
    [SerializeField] private bool _activeImmediately;
    [Tooltip("The prefab for the weapon.")]
    [SerializeField] private GameObject _weaponPrefab;

    protected override bool PickupAction(EntityServiceLocator services)
    {
        WeaponCache weaponCache = services.WeaponCache;
        if (weaponCache != null)
        {
            return weaponCache.AddWeapon(_weaponPrefab);
        }

        return false;
    }
}
