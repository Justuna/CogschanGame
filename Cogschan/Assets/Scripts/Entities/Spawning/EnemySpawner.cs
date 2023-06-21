using UnityEngine;

public class EnemySpawner : Spawner
{
    [SerializeField]
    [Tooltip("The prefab to be spawned.")]
    private GameObject _prefab;
    [SerializeField]
    [Tooltip("The information for the spawn this spawner controls.")]
    private SpawnInfo _spawnInfo;
    [SerializeField]
    [Tooltip("A list of spawn managers to add the spawn card to.")]
    private SpawnManager[] _spawnManagers;

    public override SpawnInfo SpawnInfo => _spawnInfo;

    private void Start()
    {
        foreach (SpawnManager manager in _spawnManagers)
            _spawnInfo.AddToManager(manager);
    }

    public override void Spawn(Vector3 position)
    {
        Instantiate(_prefab, position, Quaternion.identity);
    }
}