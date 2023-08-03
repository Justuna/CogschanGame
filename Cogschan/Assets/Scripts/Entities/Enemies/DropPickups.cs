using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DropPickups : MonoBehaviour
{
    [Tooltip("A table of potential drop items mapped to their weights.")]
    [SerializeField] private DropTableEntry[] _dropTable;
    [Tooltip("The minimum lateral velocity possible when this is spawned.")]
    [SerializeField] private float _minLateralVelocity;
    [Tooltip("The maximum lateral velocity possible when this is spawned.")]
    [SerializeField] private float _maxLateralVelocity;
    [Tooltip("The minimum upward velocity possible when this is spawned.")]
    [SerializeField] private float _minUpwardVelocity;
    [Tooltip("The maximum upward velocity possible when this is spawned.")]
    [SerializeField] private float _maxUpwardVelocity;
    [Tooltip("The minimum velocity possible when this is spawned.")]
    [SerializeField] private Vector3 _offset;

    [Serializable]
    public struct DropTableEntry {
        public GameObject Drop;
        public float Weight;
    }

    /// <summary>
    /// Drops a random item from the drop table based on its weight.
    /// </summary>
    /// <param name="amount">The amount of items to drop. If no number is specified, drops one item.</param>
    public void Drop(int amount = 1)
    {
        float sum = 0;
        foreach (DropTableEntry drop in _dropTable) 
        {
            sum += drop.Weight;
        }

        for (int i = 0; i < amount; i++)
        {
            float index = UnityEngine.Random.value * sum;

            float dropSum = 0;
            GameObject toBeDropped = null;
            foreach (DropTableEntry drop in _dropTable)
            {
                dropSum += drop.Weight;
                if (index <= dropSum)
                {
                    toBeDropped = drop.Drop;
                }
            }

            GameObject hasDropped = Instantiate(toBeDropped, transform.position + _offset, Quaternion.identity);

            EntityServiceLocator services = hasDropped.GetComponent<EntityServiceLocator>();
            if (services != null && services.KinematicPhysics != null)
            {
                float forward = UnityEngine.Random.Range(_minLateralVelocity, _maxLateralVelocity);
                float upward = UnityEngine.Random.Range(_minUpwardVelocity, _maxUpwardVelocity);
                Quaternion rotation = Quaternion.AngleAxis(UnityEngine.Random.Range(0f, 360f), Vector3.up);

                Vector3 impulse = rotation * (Vector3.forward * forward + Vector3.up * upward);
                Debug.Log(impulse);
                services.KinematicPhysics.AddImpulse(impulse);
            }
        }
    }
}
