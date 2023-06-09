using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class BouncePad : Interactable
{
    [SerializeField] private Vector3 _impulse;
    [SerializeField] private bool _isRelative;

    protected override void InteractInternal(EntityServiceLocator services)
    {
        Vector3 actualImpulse = _impulse;
        if (_isRelative)
        {
            actualImpulse = transform.rotation * _impulse;
        }

        services.KinematicPhysics.AddImpulse(actualImpulse, true, 0);
    }
}
