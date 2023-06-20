using System;
using UnityEngine;

public class ConstantVelocityOverride : IVelocityOverride
{
    private Vector3 _velocity;
    private Func<bool> _whenFinished;
    private float _momentumFactor;

    public ConstantVelocityOverride(Vector3 velocity, Func<bool> whenFinished, float momentumFactor)
    {
        _velocity = velocity;
        _whenFinished = whenFinished;
        _momentumFactor = momentumFactor;
    }

    public Vector3 GetVelocity()
    {
        return _velocity;
    }

    public bool IsFinished()
    {
        return _whenFinished();
    }

    public float MaintainMomentumFactor()
    {
        return _momentumFactor;
    }
}