using UnityEngine;

public class PickupKinematicPhysics : KinematicPhysics
{
    protected override Vector3 PickVelocity()
    {
        if (_services.GroundChecker.IsGrounded && _impulses.Count == 0)
        {
            return Vector3.zero;
        }
        else
        {
            while (_impulses.Count > 0)
            {
                _previousVelocity += _impulses.Dequeue() / _mass;
            }

            return ApplyForces(_previousVelocity);
        }
    }

    protected override void MakeMove(Vector3 actualVelocity)
    {
        _services.CharacterController.Move(actualVelocity * Time.deltaTime);
    }
}
