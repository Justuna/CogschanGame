using System;
using UnityEngine;

/// <summary>
/// An interface providing functions that a weapon would need.
/// </summary>
public interface IWeapon
{
    public event Action AmmoChanged;

    /// <summary>
    /// An initialization method to pass to the weapon all of the dependencies it might have through the <c>EntityServiceLocator</c>.
    /// </summary>
    /// <param name="services"></param>
    public void Init(EntityServiceLocator services);

    /// <summary>
    /// Returns a name that can be used to uniquely identify this weapon.
    /// </summary>
    /// <remarks>
    /// There's no system in place for actually ensuring uniqueness among weapon names, so its up to you to make sure there are no duplicates.
    /// </remarks>
    /// <returns>
    /// Returns the weapon's unique name.
    /// </returns>
    public string GetName();

    /// <returns>
    /// Returns the icon for this weapon
    /// </returns>
    public Sprite GetIcon();

    /// <returns>
    /// Returns the <c>GameObject</c> that the weapon is attached to.
    /// </returns>
    public GameObject GetGameObject();

    /// <summary>
    /// Attempts to use the weapon.
    /// </summary>
    public void Use();

    /// <summary>
    /// Whether or not the weapon is currently active.
    /// </summary>
    /// <returns>
    /// Returns <c>true</c> if the weapon is active. Returns <c>false</c> if the weapon is idle.
    /// </returns>
    public bool InUse();

    /// <summary>
    /// Cancels activity for this weapon if there currently is any.
    /// </summary>
    public void CancelUse();

    /// <summary>
    /// Whether or not the weapon has enough ammo to be used.
    /// </summary>
    /// <returns>
    /// Returns <c>true</c> if the weapon is loaded with enough ammo to be used, or if it does not require ammo to function. Returns <c>false</c> otherwise.
    /// </returns>
    public bool SufficientAmmo();

    /// <summary>
    /// Whether or not can be reloaded using current ammunition.
    /// </summary>
    /// <returns>
    /// Returns <c>true</c> if the weapon can successfully initiate reloading. Returns <c>false</c> if the weapon cannot be reloaded,
    /// either because there is not enough ammunition or because it does not use ammunition.
    /// </returns>
    public bool CanReload();

    /// <returns>
    /// How long the weapon takes to reload.
    /// </returns>
    public float GetReloadTime();

    /// <summary>
    /// Attempts to reload the weapon using current ammunition.
    /// </summary>
    public void Reload();

    /// <summary>
    /// Whether or not the weapon can replenish its ammunition.
    /// </summary>
    /// <returns>
    /// Returns <c>true</c> if the weapon can load the new ammunition. Returns <c>false</c> if the weapon cannot replenish ammunition,
    /// either because it is already has the maximum amount of ammunition or because it does not use ammunition.
    /// </returns>
    public bool CanLoadClip();

    /// <summary>
    /// Attempts to replenish the weapon with ammunition.
    /// </summary>
    public void LoadClip();

    /// <returns>
    /// Returns the type of ammunition that this weapon needs to be loaded with. If the weapon does not require ammo, returns <c>null</c> instead.
    /// </returns>
    public AmmoType GetAmmoType();

    /// <returns>
    /// Return the amount of ammunition currently loaded in the weapon. If the weapon does not require ammo, returns <c>null</c> instead.
    /// </returns>
    public int? GetLoadedAmmoCount();

    /// <returns>
    /// Return the amount of ammunition currently kept in reserve for this weapon. If the weapon does not require ammo, returns <c>null</c> instead.
    /// </returns>
    public int? GetReserveAmmoCount();
}