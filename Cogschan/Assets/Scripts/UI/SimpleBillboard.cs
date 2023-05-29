using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

// A simple script responsible for making objects face the camera
// Mostly useful for UI elements that sit in world space (eg. enemy healthbars), and therefore must be rotated to face the camera
public class SimpleBillboard : MonoBehaviour
{
    [SerializeField] private bool _isFlipped;

    void Update()
    {
        // This gets the transform of the script's game object to point at the camera
        transform.LookAt(Camera.main.transform);

        // For some reason, the UI is flipped when it's made to face the camera
        // Therefore, just flip it back
        if (_isFlipped) transform.Rotate(new Vector3(0, 180, 0));
    }
}
