using UnityEngine;

// <summary>
/// A class for holding data about a weapon's spread. 
/// </summary>
[CreateAssetMenu(fileName = "SpreadPattern", menuName = "Cogschan/Weapon/Spread Pattern")]
public class SpreadPattern : ScriptableObject
{
    /// <summary>
    /// The change in spread over time/shots (from 0 to 1).
    /// </summary>
    public AnimationCurve Spread;
    /// <summary>
    /// How long it takes for the spread event to reset.
    /// </summary>
    public float Duration;
    /// <summary>
    /// How much the "time" increases after one shot.
    /// </summary>
    public float Increase;
    /// <summary>
    /// How much the spread amount should be multiplied by.
    /// </summary>
    public float Magnitude;
}

