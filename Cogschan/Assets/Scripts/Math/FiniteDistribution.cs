using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

/// <summary>
/// Models a distribution with a finite sample space.
/// </summary>
/// <typeparam name="T"> The type of each element of the sample space. </typeparam>
public readonly struct FiniteDistribution<T>
{
    private readonly ReadOnlyCollection<T> _sampleSpace;
    private readonly ReadOnlyCollection<float> _weights;
    private readonly static Random s_random = new();

    /// <summary>
    /// The amount of possibilities in the sample space.
    /// </summary>
    public int SampleSpaceCount => _sampleSpace.Count;

    /// <summary>
    /// Constructor for <see cref="FiniteDistribution{T}"/>
    /// </summary>
    /// <param name="sampleSpaceAndWeights">Dictionary that maps categories to weight values.</param>
    public FiniteDistribution(IDictionary<T, float> sampleSpaceAndWeights)
    {
        _sampleSpace = new(sampleSpaceAndWeights.Keys.ToArray());
        _weights = new(sampleSpaceAndWeights.Values.ToArray());
        foreach (float weight in _weights)
        {
            if (weight <= 0)
                throw new ArgumentException("All weights must be positive.", nameof(sampleSpaceAndWeights));
        }
    }

    /// <summary>
    /// Constructor for <see cref="FiniteDistribution{T}"/>
    /// </summary>
    /// <param name="sampleSpace"> The sample space of the distribution.</param>
    /// <param name="weights"> The weight of the corresponding sample. </param>
    /// <exception cref="ArgumentException">Thrown if the lists aren't of the same size.</exception>
    public FiniteDistribution(IList<T> sampleSpace, IList<float> weights)
    {
        _sampleSpace = new(sampleSpace);
        _weights = new(weights);
        if (sampleSpace.Count != weights.Count)
            throw new ArgumentException("Both lists must have the same number of elements.");
        if (sampleSpace.Count == 0)
            throw new ArgumentException("The lists must be nonempty");
        foreach (float weight in weights)
        {
            if (weight <= 0)
                throw new ArgumentException("All weights must be positive.", nameof(weights));
        }
    }

    /// <summary>
    /// Returns a random value from the distribution based on the weights.
    /// </summary>
    /// <returns></returns>
    public T GetRandomValue()
    {
        float totalWeight = 0;
        foreach (float weight in _weights)
            totalWeight += weight;
        float randomValue = totalWeight * (float)s_random.NextDouble();
        int i = 0;
        foreach (float weight in _weights)
        {
            randomValue -= weight;
            if (randomValue < 0)
                break;
            i++;
        }
        return _sampleSpace[i];
    }

    /// <summary>
    /// Returns whether or not the given <paramref name="element"/> is a member of the sample space.
    /// </summary>
    public bool IsInSampleSpace(T element) => _sampleSpace.Contains(element);

    /// <summary>
    /// Returns a new <see cref="FiniteDistribution{T}"/> which one element removed.
    /// </summary>
    /// <exception cref="ArgumentException"> Thrown when the given <paramref name="element"/> is not in the sample space.</exception>
    /// <exception cref="InvalidOperationException"> Thrown when removing the element would result in an empty sample space. </exception>
    public FiniteDistribution<T> Ignore(T element)
    {
        int index = _sampleSpace.IndexOf(element);
        if (index == -1)
            throw new ArgumentException("The given element is not in the sample space", nameof(element));
        if (SampleSpaceCount == 1)
            throw new InvalidOperationException("The resulting distribution would have an empty sample space.");


        List<T> newSampleSpace = _sampleSpace.ToList();
        newSampleSpace.Remove(element);
        List<float> newWeights = _weights.ToList();
        newWeights.RemoveAt(index);

        return new(newSampleSpace, newWeights);
    }
}