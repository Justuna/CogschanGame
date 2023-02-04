using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CrosshairScript : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        foreach (Transform child in transform)
        {
            child.gameObject.SetActive(false);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (PlayerController.Singleton.ActState == ActionState.Fire)
        {
            Debug.Log("Firing");
            foreach (Transform child in transform)
            {
                child.gameObject.SetActive(true);
            }
        }
        else if (PlayerController.Singleton.MoveState == MovementState.ADS)
        {
            Debug.Log("ADSING");
            foreach (Transform child in transform)
            {
                child.gameObject.SetActive(true);
            }
            var eulerAngles = transform.eulerAngles;
            transform.rotation = Quaternion.Euler(eulerAngles.x, eulerAngles.y, eulerAngles.z + 0.6f);
        }
        else
        {
            Debug.Log("Nothing");
            foreach (Transform child in transform)
            {
                child.gameObject.SetActive(false);
            }
        }
        Debug.Log(PlayerController.Singleton.ActState + "owo");
    }
}
