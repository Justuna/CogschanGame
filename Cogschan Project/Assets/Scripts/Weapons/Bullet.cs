using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(Collider))]
public class Bullet : MonoBehaviour
{
    [SerializeField]
    private float MaxDamage = 10;

    [SerializeField] private float MinDamage = 1;
    [SerializeField] private float MaxFalloffDistance = 50;
    [SerializeField] private float MinFalloffDistance = 10;

    [SerializeField]
    private float Speed;
    [SerializeField]
    private GameObject HitParticles;
    [SerializeField]
    private GameObject CritParticles;
    [SerializeField] private GameObject WallParticles;

    private float _totalDistance = 0;
    private Vector3 _lastPosition;

    private Rigidbody _rb;

    private void Awake()
    {
        _lastPosition = transform.position;
        _rb = GetComponent<Rigidbody>();
    }

    private void Update()
    {
        /*
        float distance = Vector3.Distance(_lastPosition, transform.position);
        _totalDistance += distance ;
        _lastPosition = transform.position ;
        */

        float _distance = Speed * Time.deltaTime;
        _totalDistance += _distance;
    }

    public void Launch(Vector3 direction)
    {
        Vector3 velocity = direction.normalized * Speed;
        _rb.velocity = velocity;
    }

    private void OnTriggerEnter(Collider other)
    {
        Hurtbox hitbox = other.GetComponent<Hurtbox>();
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

            if(_totalDistance <= MinFalloffDistance)
            {
                hitbox.TakeHit(MaxDamage);
                Debug.Log("Did " + MaxDamage + " damage");
            } else{
                int _finalDamage = (int)Mathf.Lerp(MaxDamage, MinDamage, (_totalDistance - MinFalloffDistance)/(MaxFalloffDistance - MinFalloffDistance));
                hitbox.TakeHit(_finalDamage);
                Debug.Log("Did " + _finalDamage + " damage");
            }
            
        }
        
            Destroy(gameObject);
    }

    private void OnCollisionEnter(Collision collision)
    {
        
                
        Debug.Log("Total distance travelled:" + _totalDistance);
        Debug.Log("Destroyed self");
        Destroy(gameObject);
        Instantiate(WallParticles, gameObject.transform.position, Quaternion.identity);
        //Debug.Log("the j");
    }
}
