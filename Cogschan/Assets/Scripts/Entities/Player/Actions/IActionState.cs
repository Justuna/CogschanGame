using System;
/// <summary>
/// An interface that defines a state to be used by the action state machine. Provides additional hooks for handling the reloading and weapon-switching inputs.
/// </summary>
public interface IActionState : Globals
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