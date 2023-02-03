using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ZiplineMech : MonoBehavior
{
    [SerializeField] private ZiplineMech target;
    [SerializeField] private float speed = 5f;
    [SerializeField] private float scale = 0.2f;
    [SerializeField] private float arrival = 0.4f;
    [SerializeField] private LineRenderer cable;

    public Transform ZipTransform
    private bool zipping = false;
    private GameObject var;

    private void Awake()
    {
        cable.SetPosition(0, ZipTransform.position);
        cable.SetPosition(1, target.ZipTransform.position);
    }

    private void Update()
    {
        if(!zipping || var == null) return;

        var.GetComponent<RigidBody>().AddForce((target.ZipTransform.position - ZiplineTransform.position).normalized * speed * Time.deltaTime, ForceMode.Acceleration);// update statement

        if(Vector3.Distance(var.transform.position, target.ZipTransform.position)<= arrival){
            ResetZipline;
        }
    }

    public void RidingZip(GameObject player)
    {
        var = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        var.transform.position = ZipTransform.position;
        var.transform.localScale = new Vector3(scale, scale, scale);
        var.AddComponent<RigidBody>().useGravity = false;
        var.GetComponent<Collider>().isTrigger = true;
        
        player.GetComponent<RigidBody>().useGravity = false;
        player.GetComponent<RigidBody>().isKinematic = true;
        player.GetComponent<RigidBody>().velocity = Vector3.zero;
        player.GetComponent<ThirdPersonController>().enabled = false;
        player.GetComponent<ThirdPersonInput>().enabled = false;
        player.transform.parent = var.transform;
        zipping = true;

    }

    private void ResetZipline()
    {
        if(!zipping) return;

        GameObject player = var.transform.GetChild(0).gameObject;
        player.GetComponent<RigidBody>().useGravity = true;
        player.GetComponent<RigidBody>().isKinematic = false;
        player.GetComponent<RigidBody>().velocity = Vector3.zero;
        player.GetComponent<ThirdPersonController>().enabled = true;
        player.GetComponent<ThirdPersonInput>().enabled = true;
        player.transform.parent = null;
        Destroy(var);
        var = null;
        zipping = null;
        Debug.Log("Zipline reset")



    }







} 