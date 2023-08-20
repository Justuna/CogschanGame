using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using static Codice.Client.BaseCommands.Import.Commit;

public class AmmoPickup : Pickup
{
    [Tooltip("The number of clips of ammo this pickup has.")]
    [SerializeField] private int _clipCount;
    [Tooltip("The type of ammo this pickup contains.")]
    [SerializeField] private AmmoType _ammoType;

    protected override bool PickupAction(EntityServiceLocator services)
    {
        WeaponCache weaponCache = services.WeaponCache;
        if (weaponCache != null)
        {
            return weaponCache.AddAmmo(_clipCount, _ammoType);
        }

        return false;
    }
}