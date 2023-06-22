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
    [Tooltip("The minimum z-value of the randomized position.")]
    private float _minZ;
    [SerializeField]
    [Tooltip("The maximum z-value of the randomized position.")]
    private float _maxZ;


    protected override void ResetSpawnInterval()
    {
        base.ResetSpawnInterval();
        transform.position = ContinuousDistributions.GetRandomPointInSquare(_minX, _maxX, _minZ, _maxZ, transform.position.y * Vector3.up);
    }
}