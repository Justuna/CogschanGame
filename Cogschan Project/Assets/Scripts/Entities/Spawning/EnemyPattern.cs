using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Enemies/New Enemy Pattern")]
public class EnemyPattern : ScriptableObject
{
    public List<EnemySpread> EnemyTypes;
    public float StartDifficulty;
    public float MaxDifficulty;

    public Dictionary<GameObject, int> EvaluatePattern(float difficulty)
    {
        Dictionary<GameObject, int> dict = new Dictionary<GameObject, int>();
        foreach (EnemySpread s in EnemyTypes)
        {
            int n = s.EvaluateEnemyCount(difficulty);
            if (dict.ContainsKey(s.EnemyPrefab))
            {
                dict[s.EnemyPrefab] += n;
            }
            else
            {
                dict.Add(s.EnemyPrefab, n);
            }
        }

        return dict;
    }
}
