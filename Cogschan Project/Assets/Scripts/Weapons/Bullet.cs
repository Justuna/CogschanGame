using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(Collider))]
public class Bullet : MonoBehaviour
{
    [SerializeField]
    private float Damage;
    [SerializeField]
    private float Speed;
    [SerializeField]
    private GameObject HitParticles;
    [SerializeField]
    private GameObject CritParticles;
    [SerializeField] private GameObject WallParticles;

    private Rigidbody _rb;

    private void Awake()
    {
        _rb = GetComponent<Rigidbody>();
    }

    public void Launch(Vector3 direction)
    {
        Vector3 velocity = direction.normalized * Speed;
        _rb.velocity = velocity;
    }

    private void OnTriggerEnter(Collider other)
    {
        Hitbox hitbox = other.GetComponent<Hitbox>();
        if (hitbox != null)
        {
            if (hitbox.Multiplier > 1)
            {
                Instantiate(CritParticles, transform.position, Quaternion.identity);
                Debug.Log("IT'S A CRIT!");

            }
            else if (hitbox.Multiplier <= 1)
            {
                Instantiate(HitParticles, transform.position, Quaternion.identity);
                Debug.Log("Normal ass hit...");
            }

            
            hitbox.TakeHit(Damage);
            Debug.Log("Did damage");

            
        }
        
            Destroy(gameObject);
    }

    private void OnCollisionEnter(Collision collision)
    {
        
                
            
        Debug.Log("Destroyed self");
        Destroy(gameObject);
        Instantiate(WallParticles, gameObject.transform.position, Quaternion.identity);
        //Debug.Log("the j");
    }
}
