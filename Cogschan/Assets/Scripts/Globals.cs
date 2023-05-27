using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#region Delegates

/// <summary>
/// A simple, custom delegate type that corresponds to functions with no parameters or return values.
/// </summary>
/// <remarks>
/// Although Unity provides UnityEvents which do essentially the same thing, UnityEvents can have a lot
/// of overhead involved. Delegates like these are faster, although you have to make them yourself, and
/// they don't show up in the inspector like UnityEvents do.
/// </remarks>
public delegate void CogschanSimpleEvent();
/// <summary>
/// A simple, custom delegate type that corresponds to functions with a float parameter and no return values.
/// </summary>
/// <param name="input"></param>
public delegate void CogschanFloatEvent(float input);
/// <summary>
/// A custom delegate type that corresponds to functions with an anonymous boolean function as a parameter and no return values.
/// </summary>
/// <param name="condition"></param>
public delegate void CogschanConditionEvent(Func<bool> condition);

#endregion

#region Interfaces

#region State Machine Interfaces

/// <summary>
/// An interface that defines a basic state to be used by a state machine. Provides a <c>Behaviour</c> function, meant to be called every update while
/// the current state is active.
/// </summary>
public interface IMachineState
{
    /// <summary>
    /// What this state should do every frame.
    /// </summary>
    public void Behavior();
}

/// <summary>
/// An interface that defines a state to be used by the movement state machine. Provides additional hooks for handling 
/// dashing/jumping inputs, being knocked prone, and fetching a base speed value.
/// </summary>
public interface IMovementState : IMachineState
{
    /// <summary>
    /// What this state should do in response to the dash button.
    /// </summary>
    public void OnDash();

    /// <summary>
    /// What this state should do in response to the jump button.
    /// </summary>
    public void OnJump();

    /// <summary>
    /// What this state should do in response to being knocked prone.
    /// </summary>
    /// <param name="duration">
    /// How long the caller would like to knock Cogschan prone for. <para></para>
    /// <remarks>
    /// Whether or not the prone state is actually triggered, or whether or not it actually lasts <c>duration</c> seconds, depends on the state.
    /// Typically, though, the state will comply by triggering the prone state for <c>duration</c> seconds.
    /// </remarks>
    /// </param>
    public void OnProne(float duration);

    /// <summary>
    /// Get the base travel speed of this state.
    /// </summary>
    /// <returns>
    /// Returns the base travel speed of this state.
    /// </returns>
    public float GetBaseSpeed();
}

/// <summary>
/// An interface that defines a state to be used by the action state machine. Provides additional hooks for handling the reloading and weapon-switching inputs.
/// </summary>
public interface IActionState : IMachineState
{
    /// <summary>
    /// What this state should do in response to the reload button.
    /// </summary>
    public void OnReload();

    /// <summary>
    /// What this state should do in response to an attempt to switch to the next weapon.
    /// </summary>
    public void OnNextWeapon();

    /// <summary>
    /// What this state should do in response to an attempt to switch to the previous weapon.
    /// </summary>
    public void OnPrevWeapon();

    /// <summary>
    /// What this state should do in response to an attempt to lock actions.
    /// </summary>
    public void OnLock(Func<bool> unlockCondition);
}

/// <summary>
/// An interface that defines a behavioral state to be used by enemy AI.
/// </summary>
public interface IEnemyState : IMachineState
{
    // Nothing unique yet...
}

#endregion

#region Weapon Interfaces

/// <summary>
/// An interface providing functions that a weapon would need.
/// </summary>
public interface IWeapon
{
    /// <summary>
    /// An initialization method to pass to the weapon all of the dependencies it might have through the <c>PlayerServiceLocator</c>.
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
}

/// <summary>
/// Empty class to allow recoil patterns to be held onto in the same field, since they otherwise have little to nothing in common.
/// </summary>
public abstract class RecoilPattern : ScriptableObject { }

/// <summary>
/// Interface to represent an object that holds onto and progresses a recoil pattern.
/// </summary>
public interface IRecoilEvent
{
    /// <summary>
    /// Gets the recoil at the current time.
    /// </summary>
    /// <remarks>
    /// Should be called together with Iterate().
    /// </remarks>
    /// <returns>
    /// Returns the recoil at the current time.
    /// </returns>
    Vector2 GetRecoil();

    /// <summary>
    /// Progresses time on the recoil event.
    /// </summary>
    /// <returns>
    /// Returns true if the recoil event has finished, and false otherwise.
    /// </returns>
    bool StepTime();
}

#endregion

/// <summary>
/// Interface for retrieving information from any script that would override a character's velocity.
/// </summary>
public interface IVelocityOverride
{
    /// <summary>
    /// The velocity of the override.
    /// </summary>
    /// <returns>
    /// Returns the velocity of the override.
    /// </returns>
    public Vector3 GetVelocity();

    /// <summary>
    /// Whether or not the velocity override can be removed.
    /// </summary>
    /// <returns>
    /// True if the velocity override is done.
    /// False if the velocity override is still operating.
    /// </returns>
    public bool IsFinished();

    /// <summary>
    /// How much of the velocity is maintained as momentum after the velocity override ends.
    /// </summary>
    /// <returns>
    /// Returns a float representing the factor of velocity that should be maintained as momentum.
    /// </returns>
    public float MaintainMomentumFactor();
}

#endregion