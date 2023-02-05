using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace StarterAssets
{
    public class Bouncepad : MonoBehaviour
    {
        //Float Changes how high cogschan is sent in the air
        public float Bounceforce;
        //On touching the pad, the collision entity has its VerticalVelocity is set to bounceforce (should only work with cogs chan cause enemies
        //wouldnt work with this setup
        private void OnTriggerEnter(Collider other)
        {
            other.gameObject.GetComponent<PlayerController>().VerticalVelocity = Bounceforce;
        }
    }
}