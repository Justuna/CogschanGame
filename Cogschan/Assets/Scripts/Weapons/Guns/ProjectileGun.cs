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
        Vector3 dir = (targetPosition - _muzzle.transform.position).normalized;
        if (_spreadEvent is not null)
            SpreadEvent.ApplySpread(ref dir, _spreadEvent.GetSpread());
        print("Magnitude of spread: " + _spreadEvent?.GetSpread().magnitude);
        Projectile proj = Instantiate(_projectilePrefab, _muzzle.transform.position, Quaternion.identity);
        proj.SetDirection(dir);
    }

    protected override void FireAccurate(Vector3 targetPosition)
    {
        // TODO: Make this do different stuff
        Fire(targetPosition);
    }
}