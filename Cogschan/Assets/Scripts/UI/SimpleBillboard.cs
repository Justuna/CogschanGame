using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

/// <summary>
/// A script that makes the attached object face directly at the camera.
/// </summary>
/// <remarks>
/// Useful for in-game UI, which otherwise might not face the camera and therefore be hard to read.
/// </remarks>
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
