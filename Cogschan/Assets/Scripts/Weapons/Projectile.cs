using UnityEngine;

/// <summary>
/// Projectiles to be fired out of <see cref="ProjectileGun"/>s.
/// </summary>
[RequireComponent(typeof(Rigidbody))]
public class Projectile : MonoBehaviour
{
    [SerializeField]
    [Tooltip("The speed of the projectile")]
    private float _speed;

    /// <summary>
    /// Makes the projectile move in the specified direction.
    /// </summary>
    public void SetDirection(Vector3 direction)
    {
        GetComponent<Rigidbody>().velocity = _speed * direction.normalized;
    }
}

