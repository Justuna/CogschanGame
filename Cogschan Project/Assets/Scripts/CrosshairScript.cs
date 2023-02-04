using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CrosshairScript : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        //Disables the outer crosshair
        foreach (Transform child in transform)
        {
            child.gameObject.SetActive(false);
        }
    }

    // Update is called once per frame
    void Update()
    {
        //When the player is firing
        if (PlayerController.Singleton.ActState == ActionState.Fire)
        {
            //reenable the outer crosshair
            foreach (Transform child in transform)
            {
                child.gameObject.SetActive(true);
            }
        }
        //if the player is aiming 
        else if (PlayerController.Singleton.MoveState == MovementState.ADS)
        {
            //reenable the outer crosshair
            foreach (Transform child in transform)
            {
                child.gameObject.SetActive(true);
            }
            //rotate the outer crosshair
            var eulerAngles = transform.eulerAngles;
            transform.rotation = Quaternion.Euler(eulerAngles.x, eulerAngles.y, eulerAngles.z + 0.6f);
        }
        //if nothing else 
        else
        {
            //disable the outer crosshair
            foreach (Transform child in transform)
            {
                child.gameObject.SetActive(false);
            }
        }
    }
}
