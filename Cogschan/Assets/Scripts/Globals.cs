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

#endregion