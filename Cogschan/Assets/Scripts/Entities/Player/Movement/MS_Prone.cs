using UnityEngine;

public class MS_Prone : MonoBehaviour, IMovementState, IMachineStateBehave
{
    private float _timer;

    public CogschanSimpleEvent ProneEnded;

    public void Initialize(float duration)
    {
        _timer = duration;
    }

    public void OnBehave()
    {
        if (_timer > 0)
        {
            _timer -= Time.deltaTime;
        }
        else
        {
            ProneEnded?.Invoke();
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
