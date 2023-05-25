using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ContinuousRecoilEvent : IRecoilEvent
{
    enum RecoilState { RAMPUP, LOOPING, RAMPDOWN }

    private RecoilState _state;
    private ContinuousRecoilPattern _recoilPattern;
    private Func<bool> _endCondition;
    private float _timer;
    private Vector2 _lastRealRecoilPosition;

    public ContinuousRecoilEvent(ContinuousRecoilPattern recoilPattern, Func<bool> endCondition)
    {
        _state = RecoilState.RAMPUP;
        _recoilPattern = recoilPattern;
        _endCondition = endCondition;
    }

    public Vector2 GetRecoil()
    {
        // When ramping up, get t [0, 1] and apply the recoil.
        if (_state == RecoilState.RAMPUP)
        {
            float t = Mathf.Clamp01(_timer / _recoilPattern.RampUpDuration);
            float x = _recoilPattern.RampUpHorizontalRecoil.Evaluate(t) * _recoilPattern.HorizontalPower +
                Mathf.PerlinNoise(t * _recoilPattern.HorizontalNoiseScale, 0) * _recoilPattern.HorizontalNoiseAmplitude;
            float y = _recoilPattern.RampUpVerticalRecoil.Evaluate(t) * _recoilPattern.VerticalPower +
                Mathf.PerlinNoise(t * _recoilPattern.VerticalNoiseScale, 0) * _recoilPattern.VerticalNoiseAmplitude;
            _lastRealRecoilPosition = new Vector2(x, y);
            return _lastRealRecoilPosition;
        }
        // When looping, get t [0, inf] and apply recoil.
        else if (_state == RecoilState.LOOPING)
        {
            float t = _timer * _recoilPattern.LoopTimeScale;
            float x = _recoilPattern.LoopHorizontalRecoil.Evaluate(t) * _recoilPattern.HorizontalPower +
                Mathf.PerlinNoise(t * _recoilPattern.HorizontalNoiseScale, 0) * _recoilPattern.HorizontalNoiseAmplitude;
            float y = _recoilPattern.LoopVerticalRecoil.Evaluate(t) * _recoilPattern.VerticalPower +
                Mathf.PerlinNoise(t * _recoilPattern.VerticalNoiseScale, 0) * _recoilPattern.VerticalNoiseAmplitude;
            _lastRealRecoilPosition = new Vector2(x, y);
            return _lastRealRecoilPosition;
        }
        // When ramping down, get t [0, 1] and lerp between the last calculated recoil amount and 0.
        else
        {
            float t = Mathf.Clamp01(_timer / _recoilPattern.RampDownDuration);
            float x = _recoilPattern.RampDownFunction.Evaluate(t) * _lastRealRecoilPosition.x;
            float y = _recoilPattern.RampDownFunction.Evaluate(t) * _lastRealRecoilPosition.y;
            return new Vector2 (x, y);
        }
    }

    public bool StepTime()
    {
        _timer += Time.deltaTime;

        // If the rampup has finished, begin looping.
        if (_state == RecoilState.RAMPUP && _timer >= _recoilPattern.RampUpDuration)
        {
            _state = RecoilState.LOOPING;
            _timer = 0;
            return false;
        }

        // If the gun has stopped firing (or some other end condition), begin ramping down.
        if (_state != RecoilState.RAMPDOWN && _endCondition())
        {
            _timer = 0;
            _state = RecoilState.RAMPDOWN;
            return false;
        }

        // If the rampdown has finished, stop.
        if (_state == RecoilState.RAMPDOWN && _timer >= _recoilPattern.RampDownDuration)
        {
            return true;
        }

        return false;
    }
}
