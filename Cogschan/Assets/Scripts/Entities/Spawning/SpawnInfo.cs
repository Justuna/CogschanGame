using System;
using UnityEngine;

/// <summary>
/// Contains the information required to spawn an object.
/// </summary>
public readonly struct SpawnInfo
{
    /// <summary>
    /// The <see cref="global::Spawner"/> used to spawn the associated object.
    /// </summary>
    public readonly Spawner Spawner;
    /// <summary>
    /// The cost of the object.
    /// </summary>
    public readonly float Cost;
    /// <summary>
    /// The relative frequency of this spawn being selected.
    /// </summary>
    /// <remarks>
    /// The weights of all objects in the collection does not have to equal one; rather, the weight will be scaled by the total weight in the selection algorithm.
    /// </remarks>
    public readonly float Weight;
    /// <summary>
    /// The category of the spawned object.
    /// </summary>
    public readonly SpawnCategory Category;

    /// <summary>
    /// Constructor for <see cref="SpawnInfo"/>.
    /// </summary>
    /// <param name="spawner"> The <see cref="global::Spawner"/> used to spawn the associated object.</param>
    /// <param name="cost"> The cost of the object. Must be positive. </param>
    /// <param name="weight"> The relative frequency of this spawn being selected. Must be nonnegative. </param>
    /// <exception cref="ArgumentOutOfRangeException"> Thrown if the parameters do not meet the stated preconditions.</exception>
    public SpawnInfo(Spawner spawner, float cost, float weight, SpawnCategory category)
    {
        if (cost <= 0)
            throw new ArgumentOutOfRangeException(nameof(cost), "The cost must be positive.");
        if (weight < 0)
            throw new ArgumentOutOfRangeException(nameof(weight), "The weight must be nonnegative.");

        Spawner = spawner;
        Cost = cost;
        Weight = weight;
        Category = category;
    }

    /// <summary>
    /// Returns a new <see cref="SpawnInfo"/> with a different weight.
    /// </summary>
    /// <param name="newWeight"> The new weight of the object. Must be nonnegative.</param>
    /// <exception cref="ArgumentOutOfRangeException"> Thrown if the parameters do not meet the stated preconditions.</exception>
    public SpawnInfo OverrideWeight(float newWeight)
    {
        if (newWeight < 0)
            throw new ArgumentOutOfRangeException(nameof(newWeight), "The new weight must be nonnegative.");
        return new(Spawner, Cost, newWeight, Category);
    }

    /// <summary>
    /// Returns whether or not the object is currently purchasable.
    /// </summary>
    /// <param name="budget"> The number of availible credits.</param>
    /// <returns></returns>
    public bool IsPurchasable(float budget) => Cost <= budget && Weight != 0;

    /// <summary>
    /// Adds the spawn info to the <paramref name="manager"/>.
    /// </summary>
    public void AddToManager(SpawnManager manager)
    {
        manager.Spawns.Add(this);
    }

    public void Spawn(Vector3 position) => Spawner.Spawn(position);

    /// <summary>
    /// The possible categories the spawn info can have.
    /// </summary>
    public enum SpawnCategory
    {
        Player,
    }
}