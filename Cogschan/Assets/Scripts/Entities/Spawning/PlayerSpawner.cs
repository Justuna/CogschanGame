using UnityEngine;

public class PlayerSpawner : Spawner
{
    [SerializeField]
    [Tooltip("The prefab of the player.")]
    private GameObject _playerPrefab;
    [SerializeField]
    [Tooltip("The object in charge of spawning the player.")]
    private PlayerSpawnManager _spawnManager;
    private SpawnInfo _playerSpawn;

    public override SpawnInfo SpawnInfo => _playerSpawn;

    private void Start()
    {
        _playerSpawn = new(this, 1, 1, SpawnInfo.SpawnCategory.Player);
        _playerSpawn.AddToManager(_spawnManager);
    }

    public override void Spawn(Vector3 position) => Instantiate(_playerPrefab, position, Quaternion.identity);
}