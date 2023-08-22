using UnityEngine;

/// <summary>
/// A <see cref="SpawnManager"/> that randomly moves every spawn event.
/// </summary>
public class MovingSpawnManager : SpawnManager
{
    [Header("Position parameters.")]
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

    private void Start()
    {
        Debug.LogWarning("Do not use this script. It will soon be deleted.");
    }
    protected override void ResetSpawnInterval()
    {
        base.ResetSpawnInterval();
        transform.position = ContinuousDistributions.GetRandomPointInBox(_minX, _maxX, _minY, _maxY, _minZ, _maxZ);
    }
}