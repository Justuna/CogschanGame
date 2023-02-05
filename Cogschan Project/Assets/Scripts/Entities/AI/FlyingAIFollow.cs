using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlyingAIFollow : MonoBehaviour
{
    public float Speed;
    public float OrbitDistance;
    public float PreferredHeight;

    public Transform Target;

    private Rigidbody _rb;

    private void Awake()
    {
        _rb = GetComponent<Rigidbody>();
    }

    void FixedUpdate()
    {
        if (Vector3.Distance(transform.position, Target.position) <= OrbitDistance) return;

        Vector3 dir = Vector3.Normalize(Target.position - transform.position);
        _rb.AddForce(dir * Speed);
    }
}
