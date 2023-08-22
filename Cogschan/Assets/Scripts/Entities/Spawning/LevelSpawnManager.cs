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

    protected override Vector3 GetSpawnLocation => ContinuousDistributions.GetRandomPointInBox(
        _minX + transform.position.x,
        _maxX + transform.position.x,
        _minY + transform.position.y,
        _maxY + transform.position.y,
        _minZ + transform.position.z,
        _maxZ + transform.position.z);
}