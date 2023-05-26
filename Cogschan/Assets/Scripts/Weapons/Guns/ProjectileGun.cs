using System.Runtime.CompilerServices;
using UnityEditor;
using UnityEngine;

public class ProjectileGun : Gun
{
    [Header("Projectile Attributes")]
    [Tooltip("The prefab of the projectile that this gun shoots.")]
    [SerializeField] private Projectile _projectilePrefab;

    protected override void Fire(Vector3 targetPosition)
    {
        Projectile proj = Instantiate(_projectilePrefab, transform.position + targetPosition.normalized, Quaternion.identity);
        proj.SetDirection(targetPosition);
    }

    protected override void FireAccurate(Vector3 targetPosition)
    {
        // TODO: Make this do different stuff
        Fire(targetPosition);
    }
}