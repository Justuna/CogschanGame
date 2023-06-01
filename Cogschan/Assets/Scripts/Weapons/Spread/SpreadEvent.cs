using UnityEngine;

/// <summary>
/// A class that iterates over a <see cref="SpreadPattern"/> over time.
/// </summary>
public class SpreadEvent
{
    private SpreadPattern _spreadPattern;
    private float _counter = 0;

    public SpreadEvent(SpreadPattern spreadPattern)
    {
        _spreadPattern = spreadPattern;
    }

    /// <summary>
    /// Returns a random spread angle based on the pattern and the time elapsed. 
    /// </summary>
    public Vector2 GetSpread()
    {
        float t = _counter / _spreadPattern.Duration;
        float magnitude = _spreadPattern.Magnitude * _spreadPattern.Spread.Evaluate(t);
        float angle = Random.Range(0, 2 * Mathf.PI);
        return new(magnitude * Mathf.Sin(angle), magnitude * Mathf.Cos(angle));
    }

    public void IncrementSpread()
    {
        _counter = Mathf.Min(_spreadPattern.Duration, _counter + _spreadPattern.Increase);
    }

    /// <summary>
    /// Progresses time on the spread event.
    /// </summary>
    public void StepTime()
    {
        _counter -= Time.deltaTime;
        _counter = Mathf.Clamp(_counter, 0, _spreadPattern.Duration);
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