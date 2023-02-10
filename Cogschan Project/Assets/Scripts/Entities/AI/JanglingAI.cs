using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JanglingAI : MonoBehaviour
{
    public Transform Model;
    public Transform[] posNodes;
    public bool shouldMove;
    public Transform destination;
    public Transform Player;
    public float speed;
    private void Awake()
    {
        shouldMove = false;
        destination = posNodes[1];
    }

    // Update is called once per frame
    void Update()
    {
        if (shouldMove)
        {
            lookAtPoint(posNodes[1]);
            transform.position = Vector3.MoveTowards(transform.position, destination.position, speed * Time.deltaTime);
        }
    }
    private void lookAtPoint(Transform destination)
    {
        Model.LookAt(destination.position);
    }
}
