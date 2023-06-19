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
    /// The condition that must be met for this spawn to be selected.
    /// </summary>
    public readonly Func<bool> Condition;

    /// <summary>
    /// Constructor for <see cref="SpawnInfo"/>.
    /// </summary>
    /// <param name="spawner"> The <see cref="global::Spawner"/> used to spawn the associated object.</param>
    /// <param name="cost"> The cost of the object. Must be positive. </param>
    /// <param name="weight"> The relative frequency of this spawn being selected. Must be nonnegative. </param>
    /// <param name="condition"> The condition that must be met for this spawn to be selected. </param>
    /// <exception cref="ArgumentOutOfRangeException"> Thrown if the parameters do not meet the stated preconditions.</exception>
    public SpawnInfo(Spawner spawner, float cost, float weight, Func<bool> condition)
    {
        if (cost <= 0)
            throw new ArgumentOutOfRangeException(nameof(cost), "The cost must be positive.");
        if (weight < 0)
            throw new ArgumentOutOfRangeException(nameof(weight), "The weight must be nonnegative.");

        Spawner = spawner;
        Cost = cost;
        Weight = weight;
        Condition = condition;
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
        return new(Spawner, Cost, newWeight, Condition);
    }

    /// <summary>
    /// Returns whether or not the object is currently purchasable.
    /// </summary>
    /// <param name="budget"> The number of availible credits.</param>
    /// <returns></returns>
    public bool IsPurchasable(float budget) => Cost <= budget && Condition() && Weight != 0;
}