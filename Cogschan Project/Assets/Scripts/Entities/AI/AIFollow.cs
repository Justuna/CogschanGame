using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class AIFollow : MonoBehaviour
{
    public float lookRadius = 10f;
    public NavMeshAgent nav;
    public Transform Player;
    public Transform Model;

    void Start()
    {
        nav = GetComponent<NavMeshAgent>();
        //currentTime = idleTime;
    }

    void Update()
    {
        NavMeshPath path = new NavMeshPath();
        NavMesh.SamplePosition(transform.position, out NavMeshHit hit, Mathf.Infinity, NavMesh.AllAreas);
        bool valid = NavMesh.CalculatePath(hit.position, Player.position, NavMesh.AllAreas, path);
        if (valid)
        {
            Vector3 bestTarget = path.corners[path.corners.Length - 1];
            nav.SetPath(path);
        }
        
        Model.LookAt(new Vector3(Player.position.x, transform.position.y, Player.position.z));
        Model.Rotate(new Vector3(0, 180, 0));

    }
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, lookRadius);
    }
}
