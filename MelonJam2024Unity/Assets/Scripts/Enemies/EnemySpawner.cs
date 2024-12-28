using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    //public static EnemySpawner Instance { get; private set; }

    [SerializeField] private float _minSpawnDelay = 0.5f;
    [SerializeField] private float _maxSpawnDelay = 1.0f;
    private float _currentSpawnDelay = 0.75f;
    private float _timeSinceLastSpawn = 0f;

    [SerializeField] private List<Lane> _lanes = new();
    [SerializeField] private List<Enemy> _enemyPrefabList = new();

    private void Awake()
    {
        //if (Instance != null)
        //{
        //    Destroy(this.gameObject);
        //    return;
        //}
        //Instance = this;
        //DontDestroyOnLoad(this);
    }

    private void Start()
    {
        if (_lanes is null || _lanes.Count == 0) _lanes = GetComponentsInChildren<Lane>().ToList();
        _currentSpawnDelay = UnityEngine.Random.Range(_minSpawnDelay, _maxSpawnDelay);
    }

    private void Update()
    {
        _timeSinceLastSpawn += Time.deltaTime;

        if (_timeSinceLastSpawn > _currentSpawnDelay)
        {
            _timeSinceLastSpawn = 0f;
            _currentSpawnDelay = UnityEngine.Random.Range(_minSpawnDelay, _maxSpawnDelay);

            SpawnEnemy();
        }
    }

    [ContextMenu("SpawnEnemy")]
    [Obsolete("Only for the inspector")]
    public void SpawnOneRandomEnemy()
    {
        if (Application.isPlaying)
        {
            SpawnEnemy();
        }
    }

    public void SpawnEnemy(Enemy enemyPrefab = null, Lane lane = null)
    {
        enemyPrefab ??= _enemyPrefabList[UnityEngine.Random.Range(0, _enemyPrefabList.Count)];
        lane ??= _lanes[UnityEngine.Random.Range(0, _lanes.Count)];

        lane.SpawnEnemy(enemyPrefab);
    }
}
