using DG.Tweening;
using UnityEngine;

/// <summary>
/// Collection of varrious DOTween utility methods
/// </summary>
public static class DOTweenUtils
{
    public delegate void DOCustomEvalDelegate(float time);

    /// <summary>
    /// Evaluates the <paramref name="evalFunc"/> from 0-1 over the course of <paramref name="duration"/>
    /// </summary>
    /// <param name="evalFunc">Function that takes in the current time of the tween, represented by a float from 0-1</param>
    /// <param name="duration">Duration of the tween</param>
    /// <returns></returns>
    public static Tween DOCustom(DOCustomEvalDelegate evalFunc, float duration)
    {
        return DOTween.To(() => 0f, (t) => evalFunc(t), 1f, duration);
    }

    /// <summary>
    /// Tweens a float from start to end. The float must existing within a range 
    /// defined by <paramref name="rangeStart"/> and <paramref name="rangeEnd"/>,
    /// and the float can wrap around. This tween function always chooses the shortest path between start and end.
    /// </summary>
    /// <param name="start">Starting value</param>
    /// <param name="end">Ending value</param>
    /// <param name="callback">Callback to use the tweened value</param>
    /// <param name="duration">Duration of the tween</param>
    /// <param name="rangeStart">Start of the wrapped range</param>
    /// <param name="rangeEnd">End of the wrapped range</param>
    /// <returns></returns>
    public static Tween DORepeatFloat(float start, float end, TweenCallback<float> callback, float duration, float rangeStart = 0, float rangeEnd = 1)
    {
        return DOCustom((t) =>
        {
            callback(MathUtils.LerpRepeat(start, end, t, rangeStart, rangeEnd));
        }, duration);
    }

    /// <summary>
    /// Tweens an angle, making sure it wraps around 260 degrees correctly.
    /// </summary>
    /// <param name="start">Starting angle in degrees</param>
    /// <param name="end">Ending angle in degrees</param>
    /// <param name="callback">Callback to use the tweened angle</param>
    /// <param name="duration">Duration of the tween</param>
    /// <returns></returns>
    public static Tween DOAngle(float start, float end, TweenCallback<float> callback, float duration)
    {
        return DOCustom((t) =>
        {
            callback(Mathf.LerpAngle(start, end, t));
        }, duration);
    }
}
