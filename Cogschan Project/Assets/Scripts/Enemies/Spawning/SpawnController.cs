using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class SpawnController : MonoBehaviour
{
    [Header("Difficulty Variables")]
    public float DifficultyScale = 1.0f;
    public float StartingDifficulty = 0.0f;
    public float MaxDifficulty = 100.0f;

    public float MainSpawnTimerEarliest = 20.0f;
    public float MainSpawnTimerLatest = 30.0f;

    public float ExtraSpawnTimerEarliest = 25.0f;
    public float ExtraSpawnTimerLatest = 50.0f;

    [Header("Spawning Variables")]
    public EnemyPatternGroup MainWaveGroup;
    public Transform Player;
    public float MinDistance = 10;
    public float MaxDistance = 20;
    public float MinSpread = 0.5f;
    public float MaxNavmeshRaycastDistance = 20;
    public int MaxAttempts = 100;
    public float Lopsidedness = 2;

    private float _difficulty;
    private float _mainSpawnTimer;
    private float _extraSpawnTimer;

    private void Awake()
    {
        _difficulty = StartingDifficulty;
        SetMainSpawnTimer();
        SetExtraSpawnTimer();
    }

    void Update()
    {
        // Difficulty increases by 1 every minute, by default
        _difficulty += Time.deltaTime * 1/60 * DifficultyScale;
        _difficulty = Mathf.Min(_difficulty, MaxDifficulty);

        // Main spawn timer ticks down regularly to spawn enemy batallions near player
        _mainSpawnTimer -= Time.deltaTime;
        if (_mainSpawnTimer <= 0)
        {
            SetMainSpawnTimer();
            SpawnMainWave();
        }

        // Extra spawn timer ticks down regularly to spawn a couple enemies in far-off locations
        _extraSpawnTimer -= Time.deltaTime;
        if (_extraSpawnTimer <= 0)
        {
            SetExtraSpawnTimer();
            SpawnExtraWave();
        }
    }

    void SetMainSpawnTimer()
    {
        _mainSpawnTimer = Random.Range(MainSpawnTimerEarliest, MainSpawnTimerLatest);
    }

    void SetExtraSpawnTimer()
    {
        _extraSpawnTimer = Random.Range(ExtraSpawnTimerEarliest, ExtraSpawnTimerLatest);
    }

    void SpawnMainWave()
    {
        Debug.Log("Spawning Main Wave...");

        Dictionary<GameObject, int> dict = MainWaveGroup.PickPattern(_difficulty);
        List<Vector3> spawns = new List<Vector3>();

        foreach (KeyValuePair<GameObject, int> kvp in dict)
        {
            Debug.Log("At a difficulty of " + _difficulty + ", spawning " + kvp.Value + " " + kvp.Key.name + "(s).");

            for (int i = 0; i < kvp.Value; i++)
            {
                //Distance is random, but weighted to be closer
                NavMeshHit hit = new NavMeshHit();
                for (int j = 0; j < MaxAttempts; j++)
                {
                    bool legal = true;

                    float dist = Mathf.Pow(Random.Range(0, 1.0f), Lopsidedness) * MaxDistance + MinDistance;
                    float angle = Random.Range(0, 360.0f);

                    Vector3 displacement = Quaternion.Euler(0, angle, 0) * Vector3.forward * dist;

                    NavMesh.SamplePosition(Player.position + displacement, out hit, MaxNavmeshRaycastDistance, NavMesh.AllAreas);
                    if (hit.position == null) continue;
                    foreach (Vector3 point in spawns)
                    {
                        if (Vector3.Distance(point, hit.position) < MinSpread)
                        {
                            legal = false;
                            break;
                        }
                    }

                    if (legal) break;
                }
                spawns.Add(hit.position);

                Instantiate(kvp.Key, hit.position, Quaternion.identity);
            }
        }
    }

    void SpawnExtraWave()
    {
        Debug.Log("Spawning Extra Wave...");
    }
}
