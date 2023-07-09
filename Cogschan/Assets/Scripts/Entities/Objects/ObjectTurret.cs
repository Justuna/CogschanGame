using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectTurret : MonoBehaviour
{
    [Header("Basic Attributes")]
    [Tooltip("The point in space where the spawnable is spawned.")]
    [SerializeField] private Transform _spawnPoint;
    [Tooltip("The spawnable object that this turret \"shoots.\"")]
    [SerializeField] private GameObject _spawnable;
    [Tooltip("How long the turret waits since last shooting before shooting again at a minimum.")]
    [SerializeField] private float _minSpawnTime;
    [Tooltip("How long the turret waits since last shooting before shooting again at most.")]
    [SerializeField] private float _maxSpawnTime;

    [Header("Entity Attributes")]
    [Tooltip("Whether or not to apply an impulse on spawning. Requires the object to be an entity with kinematic physics.")]
    [SerializeField] private bool _hasImpulse;
    [Tooltip("Whether or not the impulse's direction is relative to the forward direction of the spawn point.")]
    [SerializeField] private bool _relativeImpulse;
    [Tooltip("The impulse to apply on spawning.")]
    [SerializeField] private Vector3 _impulse;

    private float _spawnTimer;

    private void Start()
    {
        ResetTimer();
    }

    private void Update()
    {
        if (_spawnTimer > 0) _spawnTimer -= Time.deltaTime;
        else
        {
            ResetTimer();
            Spawn();
        }
    }

    public void ResetTimer()
    {
        _spawnTimer = Random.Range(_minSpawnTime, _maxSpawnTime);
    }

    private void Spawn()
    {
        GameObject spawned = Instantiate(_spawnable, _spawnPoint.position, _spawnPoint.rotation);

        EntityServiceLocator entity = spawned.GetComponent<EntityServiceLocator>();
        if (entity != null)
        {
            if (entity.KinematicPhysics != null && _hasImpulse)
            {
                Vector3 impulse = _impulse;
                if (_relativeImpulse) impulse = _spawnPoint.rotation * impulse;
                entity.KinematicPhysics.AddImpulse(impulse, false, 0f);
            }
        }
    }
}
