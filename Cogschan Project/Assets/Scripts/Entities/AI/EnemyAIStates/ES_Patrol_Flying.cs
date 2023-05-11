using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ES_Patrol_Flying : ES_Patrol
{
    public LayerMask GroundMask;
    public Rigidbody RB;
    public float MinimumHeight;
    public float MaximumPreferredHeight;
    public float MinimumDistanceFromObstacles;
    public float Speed;

    protected override void SearchPatrolPoint()
    {
        Vector3 randomDir = Quaternion.Euler(Random.Range(-45, 45), Random.Range(0, 360), 0) * Vector3.forward;
        float randomDist = Random.Range(MinPatrolRange, MaxPatrolRange);
        Vector3 randomPos = transform.position + randomDir * randomDist;

        // Make sure point isn't in a solid object
        if (Physics.CheckSphere(randomPos, MinimumDistanceFromObstacles, GroundMask)) return;

        Ray down = new Ray(randomPos, Vector3.down);
        Vector3 candidate;
        // If there is a floor under the point, want the point at minimum height above it
        if (Physics.Raycast(down, out RaycastHit hit, Mathf.Infinity, GroundMask))
        {
            Ray up = new Ray(hit.point, Vector3.up);
            // If there is a ceiling, make sure it can at least maintain distance
            if (Physics.Raycast(up, out RaycastHit hit2, MaximumPreferredHeight + MinimumDistanceFromObstacles, GroundMask))
            {
                // Gap too small, no salvaging
                if (hit2.distance < 2 * MinimumDistanceFromObstacles) return;
                else
                {
                    float max = hit2.distance - MinimumDistanceFromObstacles;
                    float dist = Random.Range(Mathf.Min(max, MinimumHeight), Mathf.Min(max, MaximumPreferredHeight));
                    candidate = hit.point + new Vector3(0, dist, 0);
                }
            }

            // If there is no ceiling, take this new point
            else
            {
                candidate = hit.point + new Vector3(0, Random.Range(MinimumHeight, MaximumPreferredHeight), 0);
            }
        }
        // If there is no floor, just move toward the point
        else
        {
            candidate = randomPos;
        }

        // If there is line of sight, accept candidate
        Ray between = new Ray(candidate, transform.position - candidate);
        if (!Physics.Raycast(between, Vector3.Magnitude(transform.position - candidate), GroundMask))
        {
            _patrolPoint = candidate;
            _hasSetPatrolPoint = true;
        }
    }

    protected override void MoveToPatrolPoint()
    {
        Vector3 dir = Vector3.Normalize(_patrolPoint - transform.position);
        RB.velocity = dir * Speed;
    }
}
