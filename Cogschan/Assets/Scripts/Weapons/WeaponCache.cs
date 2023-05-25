using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// A script dedicated to retrieving the current weapon, and keeping track of all held weapons.
/// </summary>
public class WeaponCache : MonoBehaviour
{
    [SerializeField] private PlayerServiceLocator _services;
    [SerializeField] private GameObject _defaultWeaponPrefab;
    [SerializeField] private Transform _weaponParent;
    [SerializeField] private int _maxSize = 9;

    private List<IWeapon> _cache = new List<IWeapon>();
    private int _currentWeaponIndex;

    public IWeapon CurrentWeapon { get { return _cache[_currentWeaponIndex]; } }

    private void Start()
    {
        AddWeapon(_defaultWeaponPrefab);
    }

    /// <summary>
    /// Adds a weapon to Cogschan's inventory.
    /// </summary>
    /// <param name="weaponPrefab">The prefab that the weapon will be cloned from.</param>
    /// <param name="activeImmediately">Whether or not the weapon should immediately be switched to upon pickup.</param>
    /// <returns>
    /// If successfully added, returns <c>true</c>. Returns <c>false</c> otherwise.
    /// </returns>
    public bool AddWeapon(GameObject weaponPrefab, bool activeImmediately = true)
    {
        if (_cache.Count == _maxSize) return false;

        GameObject weaponObject = Instantiate(weaponPrefab, _weaponParent);
        IWeapon weapon = weaponObject.GetComponent<IWeapon>();
        if (weapon != null)
        {
            weapon.Init(_services);

            _cache.Add(weapon);
            if (activeImmediately)
            {
                PickWeapon(_cache.Count - 1);
            }

            return true;
        }
        else
        {
            Destroy(weaponObject);
            return false;
        }
    }

    /// <summary>
    /// Selects the next weapon and loads it. If using the last weapon, circles back around to the first.
    /// </summary>
    public void NextWeapon()
    {
        PickWeapon((_currentWeaponIndex + 1) % _cache.Count);
    }

    /// <summary>
    /// Selects the previous weapon and loads it. If using the first weapon, circles around to the last.
    /// </summary>
    public void PrevWeapon()
    {
        PickWeapon((_currentWeaponIndex - 1) % _cache.Count);
    }

    /// <summary>
    /// Checks if the weapon cache contains a weapon of a certain type/name.
    /// </summary>
    /// <param name="name">The name that uniquely identifies a weapon.</param>
    /// <returns>Returns <c>true</c> if a weapon matching <c>name</c> exists in the cache, and <c>false</c> otherwise.</returns>
    public bool HasWeapon(string name)
    {
        return _cache.Exists(weapon => weapon.GetName() == name);
    }

    /// <summary>
    /// Selects a weapon by index and loads it.
    /// </summary>
    /// <param name="index">The index of the weapon to switch to.</param>
    private void PickWeapon(int index)
    {
        CurrentWeapon.GetGameObject().SetActive(false);

        _currentWeaponIndex = index;
        CurrentWeapon.GetGameObject().SetActive(true);
    }
}
