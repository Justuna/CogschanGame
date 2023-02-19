using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JanglingAI : MonoBehaviour
{
    //For making model look away
    public float _health;

    public Transform Model;
    //List of predetermined destination Nodes
    public GameObject[] destNodes;
    //Whether or not Jangling should be moving
    public bool shouldMove;
    //the node number its currently on
    public int currentNode;
    //the node number of its destination
    public int nextNode;
    //the position of its destination
    public Transform destination;
    //reference to the player character
    public Transform Player;
    public bool didItNotice;
    public float setStartleTime;
    public float startleTimer;
    public float setStunTime;
    public float stunTimer;
    public float speed;
    public float runDistance;
    public float noticeDistance;
    private void Awake()
    {
        startleTimer = setStartleTime;
        didItNotice = false;
        shouldMove = false;
        //resets Jangling position to first node in list
        transform.position = destNodes[0].transform.position;
        currentNode = 0;
        _health = 100;
        stunTimer = -1;
    }

    // Update is called once per frame
    void Update()
    {
        //Raycasts towards player to check lineOfSight
        RaycastHit hit;
        Physics.Raycast(transform.position, (Player.position - transform.position), out hit, 20f);
        Debug.DrawRay(transform.position, (Player.position - transform.position), Color.green);
        //running vs idle/notice
        if (stunTimer >= 0)
        {
            stunTimer -= Time.deltaTime;
            if (stunTimer < .1)
            {

                if (shouldMove) StunnedPickNewDestination();
                else PickNewDestination();
                stunTimer = -1;
                shouldMove = true;
            }

        }
        else if (shouldMove)
        {
            //Looks at and moves to destination Node
            Model.LookAt(destination.transform);
            transform.position = Vector3.MoveTowards(transform.position, destination.position, speed * Time.deltaTime);
            //Once reached, set shouldMove to false
            //reset destination and set current node ot next
            if (transform.position.Equals(destination.position) && shouldMove)
            {
                shouldMove = false;
                destination = null;
                currentNode = nextNode;
                didItNotice = false;
            }
        }
        else
        {
            //checks if player is close enough to initiate run away
            if (Vector3.Distance(transform.position, Player.position) < runDistance && hit.collider.tag == "Player" || startleTimer != setStartleTime)
            {
                if (didItNotice)
                {
                    PickNewDestination();
                    shouldMove = true;
                }
                else
                {
                    if (hit.collider.tag == "Player") Model.LookAt(Player.transform);

                    if (startleTimer >= 0)
                    {
                        startleTimer -= 1 * Time.deltaTime;
                    }
                    else
                    {
                        PickNewDestination();
                        shouldMove = true;
                        startleTimer = setStartleTime;
                    }
                }
            }
            //checks if player is close enough for jangly to notice
            else if (Vector3.Distance(transform.position, Player.position) < noticeDistance && Vector3.Distance(transform.position, Player.position) > runDistance)
            {
                if (hit.collider.tag == "Player")
                {
                    Model.LookAt(Player.transform);
                    didItNotice = true;
                }
                else { didItNotice = false; }
            }
            else
            {
                didItNotice = false;
            }
        }
    }

    //Picks node from adjacents furthest away from player
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
    }
    private void StunnedPickNewDestination()
    {
        if (Vector3.Distance(destNodes[currentNode].transform.position, Player.position) > Vector3.Distance(destNodes[nextNode].transform.position, Player.position))
        {
            destination = destNodes[currentNode].transform;
        }
        else { destination = destNodes[nextNode].transform; }
    }

    public void DealDamage(float amount)
    {
        _health = Mathf.Max(_health - amount, 0);

        if (_health <= 0)
        {
            stunTimer = setStunTime;
            _health = 100;
        }
    }
}
