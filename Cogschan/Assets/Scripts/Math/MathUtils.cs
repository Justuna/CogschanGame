using UnityEngine;

/// <summary>
/// Collection of various math utility methods
/// </summary>
public static class MathUtils
{
    /// <summary>
    /// Interpolates a value between <paramref name="start"/> and <paramref name="end"/> within the wrapped range spanning <paramref name="rangeStart"/> and <paramref name="rangeEnd"/>.
    /// This will always take the shortest path within the wrapped range.
    /// </summary>
    /// <param name="start"></param>
    /// <param name="end"></param>
    /// <param name="t"></param>
    /// <param name="rangeStart"></param>
    /// <param name="rangeEnd"></param>
    /// <returns></returns>
    public static float LerpRepeat(float start, float end, float t, float rangeStart = 0, float rangeEnd = 1)
    {
        float rangeSpan = rangeEnd - rangeStart;
        float delta = Mathf.Repeat(end - start, rangeSpan);
        if (delta > rangeSpan / 2f)
            delta -= rangeSpan;
        float lerpedValue = start + delta * Mathf.Clamp01(t);
        if (lerpedValue < rangeStart)
            lerpedValue = rangeEnd - lerpedValue;
        return lerpedValue;
    }
}
