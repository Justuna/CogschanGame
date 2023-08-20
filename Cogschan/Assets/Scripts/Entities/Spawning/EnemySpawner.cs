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

    private static int s_enemyCount = 0;

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
        // If there are already a lot of enemies, don't spawn more.
        if (s_enemyCount >= GameStateSingleton.Instance.MaxEnemies)
        {
            print("too many enemies!!");
            return;
        }

        print("spawning!");

        position = ContinuousDistributions.GetRandomPointInAnnulus(_minimumRadius, _maximumRadius, position);
        position.y = GroundFinder.HeightOfGround(position) + _height;
        if (_isNavMeshAgent
            && NavMesh.SamplePosition(position, out NavMeshHit hit, Mathf.Infinity, NavMesh.AllAreas))
            position = hit.position;

        GameObject enemy = Instantiate(_prefab, position, Quaternion.identity);
        s_enemyCount++;
        enemy.GetComponent<EntityServiceLocator>().HealthTracker.OnDefeat.AddListener(() => { s_enemyCount--; });
    }
}