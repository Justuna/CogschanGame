using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// A simple, custom delegate type that corresponds to functions with no parameters or return values.
/// </summary>
/// <remarks>
/// Although Unity provides UnityEvents which do essentially the same thing, UnityEvents can have a lot
/// of overhead involved. Delegates like these are faster, although you have to make them yourself, and
/// they don't show up in the inspector like UnityEvents do.
/// </remarks>
public delegate void CogschanSimpleEvent();
