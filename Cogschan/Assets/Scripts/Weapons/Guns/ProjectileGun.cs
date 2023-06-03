using UnityEngine;

/// <summary>
/// A gun that fires projectiles with a non-zero travel time.
/// </summary>
public class ProjectileGun : Gun
{
    [Header("Projectile Attributes")]
    [Tooltip("The prefab of the projectile that this gun shoots.")]
    [SerializeField] private Projectile _projectilePrefab;

    protected override void Fire(Vector3 targetPosition) => SpawnProjectile(targetPosition, _spreadEvent);

    protected override void FireAccurate(Vector3 targetPosition) => SpawnProjectile(targetPosition, _spreadEventAccurate);

    // Spawn the projectile given the paramaeters.
    private void SpawnProjectile(Vector3 targetPosition, SpreadEvent spread)
    {
        Vector3 dir = (targetPosition - _muzzle.transform.position).normalized;
        for (int i = 0; i < _fireCount; i++)
        {
            Projectile proj = Instantiate(_projectilePrefab, _muzzle.position, Quaternion.identity);
            proj.SetDirection(spread.ApplySpread(dir));
        }
    }
}