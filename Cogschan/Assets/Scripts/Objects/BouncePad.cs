using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class BouncePad : MonoBehaviour
{
    [SerializeField] private Collider _collider;
    [SerializeField] private Vector3 _impulse;
    [SerializeField] private bool _isRelative;

    private void OnTriggerEnter(Collider other)
    {
        IHurtbox hurtbox = other.GetComponentInChildren<IHurtbox>();
        if (hurtbox != null)
        {
            Vector3 actualImpulse = _impulse;
            if (_isRelative)
            {
                actualImpulse = transform.rotation * _impulse;
            }
            hurtbox.AddImpulse(actualImpulse, true, 0);
        }
    }
}
