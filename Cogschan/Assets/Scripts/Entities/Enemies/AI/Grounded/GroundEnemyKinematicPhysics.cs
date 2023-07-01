using System.Collections;
using UnityEngine;

public class GroundEnemyKinematicPhysics : KinematicPhysics
{
    [SerializeField] private float _minimumTimeOffGround;

    private float _groundTimer;

    private void Update()
    {
        _groundTimer -= Time.deltaTime;
    }

    protected override Vector3 PickVelocity()
    {
        Vector3 actualVelocity;

        // Grounded movement just uses whatever the agent came up with
        // Turn on agent, should be grounded anyways
        if (_services.GroundChecker.IsGrounded && _groundTimer <= 0 && _impulses.Count == 0)
        {
            _services.NavMeshAgent.enabled = true;

            actualVelocity = DesiredVelocity;
        }
        // Either you're being knocked into the air, or you already were in the air
        // Use previous velocity to calculate momentum, and add new impulses
        // Turn off agent for a minimum amount of time to avoid snapping to floor and cancelling impulse
        else
        {
            if (_impulses.Count > 0)
            {
                _services.NavMeshAgent.enabled = false;
                _groundTimer = _minimumTimeOffGround;

                while (_impulses.Count > 0)
                {
                    _previousVelocity += _impulses.Dequeue() / _mass;
                }
            }

            actualVelocity = ApplyForces(_previousVelocity);
        }

        return actualVelocity;
    }

    protected override void MakeMove(Vector3 actualVelocity)
    {
        if (_services.NavMeshAgent.enabled) _services.NavMeshAgent.Move(actualVelocity * Time.deltaTime);
        else _services.CharacterController.Move(actualVelocity * Time.deltaTime);
    }
}
