using System.Collections.Generic;
using UnityEngine;

// The manager that handles spawning of all relevant objects.
public class SpawnManager : MonoBehaviour
{
    /// <summary>
    /// The set of <see cref="SpawnInfo"/>s the manager chooses from.
    /// </summary>
    public readonly HashSet<SpawnInfo> Spawns = new();

    [Header("Spawn parameters")]
    [SerializeField]
    [Tooltip("The minimum amount of time between spawns. Must be positive.")]
    private float _minSpawnInterval;
    [SerializeField]
    [Tooltip("The maximum amount of time between spawns. Must be at least the minimum spawn interval.")]
    private float _maxSpawnInterval;


    [SerializeField]
    [Tooltip("The initial amount of credits. Must be nonnegative.")]
    private float _credits = 0;
    [Header("Credit Parameters")]
    [SerializeField]
    [Tooltip("The rate at which credits are initially added to the Spawn Manager. Must be nonnegative.")]
    private float _creditAddRate;
    [Tooltip("The rate at which the rate at which credits are added to the Spawn Manager increases. Essentially the second derivative of credits. Must be nonnegative.")]
    private readonly float _creditAddRateRate;

    private float _spawnTimer = 0;
    private float _spawnInterval;

    private void Start()
    {
        ResetSpawnInterval();
    }

    private void Update()
    {
        _credits += _creditAddRate;
        _creditAddRate += _creditAddRateRate;
        _spawnTimer += Time.deltaTime;

        if (_spawnTimer >= _spawnInterval)
        {
            while (Spawn()) { }
            ResetSpawnInterval();
        }
    }

    // Randomize the spawn interaval and reset the timer..
    private void ResetSpawnInterval()
    {
        _spawnInterval = Random.Range(_minSpawnInterval, _maxSpawnInterval);
        _spawnTimer = 0;
    }

    // Tries to spawn an object at the location of the manager.
    private bool Spawn()
    {
        /*float totalWeight = 0;
        List<(S, float)> boundaries = new();
        foreach (SpawnCategory cat in Categories)
        {
            if (!cat.ContainsValidSpawns(_credits))
                continue;
            totalWeight += cat.Weight;
            boundaries.Add((cat, totalWeight));
        }
        if (totalWeight == 0)
            return false;
        float rand = Random.Range(0, totalWeight);

        SpawnCategory category = boundaries[-1].Item1;
        foreach ((SpawnCategory, float) info in boundaries)
        {
            if (info.Item2 >= rand)
            {
                category = info.Item1;
                break;
            }
        }
        SpawnInfo spawn = category.SelectSpawnInfo(_credits);

        _credits -= spawn.Cost;
        spawn.Spawner.Spawn(transform.position);
        return true;*/
        throw new System.NotImplementedException();
    }
}