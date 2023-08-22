using UnityEngine;

/// <summary>
/// A spawn manager for the entire level
/// </summary>
public class LevelSpawnManager : SpawnManager
{
    [Header("Level bounds")]
    [SerializeField]
    [Tooltip("The minimum x-value of the randomized position.")]
    private float _minX;
    [SerializeField]
    [Tooltip("The maximum x-value of the randomized position.")]
    private float _maxX;
    [SerializeField]
    [Tooltip("The minimum y-value of the randomized position.")]
    private float _minY;
    [SerializeField]
    [Tooltip("The maximum y-value of the randomized position.")]
    private float _maxY;
    [SerializeField]
    [Tooltip("The minimum z-value of the randomized position.")]
    private float _minZ;
    [SerializeField]
    [Tooltip("The maximum z-value of the randomized position.")]
    private float _maxZ;

    [Header("Enemy group parameters")]
    [SerializeField]
    [Tooltip("The minimum amount of enemies spawned in a group.")]
    private int _minGroupSize;
    [SerializeField]
    [Tooltip("The maximum amount of enemies spawned in a group.")]
    private int _maxGroupSize;
    [SerializeField]
    [Tooltip("The distance from the group center that enemise can spawn.")]
    private float _radius;

    private int _groupSize;
    private int _spawnCount = 0;
    private Vector3 _groupLocation;

    protected override Vector3 GetSpawnLocation
    {
        get
        {
            if (_spawnCount == 0)
            {
                _groupSize = Random.Range(_minGroupSize, _maxGroupSize + 1);
                _groupLocation = ContinuousDistributions.GetRandomPointInBox(
                    _minX + transform.position.x,
                    _maxX + transform.position.x,
                    _minY + transform.position.y,
                    _maxY + transform.position.y,
                    _minZ + transform.position.z,
                    _maxZ + transform.position.z);
            }
            _spawnCount++;
            if (_spawnCount >= _groupSize)
                _spawnCount = 0;
            return ContinuousDistributions.GetRandomPointInAnnulus(0, _radius, _groupLocation);
        }
    }
}