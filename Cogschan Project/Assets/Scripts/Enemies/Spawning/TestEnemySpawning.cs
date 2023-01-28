using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestEnemySpawning : MonoBehaviour
{
    public EnemyPatternGroup group;
    public float difficulty = 0;

    void Start()
    {
        Dictionary<GameObject, int> dict = group.PickPattern(difficulty);

        foreach (KeyValuePair<GameObject, int> kvp in dict)
        {
            Debug.Log("At a difficulty of " + difficulty + ", spawning " + kvp.Value + " " + kvp.Key.name + "(s).");
        }
    }
}
