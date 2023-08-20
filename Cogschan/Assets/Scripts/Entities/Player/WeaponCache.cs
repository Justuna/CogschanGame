using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// A script dedicated to retrieving the current weapon, and keeping track of all held weapons.
/// </summary>
public class WeaponCache : MonoBehaviour
{
    [field: SerializeField]
    public UnityEvent<IWeapon> WeaponChanged { get; private set; }

    [SerializeField] private EntityServiceLocator _services;
    [SerializeField] private GameObject[] _defaultWeaponPrefabs;
    [SerializeField] private Transform _weaponParent;
    [SerializeField] private int _maxSize = 9;

    private List<IWeapon> _cache = new List<IWeapon>();
    [SerializeField]
    private int _currentWeaponIndex;

    public IWeapon CurrentWeapon { get { return _cache[_currentWeaponIndex]; } }

    private void Start()
    {
        foreach (GameObject prefab in _defaultWeaponPrefabs)
        {
            AddWeapon(prefab, false);
        }
        PickWeapon(0);
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

        GameObject weaponObject = Instantiate(weaponPrefab);
        weaponObject.transform.parent = _weaponParent;
        weaponObject.transform.localPosition = Vector3.zero;
        IWeapon weapon = weaponObject.GetComponent<IWeapon>();
        if (weapon != null)
        {
            weapon.Init(_services);

            _cache.Add(weapon);
            if (activeImmediately)
            {
                PickWeapon(_cache.Count - 1);
            }
            else
            {
                weaponObject.SetActive(false);
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
        if (_cache.Count <= 1) return;
        PickWeapon((_currentWeaponIndex + 1) % _cache.Count);
    }

    /// <summary>
    /// Selects the previous weapon and loads it. If using the first weapon, circles around to the last.
    /// </summary>
    public void PrevWeapon()
    {
        if (_cache.Count <= 1) return;
        PickWeapon((_currentWeaponIndex - 1 + _cache.Count) % _cache.Count);
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
        WeaponChanged.Invoke(CurrentWeapon);
    }

    /// <summary>
    /// Goes through all of the weapons in the cache and attempts to load the ammo into them. 
    /// </summary>
    /// <param name="clips">The number of clips to load.</param>
    /// <param name="type">The type of ammo to load.</param>
    /// <returns>
    /// Returns <c>true</c> if at least one weapon was successfully able to reload at least one clip. 
    /// Returns <c>false</c> if no weapons were able to reload with the supplied ammo.
    /// </returns>
    public bool AddAmmo(int clips, AmmoType type)
    {
        Debug.Log("Attempting to load weapons...");

        int clipsLeft = clips;
        bool didLoad = false;

        foreach (IWeapon weapon in _cache)
        {
            while (weapon.CanLoadClip() && weapon.GetAmmoType() == type && clipsLeft > 0)
            {
                didLoad = true;
                weapon.LoadClip();
                clipsLeft--;
            }
        }

        return didLoad;
    }
}
