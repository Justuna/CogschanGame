using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponPickup : Interactable
{
    [Tooltip("Whether or not this weapon should be placed immediately in hand on pickup.")]
    [SerializeField] private bool _activeImmediately;
    [Tooltip("The prefab for the weapon.")]
    [SerializeField] private GameObject _weaponPrefab;

    // Just in case the Unity garbage collector does not delete this immediately.
    private bool _deleted;

    protected override void InteractInternal(EntityServiceLocator services)
    {
        if (_deleted) return;

        WeaponCache weaponCache = services.WeaponCache;
        if (weaponCache != null)
        {
            if (weaponCache.AddWeapon(_weaponPrefab))
            {
                _deleted = true;
                Destroy(gameObject);
            }
        }
    }
}
