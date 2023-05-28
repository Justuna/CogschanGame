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
        Projectile proj = Instantiate(_projectilePrefab, _muzzle.transform.position, Quaternion.identity);
        proj.SetDirection(targetPosition - _muzzle.transform.position);
    }

    protected override void FireAccurate(Vector3 targetPosition)
    {
        // TODO: Make this do different stuff
        Fire(targetPosition);
    }
}