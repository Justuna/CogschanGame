using UnityEngine;

/// <summary>
/// A gun that fires raycasts with zero travel time.
/// </summary>
public class RaycastGun : Gun
{
    [Header("Projectile Attributes")]
    [Tooltip("The prefab of the projectile that this gun shoots. Should only serve a visual purpose")]
    [SerializeField] private Projectile _projectilePrefab;

    protected override void Fire(Vector3 targetPosition) => FireRaycast(targetPosition, _spreadEvent);

    protected override void FireAccurate(Vector3 targetPosition) => FireRaycast(targetPosition, _spreadEventAccurate);

    // Create the raycast given the paramaeters.
    private void FireRaycast(Vector3 targetPosition, SpreadEvent spread)
    {
        Vector3 dir = (targetPosition - _muzzle.transform.position).normalized;

        if(Time.timeScale != 0){
            for (int i = 0; i < _count; i++)
            {
                Vector3 spreadDir = spread is not null ? spread.ApplySpread(dir) : dir;
                if (Physics.Raycast(_muzzle.position, spreadDir, out RaycastHit hitInfo))
                    HitObject(hitInfo.collider.gameObject);
                if (_projectilePrefab is not null)
                {
                Projectile proj = Instantiate(_projectilePrefab, _muzzle.position, Quaternion.identity);
                proj.SetDirection(spreadDir);
                }
            }
        }
        
    }

    private void HitObject(GameObject gameObject)
    {
        Debug.Log($"Raycast has collided with {gameObject.name}.");
    }
}