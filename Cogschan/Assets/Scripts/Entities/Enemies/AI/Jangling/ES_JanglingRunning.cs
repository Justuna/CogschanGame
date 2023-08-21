using UnityEngine;

public class ES_JanglingRunning : MonoBehaviour, IJanglingState
{
    public CogschanSimpleEvent RunningToIdle;
    public CogschanSimpleEvent RunningToWatching;
    public CogschanSimpleEvent RunningToStunned;

    [SerializeField] private EntityServiceLocator _services;
    [SerializeField] private float _speed;
    [SerializeField] private float _turnSpeed;

    public void OnBehave()
    {
        Vector3 dest = _services.PointGraph.GetNode(_services.JanglingAI.DestinationNode.Value);
        Vector3 curr = _services.Jangling.position;

        Vector3 dir = (dest - curr).normalized;

        // Want to make sure it doesn't overshoot the goal
        float delta = Vector3.Distance(dest, curr);

        // If it would overshoot, just move it by the actual distance remaining (in the direction of movement)
        // Also call the event that stops it moving
        if (delta <= _speed * Time.deltaTime)
        {
            _services.CharacterController.Move(dir * delta);
            if (_services.LOSChecker.CanSee) RunningToWatching?.Invoke();
            else RunningToIdle?.Invoke();
        }
        else
        {
            Vector3 velocity = dir * _speed;

            _services.Model.transform.rotation = Quaternion.Lerp(_services.Model.transform.rotation, Quaternion.LookRotation(dir), _turnSpeed * Time.deltaTime);

            _services.CharacterController.Move(velocity * Time.deltaTime);
        }
    }

    public void OnStun()
    {
        RunningToStunned.Invoke();
    }

    public void OnDamaged()
    {
        // Do nothing, already running
    }
}
