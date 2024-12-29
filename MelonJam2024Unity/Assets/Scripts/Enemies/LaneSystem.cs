using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

[DefaultExecutionOrder(-50)]
public class LaneSystem : MonoBehaviour
{
    public static LaneSystem Instance { get; private set; }

    [SerializeField, Range(1f, 20f)] private float _minSpawnDelay = 5f;
    [SerializeField, Range(1f, 20f)] private float _maxSpawnDelay = 10f;
    [SerializeField, Range(1f, 20f)] private float _initialSpawnDelay = 10f;
    private float _currentSpawnDelay = 0.75f;
    private float _timeSinceLastSpawn = 0f;

    [SerializeField] public Sprite m_onLaneSprite;

    /// <summary>
    /// Lanes from top to bottom
    /// </summary>
    [SerializeField, Tooltip("From top to bottom auto sorted")] public List<Lane> m_lanes = new();
    [SerializeField] private List<Enemy> _enemyPrefabList = new();
    [SerializeField, Tooltip("Scene to load when enemy reaches left side")] private GameManager.GAMESCENE _upgradeScene = GameManager.GAMESCENE.UPGRADE;

    //[SerializeField] public UnityEvent m_onLooseCondition;
    //[SerializeField] public UnityEvent<int> m_onEnemyDied;

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
        _timeSinceLastSpawn += Time.deltaTime;

        if (_timeSinceLastSpawn > _currentSpawnDelay)
        {
            _timeSinceLastSpawn = 0f;
            _currentSpawnDelay = UnityEngine.Random.Range(_minSpawnDelay, _maxSpawnDelay);

            SpawnEnemy();
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

    public void SpawnEnemy(Enemy enemyPrefab = null, Lane lane = null)
    {
        enemyPrefab ??= _enemyPrefabList[UnityEngine.Random.Range(0, _enemyPrefabList.Count)];
        lane ??= m_lanes[UnityEngine.Random.Range(0, m_lanes.Count)];

        lane.SpawnEnemy(enemyPrefab);
    }
}
