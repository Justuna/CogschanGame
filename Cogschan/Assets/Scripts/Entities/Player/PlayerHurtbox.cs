using UnityEngine;

public class PlayerHurtbox : MonoBehaviour, IHurtbox
{
    [SerializeField] private Collider _collider;
    [SerializeField] private CogschanKinematicPhysics _movementHandler;
    [SerializeField] private PlayerMovementController _movementController;

    public void AddImpulse(Vector3 impulse, bool cancelOverride, float maintainMomentum)
    {
        _movementHandler.AddImpulse(impulse, cancelOverride, maintainMomentum);
    }

    public void ChangeHealth(float amount)
    {
        throw new System.NotImplementedException();
    }
}
