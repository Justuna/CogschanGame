using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasicFlyingEnemyAI : MonoBehaviour
{
    public float Speed;
    public float OrbitDistance;
    public float PreferredHeight;

    public Transform Target;
    public LayerMask GroundMask;

    private Rigidbody _rb;

    private void Awake()
    {
        _rb = GetComponent<Rigidbody>();
    }

    void Update()
    {
        if (Vector3.Distance(transform.position, Target.position) <= OrbitDistance) return;

        Vector3 dir = Vector3.Normalize(Target.position - transform.position);
        Ray ray = new Ray(transform.position, Vector3.down);
        if (Physics.Raycast(ray, out RaycastHit hit, PreferredHeight, GroundMask))
        {
            dir.y = Mathf.Max(dir.y, 0);
        }

        _rb.velocity = dir * Speed;
    }
}
