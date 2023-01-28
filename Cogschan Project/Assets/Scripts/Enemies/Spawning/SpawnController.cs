using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnController : MonoBehaviour
{
    public float DifficultyScale = 1.0f;
    public float StartingDifficulty = 0.0f;
    public float MaxDifficulty = 100.0f;

    public float MainSpawnTimerEarliest = 20.0f;
    public float MainSpawnTimerLatest = 30.0f;

    public float ExtraSpawnTimerEarliest = 25.0f;
    public float ExtraSpawnTimerLatest = 50.0f;

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
    }

    void SpawnExtraWave()
    {
        Debug.Log("Spawning Extra Wave...");
    }
}
