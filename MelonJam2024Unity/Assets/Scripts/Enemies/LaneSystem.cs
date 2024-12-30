using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

[DefaultExecutionOrder(-50)]
public class LaneSystem : MonoBehaviour
{
    [Serializable]
    public struct EnemyPrefabProbabilities
    {
        public float m_spawnProbability;
        public Enemy m_enemyPrefab;
    }

    public static LaneSystem Instance { get; private set; }

    [SerializeField, Tooltip("For debugging"), Header("Debugging")] public bool _spawnEnemies = true;
    [SerializeField, Tooltip("Scene to load when enemy reaches left side")] private GameManager.GAMESCENE _upgradeScene = GameManager.GAMESCENE.UPGRADE;

    [Header("Main")]
    [SerializeField, Range(1f, 20f)] private float _minSpawnDelay = 5f;
    [SerializeField, Range(1f, 20f)] private float _maxSpawnDelay = 10f;
    [SerializeField, Range(1f, 20f)] private float _initialSpawnDelay = 10f;
    
    [Header("Difficultly")]
    [SerializeField, Range(0f, 1f)] private float _spawnDelayReduction = 0.95f;
    [SerializeField, Range(0f, 20f)] private float _spawnDelayReductionInterval = 5f;
    
    [SerializeField, Range(0f, 20f)] private float _reductionMin = 1f;
    [SerializeField, Range(0f, 20f)] private float _reductionMax = 3f;
    private float _currentSpawnDelay = 0.75f;
    private float _timeSinceLastSpawn = 0f;
    private float _timeSinceReduction = 0f;

    [SerializeField] public Sprite m_onLaneSprite;

    /// <summary>
    /// Lanes from top to bottom
    /// </summary>
    [SerializeField, Tooltip("From top to bottom auto sorted")] public List<Lane> m_lanes = new();
    [SerializeField] private List<EnemyPrefabProbabilities> _enemyPrefabList = new();

    //[SerializeField] public UnityEvent m_onLooseCondition;
    //[SerializeField] public UnityEvent<int> m_onEnemyDied;

    [SerializeField, Header("Upgrades")] public bool m_landMinesActive = false;
    [SerializeField, Range(0, 1)] private float _slowUpgradeMultiplierReduction = 0.1f;
    /// <summary>
    /// How much percent the enemies get slowed down
    /// </summary>
    public float m_slowUpgradeMultiplierReduction // Stupid setter to also be able to update it in the inspector
    {
        get { return _slowUpgradeMultiplierReduction; }
        set { _slowUpgradeMultiplierReduction = Mathf.Clamp01(value); }
    }

    public void LoadUpgradeScene()
    {
        GameManager.LoadScene(_upgradeScene);
    }

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(this.gameObject);
            return;
        }
        Instance = this;
        //DontDestroyOnLoad(this);
    }

    private void OnDestroy()
    {
        Instance = null;
    }

    private void Start()
    {
        if (m_lanes is null || m_lanes.Count == 0) m_lanes = GetComponentsInChildren<Lane>().ToList();
        m_lanes = m_lanes.OrderByDescending(y => y.m_endPoint.position.y).ToList();

        _currentSpawnDelay = UnityEngine.Random.Range(_minSpawnDelay, _maxSpawnDelay) + _initialSpawnDelay;
    }

    private void Update()
    {
        if (!_spawnEnemies) { return; }

        _timeSinceLastSpawn += Time.deltaTime;
        _timeSinceReduction += Time.deltaTime;

        if (_timeSinceLastSpawn > _currentSpawnDelay)
        {
            _timeSinceLastSpawn = 0f;
            _currentSpawnDelay = UnityEngine.Random.Range(_minSpawnDelay, _maxSpawnDelay);

            SpawnEnemy();
        }
        
        if (_timeSinceReduction > _spawnDelayReductionInterval)
        {
            _timeSinceReduction = 0f;
            _minSpawnDelay = Math.Max(_reductionMin, _minSpawnDelay * _spawnDelayReduction);
            _maxSpawnDelay = Math.Max(_reductionMax, _maxSpawnDelay * _spawnDelayReduction);
        }
    }

    public void UpdateLaneIndicator(int newlyActiveLane)
    {
        if (newlyActiveLane != -1)
        {
            m_lanes[newlyActiveLane].SetLaneIndicator(m_onLaneSprite);
        }
        m_lanes.Where((lane, index) => index != newlyActiveLane).ToList().ForEach(x => x.SetLaneIndicator(false));
    }

    [ContextMenu("Spawn random enemy")]
    public void SpawnEnemy(Enemy enemyPrefab = null, Lane lane = null)
    {
        enemyPrefab ??= GetRandomEnemy();
        int newLane = UnityEngine.Random.Range(0, m_lanes.Count);
        lane ??= m_lanes[newLane];

        lane.SpawnEnemy(enemyPrefab, m_slowUpgradeMultiplierReduction, newLane + 10); 
    }

    private Enemy GetRandomEnemy()
    {
        float randomNumber = UnityEngine.Random.Range(0, _enemyPrefabList.Sum(probability => probability.m_spawnProbability));
        float cumulative = 0f;
        foreach (EnemyPrefabProbabilities epp in _enemyPrefabList)
        {
            cumulative += epp.m_spawnProbability;
            if (randomNumber <= cumulative)
            {
                return epp.m_enemyPrefab;
            }
        }
        return _enemyPrefabList[0].m_enemyPrefab;
    }
}
