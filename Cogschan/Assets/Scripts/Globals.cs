using System;
using System.Collections;
using System.Collections.Generic;

#region Constants
/// <summary>
/// A collection of global constants.
/// </summary>
public static class Constants
{
    /// <summary>
    /// The maximum number of iterations for loops that may otherwise never terminate.
    /// </summary>
    public const int MAX_ITER = 1024;
}
#endregion

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