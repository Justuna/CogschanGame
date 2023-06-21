using System;
using System.Collections.Generic;
using UnityEngine;

using static SpawnInfo;

/// <summary>
/// The manager that handles spawning of all relevant objects.
/// </summary>
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
    [Tooltip("The weighting of the categories. Must contain at least one non-zero value and no negative values.")]
    private float[] _categoryWeights;

    [Header("Credit parameters")]
    [SerializeField]
    [Tooltip("The initial amount of credits. Must be nonnegative.")]
    private float _credits = 0;
    [Header("Credit Parameters")]
    [SerializeField]
    [Tooltip("The rate at which credits are initially added to the Spawn Manager. Must be nonnegative.")]
    private float _creditAddRate;
    [Tooltip("The rate at which the rate at which credits are added to the Spawn Manager increases. Essentially the second derivative of credits. Must be nonnegative.")]
    private readonly float _creditAddRateRate;

    private FiniteDistribution<SpawnCategory> _categoryDist;
    private float _spawnTimer = 0;
    private float _spawnInterval;
    private bool _isFirstSpawn = true;

    private void Start()
    {
        _categoryDist = new((SpawnCategory[])Enum.GetValues(typeof(SpawnCategory)), _categoryWeights);
        ResetSpawnInterval();
    }

    private void Update()
    {
        _credits += _creditAddRate * Time.deltaTime;
        _creditAddRate += _creditAddRateRate * Time.deltaTime;
        _spawnTimer += Time.deltaTime;

        if (_spawnTimer >= _spawnInterval)
        {
            while (Spawn()) { }
            ResetSpawnInterval();
        }
    }

    /// <summary>
    /// The method called for the first spawn event.
    /// </summary>
    /// <returns> Whether or not the spawn was successful. </returns>
    protected virtual bool FirstSpawn()
    {
        return Spawn();
    }

    /// <summary>
    /// Randomize the spawn interaval and reset the timer.
    /// </summary>
    protected virtual void ResetSpawnInterval()
    {
        _spawnInterval = UnityEngine.Random.Range(_minSpawnInterval, _maxSpawnInterval);
        _spawnTimer = 0;
    }

    // Tries to spawn an object at the location of the manager.
    private bool Spawn()
    {
        // If the first spawn event, run that method instead.
        if (_isFirstSpawn)
        {
            _isFirstSpawn = false;
            return FirstSpawn();
        }
        
        // Select a random category and verify that there are valid spawns.
        FiniteDistribution<SpawnCategory> dist = _categoryDist;
        List<SpawnInfo> spawnsInCat = new();
        List<float> spawnWeights = new();
        bool spawnPossible = false;
        while (true)
        {
            SpawnCategory category = dist.GetRandomValue();
            foreach (SpawnInfo spawn in Spawns)
            {
                if (spawn.IsPurchasable(_credits) && spawn.Category == category)
                {
                    spawnsInCat.Add(spawn);
                    spawnWeights.Add(spawn.Weight);
                    spawnPossible = true;
                }
            }
            if (spawnPossible)
                break;
            if (dist.SampleSpaceCount == 1)
                return false; // No more valid categories.
            dist = dist.Ignore(category);
            spawnsInCat = new();
            spawnWeights = new();
        }

        // Spawn an object at random.
        SpawnInfo selectedSpawn = new FiniteDistribution<SpawnInfo>(spawnsInCat, spawnWeights).GetRandomValue();
        _credits -= selectedSpawn.Cost;
        selectedSpawn.Spawner.Spawn(transform.position);
        return true;
    }
}