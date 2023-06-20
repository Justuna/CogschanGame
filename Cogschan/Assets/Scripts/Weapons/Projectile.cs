using UnityEngine;

/// <summary>
/// Script that moves a kinematic rigidbody at constant speed.
/// </summary>
[RequireComponent(typeof(Rigidbody))]
public class Projectile : MonoBehaviour
{
    [Tooltip("The rigidbody that is attached to this projectile.")]
    [SerializeField] private Rigidbody _rigidBody;
    [Tooltip("The speed of the projectile.")]
    [SerializeField] private float _speed;

    private Vector3 _velocity = Vector3.zero;

    /// <summary>
    /// Tells the projectile to move in a specific direction.
    /// </summary>
    public void SetDirection(Vector3 direction)
    {
        _velocity = _speed * direction.normalized;
    }

    // Because the rigidbody is kinematic, we have to do the moving ourselves.
    // Since it's a rigidbody, used fixed update (kinematic body will do smooth interpolation for us)
    // We also don't have to check if timescale = 0, because physics inherently uses timescale
    private void FixedUpdate()
    {
        _rigidBody.MovePosition(_rigidBody.position + _velocity * Time.fixedDeltaTime);
    }
}

