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


    public override bool HipFire()
    {
        if (!base.HipFire())
            return false;

        Vector2 screenCenterPoint = new Vector2(Screen.width / 2f, Screen.height / 2f);

        Ray cameraRay = new Ray(Camera.main.ScreenToWorldPoint(screenCenterPoint) + (Camera.main.transform.forward) * thirdPersonController.forwardCameraDisplacement, Camera.main.transform.forward);

        Physics.Raycast(cameraRay, out RaycastHit hit, 999f, AimColliderLayerMask);

        Vector3 dir = hit.point - Muzzle.position;
        Bullet bullet = Instantiate(Projectile, transform.position, Quaternion.identity).GetComponent<Bullet>();
        if (bullet != null) bullet.Launch(dir);

        return true;
    }

    public override bool ADSFire()
    {
        if (!base.ADSFire())
            return false;

        Vector2 screenCenterPoint = new Vector2(Screen.width / 2f, Screen.height / 2f);

        Ray cameraRay = new Ray(Camera.main.ScreenToWorldPoint(screenCenterPoint) + (Camera.main.transform.forward) * thirdPersonController.forwardCameraDisplacement, Camera.main.transform.forward);

        Physics.Raycast(cameraRay, out RaycastHit hit, 999f, AimColliderLayerMask);

        Vector3 dir = hit.point - Muzzle.position;
        Bullet bullet = Instantiate(Projectile, transform.position, Quaternion.identity).GetComponent<Bullet>();
        if (bullet != null) bullet.Launch(dir);

        return true;
    }
}
