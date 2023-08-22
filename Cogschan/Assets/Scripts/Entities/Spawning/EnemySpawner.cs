using UnityEngine;
using UnityEngine.AI;

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
    [Tooltip("The height at which the object is spawned.")]
    private float _height;

    public override SpawnInfo SpawnInfo => _spawnInfo;
    public SpawnManager[] SpawnManagers { get => _spawnManagers; set => _spawnManagers = value; }

    private void Start()
    {
        _spawnInfo.Spawner = this;
        foreach (SpawnManager manager in _spawnManagers)
            _spawnInfo.AddToManager(manager);
    }

    public override void Spawn(Vector3 position)
    {
        position.y = GroundFinder.HeightOfGround(position);
        if (NavMesh.SamplePosition(position, out NavMeshHit hit, Mathf.Infinity, NavMesh.AllAreas))
            position = hit.position;
        position += Vector3.up * _height;

        Instantiate(_prefab, position, Quaternion.identity);
    }
}