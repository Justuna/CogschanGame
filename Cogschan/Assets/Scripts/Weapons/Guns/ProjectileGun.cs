using System.Runtime.CompilerServices;
using UnityEditor;
using UnityEngine;

public class ProjectileGun : Gun
{
    [Header("Projectile Attributes")]
    [Tooltip("The prefab of the projectile that this gun shoots.")]
    [SerializeField] private GameObject _projectilePrefab;

    protected override void Fire(Vector3 targetPosition)
    {
    }

    protected override void FireAccurate(Vector3 targetPosition)
    {
    }
}