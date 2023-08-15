using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// Script that moves a kinematic rigidbody at constant speed.
/// </summary>
[RequireComponent(typeof(Rigidbody))]
public class Projectile : MonoBehaviour
{
    public UnityEvent Destroyed;

    [Tooltip("The rigidbody that is attached to this projectile.")]
    [SerializeField] private Rigidbody _rigidBody;
    [Tooltip("The speed of the projectile.")]
    [SerializeField] private float _speed;
    [Tooltip("How long the projectile lasts before destroying itself")]
    [SerializeField] private float _lifetime = 10f;
    [Tooltip("Makes the projectile face the direction it's moving")]
    [SerializeField] private bool _rotateToVelocity;

    private Vector3 _velocity = Vector3.zero;
    private float _time;

    /// <summary>
    /// Tells the projectile to move in a specific direction.
    /// </summary>
    public void SetDirection(Vector3 direction)
    {
        _velocity = _speed * direction.normalized;
        if (_rotateToVelocity)
            transform.LookAt(transform.position + direction);
        if (!_rigidBody.isKinematic)
            _rigidBody.velocity = _velocity;
    }

    private void Update()
    {
        _time += Time.deltaTime;
        if (_time > _lifetime)
        {
            Destroy(gameObject);
        }
    }

    // Because the rigidbody is kinematic, we have to do the moving ourselves.
    // Since it's a rigidbody, used fixed update (kinematic body will do smooth interpolation for us)
    // We also don't have to check if timescale = 0, because physics inherently uses timescale
    private void FixedUpdate()
    {
        if (_rigidBody.isKinematic)
            _rigidBody.MovePosition(_rigidBody.position + _velocity * Time.fixedDeltaTime);
    }

    private void OnDestroy()
    {
        Destroyed.Invoke();
    }
}

