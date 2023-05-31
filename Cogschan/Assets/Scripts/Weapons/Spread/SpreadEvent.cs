﻿using UnityEngine;

/// <summary>
/// A class that iterates over a <see cref="SpreadPattern"/> over time.
/// </summary>
public class SpreadEvent
{
    private SpreadPattern _spreadPattern;
    private float _timer = 0;

    public SpreadEvent(SpreadPattern spreadPattern)
    {
        _spreadPattern = spreadPattern;
    }

    /// <summary>
    /// Returns a random spread angle based on the pattern and the time elapsed. 
    /// </summary>
    public Vector2 GetSpread()
    {
        float t = Mathf.Clamp01(_timer / _spreadPattern.Duration);
        float magnitude = _spreadPattern.Spread.Evaluate(t);
        float angle = Random.Range(0, 2 * Mathf.PI);
        return new Vector2(magnitude * Mathf.Sin(angle), magnitude * Mathf.Cos(angle));
    }

    /// <summary>
    /// Progresses time on the spread event.
    /// </summary>
    /// <returns> Returns true if the spread event has finished, and false otherwise. </returns>
    public bool StepTime()
    {
        _timer += Time.deltaTime;
        if (_timer >= _spreadPattern.Duration)
        {
            return true;
        }
        return false;
    }

    /// <summary>
    /// Applies the <paramref name="spread"/> to the <paramref name="vector"/>.
    /// </summary>
    /// <param name="vector">The vector the spread will be applied to.</param>
    /// <param name="spread">The spread to be applied.</param>
    public static void ApplySpread(ref Vector3 vector, Vector2 spread)
    {
        Quaternion toForward = Quaternion.FromToRotation(vector, Vector3.forward);
        vector = Quaternion.Inverse(toForward) * Quaternion.Euler(spread)
            * toForward * vector;
    }
}