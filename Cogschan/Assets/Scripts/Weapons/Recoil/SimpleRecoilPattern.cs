using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// A class for holding data about a weapon's recoil. Meant for simple, one-off recoil effects.
/// </summary>
[CreateAssetMenu(fileName="SimpleRecoilPattern", menuName ="Cogschan/Weapon/Simple Recoil Pattern")]
public class SimpleRecoilPattern : RecoilPattern
{
    /// <summary>
    /// The change in the vertical recoil over time (from 0 to 1).
    /// </summary>
    public AnimationCurve VerticalRecoil;
    /// <summary>
    /// The change in the horizontal recoil over time (from 0 to 1).
    /// </summary>
    public AnimationCurve HorizontalRecoil;
    /// <summary>
    /// How long it takes for the recoil event to progress from start to finish (0 to 1).
    /// </summary>
    public float Duration;
    /// <summary>
    /// How much the vertical recoil amount should be multiplied by.
    /// </summary>
    /// <remarks>
    /// Although you can set the strength of the recoil in the animation curves, this lets you do quick tweaks.
    /// </remarks>
    public float VerticalPower;
    /// <summary>
    /// How much the horizontal recoil amount should be multiplied by.
    /// </summary>
    /// <remarks>
    /// Although you can set the strength of the recoil in the animation curves, this lets you do quick tweaks.
    /// </remarks>
    public float HorizontalPower;
}
