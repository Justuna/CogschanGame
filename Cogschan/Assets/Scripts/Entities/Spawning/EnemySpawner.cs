using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UIElements;

public class EnemySpawner : Spawner
{
    [Header("Spawned object parameters")]
    [SerializeField]
    [Tooltip("The prefab to be spawned.")]
    private GameObject _prefab;
    [SerializeField]
    [Tooltip("The information for the spawn this spawner controls.")]
    private SpawnInfo _spawnInfo;
    [SerializeField]
    [Tooltip("A list of spawn managers to add the spawn card to.")]
    private SpawnManager[] _spawnManagers;

    [Header("Spawn position parameters")]
    [SerializeField]
    [Tooltip("The minimum distance from the spawn point the object can spawn.")]
    private float _minimumRadius;
    [SerializeField]
    [Tooltip("The maximum distance from the spawn point the object can spawn.")]
    private float _maximumRadius;
    [SerializeField]
    [Tooltip("The height at which the object is spawned.")]
    private float _height;
    [SerializeField]
    [Tooltip("Whether or not the object uses the nav mesh.")]
    private bool _isNavMeshAgent;

    public override SpawnInfo SpawnInfo => _spawnInfo;

    private void Start()
    {
        foreach (SpawnManager manager in _spawnManagers)
            _spawnInfo.AddToManager(manager);
    }

    public override void Spawn(Vector3 position)
    {
        float radius = Random.Range(_minimumRadius, _maximumRadius);
        float theta = Random.Range(0, 2 * Mathf.PI);
        position += new Vector3(radius, theta).CylindricalToCartesian();
        position.y = GroundFinder.HeightOfGround(position) + _height;
        if (_isNavMeshAgent
            && NavMesh.SamplePosition(position, out NavMeshHit hit, Mathf.Infinity, NavMesh.AllAreas))
            position = hit.position;

        Instantiate(_prefab, position, Quaternion.identity);
    }
}