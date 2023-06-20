using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// A simple script responsible for making objects face the camera
// Mostly useful for UI elements that sit in world space (eg. enemy healthbars), and therefore must be rotated to face the camera
public class SimpleBillboard : MonoBehaviour
{
    // Drop in the target game object into this inspector field
    public Transform POV; 

    void Update()
    {
        // This gets the transform of the script's game object to point at the target object
        transform.LookAt(POV);

        // For some reason, the UI is flipped when it's made to face the camera
        // Therefore, just flip it back
        transform.Rotate(new Vector3(0, 180, 0));
    }
}
