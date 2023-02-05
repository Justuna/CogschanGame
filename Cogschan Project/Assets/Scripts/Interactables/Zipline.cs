using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Zipline : MonoBehaviour
{
    private Transform ZiplineTransform;
    public Transform ZiplineTarget;
    public float ZiplineSpeed = 15.0f;
    private GameObject CogsChan;
    // Start is called before the first frame update
    void Start()
    {
        ZiplineTransform = gameObject.GetComponentInChildren<Transform>();
        CogsChan = GameObject.FindGameObjectWithTag("Player");
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log(other + "triggered the zipline");
        ZiplineTransform.position = Vector3.MoveTowards(ZiplineTransform.position, ZiplineTarget.position, ZiplineSpeed*Time.deltaTime); 
        //CogsChan.GetComponent<PlayerController>().enabled = false;
        //CogsChan.transform.position = Vector3.MoveTowards(CogsChan.transform.position, ZiplineTarget.position, 1 * Time.deltaTime);
        //CogsChan.GetComponent<CharacterController>().Move(ZiplineTarget.position*Time.deltaTime);
        // CogsChan.GetComponent<PlayerController>().enabled = true;
    }

}
