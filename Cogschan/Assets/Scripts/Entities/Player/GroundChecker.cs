using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Reflection.Emit;
using Unity.VisualScripting;
using UnityEngine;

[ExecuteInEditMode]
public class GroundChecker : MonoBehaviour
{
    public enum GroundCheckMethod { BOX_CHECK, SPHERE_CHECK, SPHERE_CAST, CHARACTER_CONTROLLER }
    public GroundCheckMethod CheckMethod;

    [SerializeField] private LayerMask _ground;

    [Header("Box Check Attributes")]
    [SerializeField] private float _yOffsetBox = 0.1f;
    [SerializeField] private float _height = 0.1f;
    [SerializeField] private float _width = 1f;

    [Header("Sphere Check/Cast Attributes")]
    [SerializeField] private float _yOffsetSphere = 0.1f;
    [SerializeField] private float _radius = 1f;

    [Header("Raycast Attributes")]
    [SerializeField] private float _castDist = 0.1f;

    [Header("Character Controller Attributes")]
    [SerializeField] private CharacterController _cc;

    [Header("Timer Attributes")]
    [SerializeField] private bool _useTimer;
    [SerializeField] private float _minTimeOffGround = 0.05f;

    private float _timer;

    public bool IsGrounded { get; private set; }

    private void Update()
    {
        bool isGrounded;

        switch (CheckMethod)
        {
            case GroundCheckMethod.BOX_CHECK:
                Vector3 halfExtents = new Vector3(_width / 2, _height, _width / 2);
                Vector3 offsetBox = new Vector3(0, _yOffsetBox, 0);
                isGrounded = Physics.CheckBox(transform.position + offsetBox, halfExtents, transform.rotation, _ground);
                break;
            case GroundCheckMethod.SPHERE_CHECK:
                Vector3 offsetSphere = new Vector3(0, _yOffsetSphere, 0);
                isGrounded = Physics.CheckSphere(transform.position + offsetSphere, _radius, _ground);
                break;
            case GroundCheckMethod.CHARACTER_CONTROLLER:
                isGrounded = _cc.isGrounded;
                break;
            default:
                isGrounded = false;
                break;
        }

        if (_useTimer && _timer > 0)
        {
            isGrounded = false;
            _timer -= Time.deltaTime;
        }

        IsGrounded = isGrounded;
    }

    public void BeginGroundedTimer()
    {
        _timer = _minTimeOffGround;
    }

    private void OnDrawGizmos()
    {
        switch (CheckMethod)
        {
            case GroundCheckMethod.BOX_CHECK:
                if (IsGrounded)
                {
                    Gizmos.color = Color.green;
                }
                else
                {
                    Gizmos.color = Color.red;
                }

                Vector3 size = new Vector3(_width, _height * 2, _width);
                Vector3 offsetBox = new Vector3(0, _yOffsetBox, 0);
                Gizmos.DrawWireCube(transform.position + offsetBox, size);
                break;
            case GroundCheckMethod.SPHERE_CHECK:
                if (IsGrounded)
                {
                    Gizmos.color = Color.green;
                }
                else
                {
                    Gizmos.color = Color.red;
                }

                Vector3 offsetSphere = new Vector3(0, _yOffsetSphere, 0);
                Gizmos.DrawWireSphere(transform.position + offsetSphere, _radius);
                break;
            case GroundCheckMethod.CHARACTER_CONTROLLER:
                break;
        }
        
    }
}
