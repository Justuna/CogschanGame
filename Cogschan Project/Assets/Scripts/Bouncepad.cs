using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace StarterAssets
{
    public class Bouncepad : MonoBehaviour
    {
        public float Bounceforce;
        private void OnTriggerEnter(Collider other)
        {
            Debug.Log(other);
            other.gameObject.GetComponent<PlayerMovement>()._verticalVelocity = Bounceforce;
        }
    }
}