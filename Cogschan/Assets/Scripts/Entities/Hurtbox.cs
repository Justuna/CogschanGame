using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// A class for storing information about hurtboxes. Requires a kinematic rigidbody and a trigger collider.
/// </summary>
[RequireComponent(typeof(Rigidbody))]
public class Hurtbox : MonoBehaviour
{
    [SerializeField] private EntityServiceLocator _services;
    [SerializeField] private float _critMultiplier = 1f;

    public EntityServiceLocator Services => _services;
    public float CritMultiplier => _critMultiplier;
}