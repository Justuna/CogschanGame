using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JanglingAI : MonoBehaviour
{
    public Transform Model;
    public GameObject[] destNodes;
    public bool shouldMove;
    public int currentNode;
    public int nextNode;
    public Transform destination;
    public Transform Player;
    public float speed;
    private void Awake()
    {
        shouldMove = false;
        transform.position = destNodes[0].transform.position;
        currentNode = 0;
    }

    // Update is called once per frame
    void Update()
    {
        if (shouldMove)
        {
            //Looks at and moves to destination Node
            Model.LookAt(destination.transform);
            transform.position = Vector3.MoveTowards(transform.position, destination.position, speed * Time.deltaTime);
            //Once reached, set shouldMove to false
            if (transform.position.Equals(destination.position) && shouldMove)
            {
                shouldMove = false;
                destination = null;
                currentNode = nextNode;
                Model.LookAt(Player.transform);
            }
        }
        else
        {
            Model.LookAt(Player.transform);
        }
    }
    void OnTriggerStay(Collider other)
    {
        PickNewDestination();
        shouldMove = true;
    }

    private void PickNewDestination()
    {
        JanglingNode node = destNodes[currentNode].GetComponent<JanglingNode>();
        int bestNode = 0;
        float greatestDistance = 0;
        for (int i = 0; i < node.adjacentNodes.Length; i++)
        {
            if (Vector3.Distance(destNodes[node.adjacentNodes[i]].transform.position, Player.position) > greatestDistance)
            {
                bestNode = i;
                greatestDistance = Vector3.Distance(destNodes[node.adjacentNodes[i]].transform.position, Player.position);
            }
        }
        destination = destNodes[node.adjacentNodes[bestNode]].transform;
        nextNode = node.adjacentNodes[bestNode];
        Debug.Log(destination + " " + nextNode);

    }
}
