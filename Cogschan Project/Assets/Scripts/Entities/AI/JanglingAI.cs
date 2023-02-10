using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JanglingAI : MonoBehaviour
{
    public Transform Model;
    public GameObject[] posNodes;
    public bool[][] adjMatrix;
    public bool shouldMove;
    public Transform destination;
    public Transform Player;
    public float speed;
    private void Awake()
    {
        shouldMove = false;
        destination = posNodes[1].transform;
    }

    // Update is called once per frame
    void Update()
    {
        if (shouldMove)
        {
            //Looks at and moves to destination Node
            Model.LookAt(posNodes[1].transform);
            transform.position = Vector3.MoveTowards(transform.position, destination.position, speed * Time.deltaTime);
            //Once reached, set shouldMove to false
            if(transform.position.Equals(destination)){
                shouldMove = false;
            }
        }
    }


    private void PickNewNode(){
        
    }
}
