using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimpleProjectileGun : Gun
{
    [Header("Simple Gun Attributes")]
    [SerializeField]
    private GameObject Projectile;
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
        
        for(int x = 0; x<BulletCount; x++)
        {       
        Ray cameraRay = new Ray(Camera.main.ScreenToWorldPoint(screenCenterPoint) + (Camera.main.transform.forward) * thirdPersonController.forwardCameraDisplacement, Camera.main.transform.forward);
        cameraRay.direction += new Vector3(Random.Range(-BulletSpreadVariance.x, BulletSpreadVariance.x),
                                            Random.Range(-BulletSpreadVariance.y, BulletSpreadVariance.y),
                                            Random.Range(-BulletSpreadVariance.z, BulletSpreadVariance.z)
                                            );
        cameraRay.direction.Normalize();

        Physics.Raycast(cameraRay, out RaycastHit hit, 999f, AimColliderLayerMask);

        Vector3 dir = hit.point - Muzzle.position;
        Bullet bullet = Instantiate(Projectile, transform.position, Quaternion.identity).GetComponent<Bullet>();
        if (bullet != null) bullet.Launch(dir);
        }
        return true;
    }

    public override bool ADSFire()
    {
        if (!base.ADSFire())
            return false;

        Vector2 screenCenterPoint = new Vector2(Screen.width / 2f, Screen.height / 2f);

        for(int x = 0; x<BulletCount; x++)
        {
        Ray cameraRay = new Ray(Camera.main.ScreenToWorldPoint(screenCenterPoint) + (Camera.main.transform.forward) * thirdPersonController.forwardCameraDisplacement, Camera.main.transform.forward);
        
        cameraRay.direction += new Vector3(Random.Range((-BulletSpreadVariance.x)/ADSAccuracyBoost, (BulletSpreadVariance.x)/ADSAccuracyBoost),
                                            Random.Range((-BulletSpreadVariance.y)/ADSAccuracyBoost, (BulletSpreadVariance.y)/ADSAccuracyBoost),
                                            Random.Range((-BulletSpreadVariance.z)/ADSAccuracyBoost, (BulletSpreadVariance.z)/ADSAccuracyBoost)
                                            );
        cameraRay.direction.Normalize();

        Physics.Raycast(cameraRay, out RaycastHit hit, 999f, AimColliderLayerMask);

        Vector3 dir = hit.point - Muzzle.position;
        Bullet bullet = Instantiate(Projectile, transform.position, Quaternion.identity).GetComponent<Bullet>();
        if (bullet != null) bullet.Launch(dir);
        }
        return true;
    
    }
}
