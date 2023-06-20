using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// A class that iterates through a <c>RecoilPattern</c> over time.
/// </summary>
public class SimpleRecoilEvent : IRecoilEvent
{
    private SimpleRecoilPattern _recoilPattern;
    private float _timer = 0;

    public SimpleRecoilEvent(SimpleRecoilPattern recoilPattern)
    {
        _recoilPattern = recoilPattern;
    }

    public Vector2 GetRecoil()
    {
        float t = Mathf.Clamp01(_timer / _recoilPattern.Duration);
        float x = _recoilPattern.HorizontalRecoil.Evaluate(t) * _recoilPattern.HorizontalPower;
        float y = _recoilPattern.VerticalRecoil.Evaluate(t) * _recoilPattern.VerticalPower;
        return new Vector2(x, y);
    }

    public bool StepTime()
    {
        _timer += Time.deltaTime;
        if (_timer >= _recoilPattern.Duration)
        {
            return true;
        }
        return false;
    }
}
