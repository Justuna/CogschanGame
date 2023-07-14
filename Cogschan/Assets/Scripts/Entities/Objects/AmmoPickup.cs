using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

public class AmmoPickup : Interactable
{
    [Tooltip("The EntityServiceLocator for this entity.")]
    [SerializeField] private EntityServiceLocator _services;
    [Tooltip("The number of clips of ammo this pickup has.")]
    [SerializeField] private int _clipCount;
    [Tooltip("The type of ammo this pickup contains.")]
    [SerializeField] private AmmoType _ammoType;

    // Just in case the Unity garbage collector does not delete this immediately.
    private bool _deleted;

    protected override void InteractInternal(EntityServiceLocator services)
    {
        if (_deleted) return;

        WeaponCache weaponCache = services.WeaponCache;
        if (weaponCache != null )
        {
            if (weaponCache.AddAmmo(_clipCount, _ammoType)) {
                _deleted = true;
                Destroy(_services.gameObject);
            }
        }
    }
}
