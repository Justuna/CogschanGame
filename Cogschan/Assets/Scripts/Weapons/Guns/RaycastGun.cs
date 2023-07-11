using UnityEngine;
using UnityEngine.VFX;

/// <summary>
/// A gun that fires raycasts with zero travel time.
/// </summary>
public class RaycastGun : Gun
{
    [Header("Raycast Gun Attributes")]
    [SerializeField] private GameObject _beamEffectPrefab;
    [SerializeField] private LayerMask _layerMask;
    [SerializeField] private int _damage;
    [SerializeField] private bool _canCrit;

    protected override void Fire(Vector3 targetPosition) => FireRaycast(targetPosition, _spreadEvent);

    protected override void FireAccurate(Vector3 targetPosition) => FireRaycast(targetPosition, _spreadEventAccurate);

    // Create the raycast given the parameters.
    private void FireRaycast(Vector3 targetPosition, SpreadEvent spread)
    {
        Vector3 dir = (targetPosition - _muzzle.transform.position).normalized;
        for (int i = 0; i < _count; i++)
        {
            Vector3 spreadDir = spread is not null ? spread.ApplySpread(dir) : dir;
            if (Physics.Raycast(_muzzle.position, spreadDir, out RaycastHit hitInfo, Mathf.Infinity, _layerMask))
            {
                HitObject(hitInfo.collider.gameObject);
                IBeamEffectPlayer beamEffect = Instantiate(_beamEffectPrefab).GetComponent<IBeamEffectPlayer>();
                beamEffect?.Fire(_muzzle.position, hitInfo.point);
            }
        }
    }

    private void HitObject(GameObject gameObject)
    {
        Hurtbox hurtbox = gameObject.GetComponent<Hurtbox>();
        if (hurtbox != null)
        {
            int damage = _damage;
            if (_canCrit) damage = (int) (damage * hurtbox.CritMultiplier);
            hurtbox.Services.HealthTracker.Damage(damage);
        }
    }
}