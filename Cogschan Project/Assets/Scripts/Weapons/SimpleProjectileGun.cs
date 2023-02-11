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
    [SerializeField] private Vector3 MinBulletSpreadVariance = new Vector3(0.1f, 0.1f, 0.1f);
    [SerializeField] private Vector3 MaxBulletSpreadVariance = new Vector3(0.2f, 0.2f, 0.2f);
    [SerializeField] private float TimeToMaxVariance = 1;
    [SerializeField] private float TimeToCooldown = 0.5f;
    [SerializeField] private float ADSAccuracyBoost = 5;
    [SerializeField] private int BulletCount = 1;

    private float _varianceFactor = 0;

    public override bool HipFire()
    {
        if (!base.HipFire())
            return false;

        Vector3 finalVariance = Vector3.Lerp(MinBulletSpreadVariance, MaxBulletSpreadVariance, _varianceFactor);

        return FireBullets(finalVariance);
    }

    public override bool ADSFire()
    {
        if (!base.ADSFire())
            return false;

        Vector3 finalVariance = Vector3.Lerp(MinBulletSpreadVariance, MaxBulletSpreadVariance, _varianceFactor) / ADSAccuracyBoost;

        return FireBullets(finalVariance);
    }

    private bool FireBullets(Vector3 finalVariance)
    {
        Vector2 screenCenterPoint = new Vector2(Screen.width / 2f, Screen.height / 2f);

        for (int x = 0; x < BulletCount; x++)
        {
            Ray cameraRay = new Ray(Camera.main.ScreenToWorldPoint(screenCenterPoint) + (Camera.main.transform.forward) * playerController.forwardCameraDisplacement, Camera.main.transform.forward);
            cameraRay.direction += new Vector3(Random.Range(-finalVariance.x, finalVariance.x),
                                                Random.Range(-finalVariance.y, finalVariance.y),
                                                Random.Range(-finalVariance.z, finalVariance.z)
                                                );
            cameraRay.direction.Normalize();

            Physics.Raycast(cameraRay, out RaycastHit hit, 999f, AimColliderLayerMask);

            Vector3 dir = hit.point - Muzzle.position;
            Bullet bullet = Instantiate(Projectile, transform.position, Quaternion.identity).GetComponent<Bullet>();
            if (bullet != null) bullet.Launch(dir);
        }

        return true;
    }

    protected override void Update()
    {
        base.Update();

        if (TimeToCooldown == 0 || TimeToMaxVariance == 0) return;

        if (playerController.ActState == ActionState.Fire) _varianceFactor += Time.deltaTime / TimeToMaxVariance;
        else _varianceFactor -= Time.deltaTime / TimeToCooldown;

        _varianceFactor = Mathf.Clamp01(_varianceFactor);
    }
}
