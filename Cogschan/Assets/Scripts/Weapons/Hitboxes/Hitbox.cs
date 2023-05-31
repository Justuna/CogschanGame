using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// A script dedicated for holding information about damage sources/healing sources. Requires a kinematic rigidbody and a trigger collider.
/// </summary>
[RequireComponent (typeof(Rigidbody))]
public abstract class Hitbox : MonoBehaviour
{
    [Tooltip("The damage that this hitbox should deal.")]
    [SerializeField] protected int _damage;
    [Tooltip("Whether or not this hitbox should heal instead of dealing damage.")]
    [SerializeField] protected bool _isHealing = false;
}
