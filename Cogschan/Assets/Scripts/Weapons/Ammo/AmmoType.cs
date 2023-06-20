using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName="NewAmmoType", menuName="Cogschan/Weapon/AmmoType")]
public class AmmoType : ScriptableObject, IEquatable<AmmoType>
{
    public string Name;

    public bool Equals(AmmoType other)
    {
        return other.Name == Name;
    }
}