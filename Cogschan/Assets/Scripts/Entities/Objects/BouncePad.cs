using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class BouncePad : MonoBehaviour
{
    [SerializeField] private Collider _collider;
    [SerializeField] private Vector3 _impulse;
    [SerializeField] private bool _isRelative;
    [SerializeField] private float _proneTime = 0f;

    private void OnTriggerEnter(Collider other)
    {
        KinematicPhysics movementHandler = other.GetComponentInChildren<KinematicPhysics>();
        PlayerMovementController movementController = other.GetComponentInChildren<PlayerMovementController>();
        if (movementHandler != null)
        {
            Vector3 actualImpulse = _impulse;
            if (_isRelative)
            {
                actualImpulse = transform.rotation * _impulse;
            }
            movementHandler.AddImpulse(actualImpulse, true, 0);
        }
        if (movementController != null && _proneTime > 0)
        {
            movementController.KnockProne(_proneTime);
        }
    }
}
