using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Enemies/New Enemy Group")]
public class EnemyPatternGroup : ScriptableObject
{
    [Serializable]
    public struct WeightedEnemyPattern{
        public EnemyPattern Pattern;
        public float Weight;
    }

    public WeightedEnemyPattern[] Patterns;

    public Dictionary<GameObject, int> PickPattern(float difficulty)
    {
        float sum = 0;
        foreach (WeightedEnemyPattern p in Patterns)
        {
            if (p.Pattern.MaxDifficulty < difficulty || p.Pattern.StartDifficulty > difficulty) continue;
            sum += p.Weight;
        }
        Debug.Log(sum);

        float value = UnityEngine.Random.Range(0, sum);
        Debug.Log(value);
        float counter = 0;
        foreach (WeightedEnemyPattern p in Patterns)
        {
            if (p.Pattern.MaxDifficulty < difficulty || p.Pattern.StartDifficulty > difficulty) continue;

            counter += p.Weight;
            if (value <= counter)
            {
                return p.Pattern.EvaluatePattern(difficulty);
            }
        }

        Debug.Log("Something went wrong...");
        return null;
    }
}
