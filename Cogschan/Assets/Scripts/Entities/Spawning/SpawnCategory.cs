using System;
using System.Collections.Generic;
using System.Linq;

/// <summary>
/// A category containing <see cref="SpawnInfo"/> instances.
/// </summary>
public class SpawnCategory
{
    /// <summary>
    /// A set of <see cref="SpawnInfo"/> instances to be selected from.
    /// </summary>
    public readonly HashSet<SpawnInfo> Spawns;

    private float _weight;


    /// <summary>
    /// The relative frequency of this category.
    /// </summary>
    /// <remarks>
    /// The weights of all objects in the collection does not have to equal one; rather, the weight will be scaled by the total weight in the selection algorithm.
    /// </remarks>
    /// <exception cref="ArgumentOutOfRangeException"> Thrown in the setter if the value is negative. </exception>
    public float Weight
    {
        get => _weight;
        set
        {
            if (value < 0)
                throw new ArgumentOutOfRangeException("value", "The weight must be nonnegative.");
            _weight = value;
        }
    }

    /// <summary>
    /// Constructor for <see cref="SpawnCategory"/>.
    /// </summary>
    public SpawnCategory(IEnumerable<SpawnInfo> spawns, float weight)
    {
        Spawns = spawns.ToHashSet();
        Weight = weight;
    }


    /// <summary>
    /// Returns whether or not this category contains objects that can be spawned.
    /// </summary>
    /// <param name="budget"> The amount of credits that can be spent.</param>
    /// <returns></returns>
    public bool ContainsValidSpawns(float budget)
    {
        foreach (SpawnInfo info in Spawns)
        {
            if (info.IsPurchasable(budget))
                return true;
        }
        return false;
    }


    /// <summary>
    /// Selects a <see cref="SpawnInfo"/> at random. Requires that <see cref="ContainsValidSpawns"/> is true.
    /// </summary>
    /// <param name="budget"> The amount of credits that can be spent.</param>
    public SpawnInfo SelectSpawnInfo(float budget)
    {
        float totalWeight = 0;
        List<(SpawnInfo, float)> boundaries = new();
        foreach (SpawnInfo info in Spawns)
        {
            if (!info.IsPurchasable(budget))
                continue;
            totalWeight += info.Weight;
            boundaries.Add((info, totalWeight));
        }
        if (totalWeight == 0)
            throw new InvalidOperationException("There are no spawns possible");
        float rand = UnityEngine.Random.Range(0, totalWeight);

        foreach ((SpawnInfo, float) info in boundaries)
        {
            if (info.Item2 >= rand)
                return info.Item1;
        }
        return boundaries[-1].Item1;
    }
}