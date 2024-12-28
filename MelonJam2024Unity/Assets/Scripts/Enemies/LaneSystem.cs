using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

[DefaultExecutionOrder(-50)]
public class LaneSystem : MonoBehaviour
{
    public static LaneSystem Instance { get; private set; }

    [SerializeField] private float _minSpawnDelay = 0.5f;
    [SerializeField] private float _maxSpawnDelay = 1.0f;
    private float _currentSpawnDelay = 0.75f;
    private float _timeSinceLastSpawn = 0f;

    [SerializeField] public Sprite m_offLaneSprite;
    [SerializeField] public Sprite m_onLaneSprite;

    /// <summary>
    /// Lanes from top to bottom
    /// </summary>
    [SerializeField, Tooltip("From top to bottom auto sorted")] public List<Lane> m_lanes = new();
    [SerializeField] private List<Enemy> _enemyPrefabList = new();

    [SerializeField] public UnityEvent m_looseCondition;
    [SerializeField] public UnityEvent<int> m_enemyDied;

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

    public void UpdateLaneIndicator(int newlyActiveLane)
    {
        if (newlyActiveLane != -1)
        {
            m_lanes[newlyActiveLane].SetLaneIndicator(m_onLaneSprite);
        }
        m_lanes.Where((lane, index) => index != newlyActiveLane).ToList().ForEach(x => x.SetLaneIndicator(m_offLaneSprite));
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
        lane ??= m_lanes[UnityEngine.Random.Range(0, m_lanes.Count)];

        lane.SpawnEnemy(enemyPrefab);
    }
}
