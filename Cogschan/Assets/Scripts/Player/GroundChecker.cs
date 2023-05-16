using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class GroundChecker : MonoBehaviour
{
    [SerializeField] private LayerMask _ground;
    [SerializeField] private float _error = 0.1f;

    public bool IsGrounded { get; private set; }

    private void Update()
    {
        Ray down = new Ray(transform.position, Vector3.down);
        IsGrounded = Physics.Raycast(down, _error, _ground);
    }

    private void OnDrawGizmos()
    {
        if (IsGrounded)
        {
            Gizmos.color = Color.green;
        }
        else
        {
            Gizmos.color = Color.red;
        }

        Gizmos.DrawLine(transform.position, transform.position + Vector3.down * _error);
    }
}
