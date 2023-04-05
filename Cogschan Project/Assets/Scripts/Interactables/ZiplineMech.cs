using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ZiplineMech : MonoBehaviour
{
    [SerializeField] private ZiplineMech target;
    [SerializeField] private float speed = 5f;
    [SerializeField] private float scale = 0.2f;
    [SerializeField] private float arrival = 0.4f;
    [SerializeField] private LineRenderer cable;

    public Transform ZipTransform;
    private bool zipping = false;
    private GameObject var;

    private void Awake()
    {
        cable.SetPosition(0, ZipTransform.position);
        cable.SetPosition(1, target.ZipTransform.position);
    }

    private void Update()
    {
        if (!zipping || var == null) return;

        var.GetComponent<Rigidbody>().AddForce((target.ZipTransform.position - ZipTransform.position).normalized * speed * Time.deltaTime, ForceMode.Acceleration);// update statement

        if (Vector3.Distance(var.transform.position, target.ZipTransform.position) <= arrival)
        {
            ResetZipline();
        }
    }

    public void RidingZip(GameObject player)
    {
        var = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        var.transform.position = ZipTransform.position;
        var.transform.localScale = new Vector3(scale, scale, scale);
        var.AddComponent<Rigidbody>().useGravity = false;
        var.GetComponent<Collider>().isTrigger = true;

        player.GetComponent<Rigidbody>().useGravity = false;
        player.GetComponent<Rigidbody>().isKinematic = true;
        player.GetComponent<Rigidbody>().velocity = Vector3.zero;
        player.GetComponent<PlayerController>().enabled = false;
        player.transform.parent = var.transform;
        zipping = true;

    }

    private void ResetZipline()
    {
        if (!zipping) return;

        GameObject player = var.transform.GetChild(0).gameObject;
        player.GetComponent<Rigidbody>().useGravity = true;
        player.GetComponent<Rigidbody>().isKinematic = false;
        player.GetComponent<Rigidbody>().velocity = Vector3.zero;
        player.GetComponent<PlayerController>().enabled = true;
        player.transform.parent = null;
        Destroy(var);
        var = null;
        zipping = false;
        Debug.Log("Zipline reset");



    }







}