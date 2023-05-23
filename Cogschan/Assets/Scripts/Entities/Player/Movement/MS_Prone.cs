using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MS_Prone : MonoBehaviour, IMovementState
{
    private float _timer;

    public CogschanSimpleEvent ProneEnded;

    public void Initialize(float duration)
    {
        _timer = duration;
    }

    public void Behavior()
    {
        if (_timer > 0 )
        {
            _timer -= Time.deltaTime;
        }
        else
        {
            ProneEnded.Invoke();
        }
    }

    public float GetBaseSpeed()
    {
        return 0;
    }

    public void OnDash()
    {
        // Do nothing
    }

    public void OnJump()
    {
        // Do nothing
    }

    public void OnProne(float duration)
    {
        Initialize(duration);
    }
}
