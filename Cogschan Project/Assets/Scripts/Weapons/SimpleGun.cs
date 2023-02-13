using UnityEngine;

/// <summary>
/// A dummy weapon for testing purposes, may be repurposed into pistol.
/// </summary>
public class SimpleGun : Gun
{
    [Header("Simple Gun Attributes")]
    [SerializeField]
    private int Damage;
    [SerializeField]
    private GameObject HitParticle;
    [SerializeField]
    private GameObject CritParticle;
    [SerializeField]
    private GameObject WallParticle;
    [SerializeField]
    private Transform Muzzle;
    [SerializeField]
    private LayerMask AimColliderLayerMask;
    [SerializeField] private Vector3 BulletSpreadVariance = new Vector3(0.1f, 0.1f, 0.1f);
    [SerializeField] private float ADSAccuracyBoost = 5;
    [SerializeField] private int BulletCount = 1;


    public override bool HipFire()
    {
        if (!base.HipFire())
            return false;

        Vector2 screenCenterPoint = new Vector2(Screen.width / 2f, Screen.height / 2f);

        for (int x = 0; x < BulletCount; x++)
        {
            Ray cameraRay = new Ray(Camera.main.ScreenToWorldPoint(screenCenterPoint) + (Camera.main.transform.forward) * playerController.forwardCameraDisplacement, Camera.main.transform.forward);
            cameraRay.direction += new Vector3(Random.Range(-BulletSpreadVariance.x, BulletSpreadVariance.x),
                                                Random.Range(-BulletSpreadVariance.y, BulletSpreadVariance.y),
                                                Random.Range(-BulletSpreadVariance.z, BulletSpreadVariance.z)
                                                );
            cameraRay.direction.Normalize();

            Physics.Raycast(cameraRay, out RaycastHit hit, Mathf.Infinity, AimColliderLayerMask);

            Ray gunRay = new Ray(Muzzle.position, hit.point - Muzzle.position);

            if (Physics.Raycast(gunRay, out hit, Mathf.Infinity, AimColliderLayerMask))
            {
                Hitbox hitbox = hit.collider.GetComponent<Hitbox>();
                if (hitbox != null && hitbox.Multiplier > 1)
                {
                    Instantiate(CritParticle, hit.point, Quaternion.identity);
                    Debug.Log("IT'S A CRIT!");
                    hitbox.TakeHit(Damage);

                }
                else if (hitbox != null && hitbox.Multiplier <= 1)
                {
                    Instantiate(HitParticle, hit.point, Quaternion.identity);
                    Debug.Log("Normal ass hit...");
                    hitbox.TakeHit(Damage);
                }
                else
                {
                    Debug.Log("Hit nothing...");
                    Instantiate(WallParticle, hit.point, Quaternion.identity);
                }
            }
        }

        return true;
    }

    public override bool ADSFire()
    {
        if (!base.ADSFire())
            return false;

        Vector2 screenCenterPoint = new Vector2(Screen.width / 2f, Screen.height / 2f);

        for (int x = 0; x < BulletCount; x++)
        {
            Ray cameraRay = new Ray(Camera.main.ScreenToWorldPoint(screenCenterPoint) + (Camera.main.transform.forward) * playerController.forwardCameraDisplacement, Camera.main.transform.forward);
            cameraRay.direction += new Vector3(Random.Range((-BulletSpreadVariance.x) / ADSAccuracyBoost, (BulletSpreadVariance.x) / ADSAccuracyBoost),
                                                Random.Range((-BulletSpreadVariance.y) / ADSAccuracyBoost, (BulletSpreadVariance.y) / ADSAccuracyBoost),
                                                Random.Range((-BulletSpreadVariance.z) / ADSAccuracyBoost, (BulletSpreadVariance.z) / ADSAccuracyBoost)
                                                );
            cameraRay.direction.Normalize();

            Physics.Raycast(cameraRay, out RaycastHit hit, Mathf.Infinity, AimColliderLayerMask);

            Ray gunRay = new Ray(Muzzle.position, hit.point - Muzzle.position);

            if (Physics.Raycast(gunRay, out hit, Mathf.Infinity, AimColliderLayerMask))
            {
                Hitbox hitbox = hit.collider.GetComponent<Hitbox>();
                if (hitbox != null && hitbox.Multiplier > 1)
                {
                    Instantiate(CritParticle, hit.point, Quaternion.identity);
                    Debug.Log("IT'S A CRIT!");
                    hitbox.TakeHit(Damage);

                }
                else if (hitbox != null && hitbox.Multiplier <= 1)
                {
                    Instantiate(HitParticle, hit.point, Quaternion.identity);
                    Debug.Log("Normal ass hit...");
                    hitbox.TakeHit(Damage);
                }
                else
                {
                    Debug.Log("Hit nothing...");
                    Instantiate(WallParticle, hit.point, Quaternion.identity);
                }
            }
        }

        return true;
    }

}