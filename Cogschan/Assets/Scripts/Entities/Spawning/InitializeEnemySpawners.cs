using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

[ExecuteAlways]
public class InitializeEnemySpawners : MonoBehaviour
{
    [SerializeField]
    private SpawnManager[] spawnManagers;

    private void OnValidate()
    {
        if (Application.isPlaying) return;

        Undo.RecordObjects(GetComponentsInChildren<EnemySpawner>(), "Update spawners' managers");
        foreach (var spawner in GetComponentsInChildren<EnemySpawner>())
        {
            var newSpawnManagerList = new List<SpawnManager>(spawner.SpawnManagers);
            foreach (var spawnManager in spawnManagers)
            {
                if (spawnManager == null) continue;
                if (!spawner.SpawnManagers.Contains(spawnManager))
                    newSpawnManagerList.Add(spawnManager);
            }

            if (newSpawnManagerList.Count > spawner.SpawnManagers.Length)
                spawner.SpawnManagers = newSpawnManagerList.ToArray();
        }
    }
}
