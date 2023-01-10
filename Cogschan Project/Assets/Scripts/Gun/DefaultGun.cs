using UnityEngine;

/// <summary>
/// A dummy weapon for testing purposes, may be repurposed into pistol.
/// </summary>
public class DefaultGun : Gun
{
    [SerializeField]
    private int damage;
    [SerializeField]
    private GameObject particle;
    [SerializeField]
    private GameObject critParticle;


    public override bool HipFire(Transform hitTransform)
    {
        if (!base.HipFire(hitTransform))
            return false;
        OnFire(hitTransform);
        return true;
    }

    public override bool ADSFire(Transform hitTransform)
    {
        if (!base.ADSFire(hitTransform))
            return false;
        OnFire(hitTransform);
        return true;
    }



    // Run when the gun fires
    private void OnFire(Transform hitTransform)
    {
        Hitbox hitbox = hitTransform.GetComponent<Hitbox>();
        if (hitbox != null && hitbox.multiplier >= 5)
        {
            Instantiate(critParticle, hitTransform.position, Quaternion.identity);
            Debug.Log("IT'S A CRIT!");
            hitbox.TakeHit(damage);

        }
        else if (hitbox != null && hitbox.multiplier < 5)
        {
            Instantiate(particle, hitTransform.position, Quaternion.identity);
            Debug.Log("Normal ass hit...");
            hitbox.TakeHit(damage);
        }

        else
        {
            //Instantiate(vfxHitRed, transform.position, Quaternion.identity);
            Debug.Log("FUCK!");
        }
    }
}