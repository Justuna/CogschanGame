using UnityEngine;

/// <summary>
/// A class for holding data about a weapon's recoil. Meant for fast-firing weapons whose recoil follows a pattern over many shots.
/// </summary>
[CreateAssetMenu(fileName = "Continuous RecoilPattern", menuName = "Cogschan/Weapon/Continuous Recoil Pattern")]
public class ContinuousRecoilPattern : ScriptableObject
{
    /// <summary>
    /// The change in the vertical recoil as it ramps up (from 0 to 1).
    /// </summary>
    public AnimationCurve RampUpVerticalRecoil;
    /// <summary>
    /// The change in the horizontal recoil as it ramps up (from 0 to 1).
    /// </summary>
    public AnimationCurve RampUpHorizontalRecoil;
    /// <summary>
    /// How long after starting the gun takes to begin looping.
    /// </summary>
    public float RampUpDuration;
    /// <summary>
    /// The change in the vertical recoil as it loops (from 0 to 1).
    /// </summary>
    /// /// <remarks>
    /// This curve should be set to either loop or ping pong.
    /// </remarks>
    public AnimationCurve LoopVerticalRecoil;
    /// <summary>
    /// The change in the horizontal recoil as it loops (from 0 to 1).
    /// </summary>
    /// /// /// <remarks>
    /// This curve should be set to either loop or ping pong.
    /// </remarks>
    public AnimationCurve LoopHorizontalRecoil;
    /// <summary>
    /// How fast the loop should play out.
    /// </summary>
    public float LoopTimeScale;
    /// <summary>
    /// The function which defines how the camera will return to the initial position over time. The curve should end at (1, 0).
    /// </summary>
    /// <remarks>
    /// In case the loop pattern varies wildly, we cannot guarantee that the loop pattern will stop in the same place.
    /// So instead of using a specific pattern, use a curve to define how to transition between the last position and the original position.
    /// </remarks>
    public AnimationCurve RampDownFunction;
    /// <summary>
    /// How long after stopping the gun takes to return to the default position.
    /// </summary>
    public float RampDownDuration;
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
    public float VerticalNoiseAmplitude;
    public float HorizontalNoiseAmplitude;
    public float VerticalNoiseScale;
    public float HorizontalNoiseScale;
}