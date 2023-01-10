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
    }

    void Update()
    {
        nav.SetDestination(Player.position);
        Model.LookAt(new Vector3(Player.position.x, transform.position.y, Player.position.z)); 
        Model.Rotate(new Vector3(0, 180, 0));

    }
    void OnDrawGizmosSelected ()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, lookRadius);
    }
}
