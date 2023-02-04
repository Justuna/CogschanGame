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
    private GameObject Particle;
    [SerializeField]
    private GameObject CritParticle;
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

        Transform hitTransform = null;
        if (Physics.Raycast(cameraRay, out RaycastHit raycastHit, 999f, AimColliderLayerMask))
        {
            hitTransform = raycastHit.transform;
        }

        Ray gunRay = new Ray(Muzzle.position, raycastHit.point - Muzzle.position);

        if (Physics.Raycast(gunRay, out raycastHit, 999f, AimColliderLayerMask))
        {
            hitTransform = raycastHit.transform;
        }

        Hitbox hitbox = hitTransform?.GetComponent<Hitbox>();
        if (hitbox != null && hitbox.multiplier > 1)
        {
            Instantiate(CritParticle, hitTransform.position, Quaternion.identity);
            Debug.Log("IT'S A CRIT!");
            hitbox.TakeHit(Damage);

        }
        else if (hitbox != null && hitbox.multiplier <= 1)
        {
            Instantiate(Particle, hitTransform.position, Quaternion.identity);
            Debug.Log("Normal ass hit...");
            hitbox.TakeHit(Damage);
        }

        return true;
    }

    public override bool ADSFire()
    {
        if (!base.ADSFire())
            return false;

        Vector2 screenCenterPoint = new Vector2(Screen.width / 2f, Screen.height / 2f);

        Ray cameraRay = new Ray(Camera.main.ScreenToWorldPoint(screenCenterPoint) + (Camera.main.transform.forward) * thirdPersonController.forwardCameraDisplacement, Camera.main.transform.forward);
        Debug.DrawRay(Camera.main.ScreenToWorldPoint(screenCenterPoint) + (Camera.main.transform.forward) * thirdPersonController.forwardCameraDisplacement, Camera.main.transform.forward, Color.green);

        Transform hitTransform = null;
        if (Physics.Raycast(cameraRay, out RaycastHit raycastHit, 999f, AimColliderLayerMask))
        {
            hitTransform = raycastHit.transform;
        }

        Ray gunRay = new Ray(Muzzle.position, raycastHit.point - Muzzle.position);

        if (Physics.Raycast(gunRay, out raycastHit, 999f, AimColliderLayerMask))
        {
            hitTransform = raycastHit.transform;
        }

        Hitbox hitbox = hitTransform.GetComponent<Hitbox>();
        if (hitbox != null && hitbox.multiplier > 1)
        {
            Instantiate(CritParticle, hitTransform.position, Quaternion.identity);
            Debug.Log("IT'S A CRIT!");
            hitbox.TakeHit(Damage);

        }
        else if (hitbox != null && hitbox.multiplier <= 1)
        {
            Instantiate(Particle, hitTransform.position, Quaternion.identity);
            Debug.Log("Normal ass hit...");
            hitbox.TakeHit(Damage);
        }

        return true;
    }

}