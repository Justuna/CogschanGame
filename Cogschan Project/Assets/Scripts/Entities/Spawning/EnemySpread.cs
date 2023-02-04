using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Enemies/New Enemy Spread")]
public class EnemySpread : ScriptableObject
{
    public GameObject EnemyPrefab;

    public AnimationCurve DifficultyRamp;
    public float StartDifficulty;
    public float MaxDifficulty;
    public float Noise;
    public int MinCount;
    public int MaxCount;

    public int EvaluateEnemyCount(float difficulty)
    {
        float noise = Random.Range(-1.0f, 1.0f) * Noise;
        float t = Mathf.Clamp((difficulty - StartDifficulty) / (MaxDifficulty - StartDifficulty) + noise, 0, 1.0f);
        int n = MinCount + Mathf.CeilToInt(DifficultyRamp.Evaluate(t) * (MaxCount - MinCount));
        return n;
    }
}
