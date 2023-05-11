using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LOSCalculator : MonoBehaviour
{
    public Transform Player;
    public Transform Model;

    public LayerMask GroundMask, PlayerMask;


    public float SightRange;
    public float LOSInterruptThreshold;

    public bool CanSee { get; private set; }

    public Vector3 LastSeenPosition { get; private set; }

    protected float _LOSInterruptClock;

    void Update()
    {
        Vector3 dir = Player.position - transform.position;
        Ray ray = new Ray(transform.position, dir);
        RaycastHit hit = new RaycastHit();
        bool LOS = Physics.CheckSphere(transform.position, SightRange, PlayerMask) && !Physics.Raycast(ray, out hit, dir.magnitude, GroundMask);
        Debug.DrawLine(transform.position, hit.point, Color.red);
        if (LOS)
        {
            _LOSInterruptClock = LOSInterruptThreshold;
            LastSeenPosition = Player.position;
            CanSee = true;
        }
        else
        {
            if (_LOSInterruptClock > 0)
            {
                _LOSInterruptClock -= Time.deltaTime;
                LastSeenPosition = Player.position;
                CanSee = true;
            }
            else CanSee = false;
        }
    }
}
