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

    public float AuxSpawnTimerEarliest = 25.0f;
    public float AuxSpawnTimerLatest = 50.0f;

    [Header("General Spawning Variables")]
    public Transform Player;
    public int MaxAttempts = 100;

    [Header("Main Wave Variables")]
    public EnemyPatternGroup MainWaveGroup;
    public float MainWaveMinDistance = 10;
    public float MainWaveMaxDistance = 20;
    public float MainWaveMinSpread = 0.5f;
    public float MainWaveMaxNavmeshRaycastDistance = 20;
    public float Lopsidedness = 2;

    [Header("Auxiliary Wave Variables")]
    public EnemyPatternGroup AuxiliaryGroup;
    public float AuxWaveCenterMinDistance = 10;
    public float AuxWaveCenterMaxDistance = 20;
    public float AuxWaveMinDistance = 10;
    public float AuxWaveMaxDistance = 20;
    public float AuxWaveMinSpread = 0.5f;
    public float AuxWaveMaxNavmeshRaycastDistance = 20;

    [Header("Zenith Variables")]
    [SerializeField]
    private float _zenithTime;
    private float _zenithTimer;
    public bool IsZenith { get; private set; }

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
        _difficulty += Time.deltaTime * 1 / 60 * DifficultyScale;
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

        // Zenith timer ticks up to begin Zenith, which is one of the requirements for the boss spawning.
        if (_zenithTimer < _zenithTime)
            _zenithTimer += Time.deltaTime;
        if (_zenithTimer >= _zenithTime)
            IsZenith = true;
    }

    void SetMainSpawnTimer()
    {
        _mainSpawnTimer = Random.Range(MainSpawnTimerEarliest, MainSpawnTimerLatest);
    }

    void SetExtraSpawnTimer()
    {
        _extraSpawnTimer = Random.Range(AuxSpawnTimerEarliest, AuxSpawnTimerLatest);
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

                    float dist = Mathf.Pow(Random.Range(0, 1.0f), Lopsidedness) * MainWaveMaxDistance + MainWaveMinDistance;
                    float angle = Random.Range(0, 360.0f);

                    Vector3 displacement = Quaternion.Euler(0, angle, 0) * Vector3.forward * dist;

                    NavMesh.SamplePosition(Player.position + displacement, out hit, MainWaveMaxNavmeshRaycastDistance, NavMesh.AllAreas);
                    if (hit.position == null) continue;
                    foreach (Vector3 point in spawns)
                    {
                        if (Vector3.Distance(point, hit.position) < MainWaveMinSpread)
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

        Dictionary<GameObject, int> dict = AuxiliaryGroup.PickPattern(_difficulty);
        List<Vector3> spawns = new List<Vector3>();

        NavMeshHit centerHit = new NavMeshHit();
        float centerDist = Random.Range(0, 1.0f) * AuxWaveCenterMaxDistance + AuxWaveCenterMinDistance;
        float centerAngle = Random.Range(0, 360.0f);

        NavMesh.SamplePosition(Quaternion.Euler(0, centerAngle, 0) * Vector3.forward * centerDist + Player.position, out centerHit, Mathf.Infinity, NavMesh.AllAreas); ;
        Vector3 epicenter = centerHit.position;

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

                    float dist = Random.Range(0, 1.0f) * AuxWaveMaxDistance + AuxWaveMinDistance;
                    float angle = Random.Range(0, 360.0f);

                    Vector3 displacement = Quaternion.Euler(0, angle, 0) * Vector3.forward * dist;

                    NavMesh.SamplePosition(epicenter + displacement, out hit, AuxWaveMaxNavmeshRaycastDistance, NavMesh.AllAreas);
                    if (hit.position == null) continue;
                    foreach (Vector3 point in spawns)
                    {
                        if (Vector3.Distance(point, hit.position) < AuxWaveMinSpread)
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
}
