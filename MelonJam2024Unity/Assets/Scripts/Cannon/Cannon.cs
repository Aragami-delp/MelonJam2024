using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;

public class Cannon : MonoBehaviour
{
    public static Cannon Instance { get; private set; }

    [SerializeField] private Transform _scrapPickupY;
    [SerializeField] private Transform _cannon;
    [SerializeField] private Transform _scrapHoldPos;
    [SerializeField] private Bullet _bulletPrefab;
    [SerializeField] private int _startingDamage;
    [SerializeField, Tooltip("True for magnet turns on")] private UnityEvent<bool> _onSwitchPolarity;

    private List<float> _lanePositions = new();
    private bool _isMoving = false;
    private Vector3 _currentTarget = new();
    private List<Bullet> _holdingScrap = new();
    private bool _isAttracting = false;
    private bool _shootNextFrame = false;

    #region Upgrade Targets
    [Header("Upgrades")]
    public int m_maxScrapCapacity = 1;
    public bool m_autoReload = true;
    public float _moveSpeed = 4f;
    private int _cannonUpgradeDamage = 0;
    /// <summary>
    /// Default Value is 1
    /// </summary>
    public int m_cannonDamage
    {
        get { return _cannonUpgradeDamage; }
        set { _cannonUpgradeDamage = Mathf.Clamp(value, 1, 1000); }
    }
    #endregion

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

    private bool _isHoldingScrap => _holdingScrap.Count > 0;

    /// <summary>
    /// -1 == Pickup; 
    /// </summary>
    private int _currentLane = -1;

    [ContextMenu("Move To Scrap")]
    public void MoveToScrap()
    {
        MoveToLane(-1);
    }

    /// <summary>
    /// Moves to a specific lane
    /// </summary>
    /// <param name="lane">Scrap pickup is lane -1. Lane 0 on top decending</param>
    public void MoveToLane(int lane)
    {
        // int Mathf.Clamp(value, min, max) is inclusive ... for some reason
        if (_currentLane == lane) { return; }

        _currentLane = Mathf.Clamp(lane, -1, LaneSystem.Instance.m_lanes.Count - 1);
        _currentTarget = new Vector2(_cannon.transform.position.x, _currentLane == -1 ? _scrapPickupY.position.y : GetCurrentLane.m_endPoint.transform.position.y);

        if (!_isMoving)
            MusicSoundManagement.Instance.PlaySfx(MusicSoundManagement.AUDIOTYPE.MAGNET_MOVE);
        _isMoving = true;

        LaneSystem.Instance.UpdateLaneIndicator(_currentLane);
    }

    // Top to bottom. Highest lane is 0
    [ContextMenu("Move Lane Down")]
    public void MoveLaneDown()
    {
        MusicSoundManagement.Instance.PlaySfx(MusicSoundManagement.AUDIOTYPE.BUTTON_DOWN);
        MoveToLane(_currentLane + 1);
    }

    [ContextMenu("Move Lane Up")]
    public void MoveLaneUp()
    {
        MusicSoundManagement.Instance.PlaySfx(MusicSoundManagement.AUDIOTYPE.BUTTON_UP);
        MoveToLane(_currentLane - 1);
    }

    private Lane GetCurrentLane => _currentLane != -1 ? LaneSystem.Instance.m_lanes[_currentLane] : null;

    public void SwitchPolarity()
    {
        MusicSoundManagement.Instance.PlaySfx(MusicSoundManagement.AUDIOTYPE.SWITCH_POLARITY);
        MusicSoundManagement.Instance.PlaySfx(MusicSoundManagement.AUDIOTYPE.BUTTON_POLARITY);
        _isAttracting = !_isAttracting;
        _onSwitchPolarity?.Invoke(_isAttracting);
        if (_isAttracting)
        {
            _shootNextFrame = false;
        }
        else
        {
            _shootNextFrame = true;
        }
    }

    public void Shoot()
    {
        if (_currentLane != -1)
        {
            GetCurrentLane.Shoot(_holdingScrap);
            _holdingScrap.Clear();
        }
    }

    public void PickupScrap(List<Bullet> scrap = null)
    {
        if (scrap.Count > 0)
        {
            MusicSoundManagement.Instance.PlaySfx(MusicSoundManagement.AUDIOTYPE.MAGNET_SCRAP_ATTACH);
        }
        _holdingScrap = scrap;
        _holdingScrap.ForEach(bullet =>
            {
                bullet.m_damage = _startingDamage + m_cannonDamage;
                bullet.transform.position = _scrapHoldPos.position;
                bullet.transform.SetParent(null);
                bullet.enabled = true;
            });


    }

    [ContextMenu("Shoot Test")]
    [Obsolete("Only for the inspector/testing")]
    public void SpawnOneTestBullet()
    {
        if (Application.isPlaying && _isAttracting)
        {
            _holdingScrap = new List<Bullet>
            {
                Instantiate(_bulletPrefab),
                Instantiate(_bulletPrefab),
                Instantiate(_bulletPrefab)
            };
        }
    }

    private void Update()
    {
        //if (Input.GetKeyDown(KeyCode.DownArrow))
        //{
        //    MoveLaneDown();
        //}
        //else if (Input.GetKeyDown(KeyCode.UpArrow))
        //{
        //    MoveLaneUp();
        //}
        //else if (Input.GetKeyDown(KeyCode.Space))
        //{
        //    SwitchPolarity();
        //}

        if (_isMoving)
        {
            _cannon.transform.position = Vector3.MoveTowards(_cannon.transform.position, _currentTarget, _moveSpeed * Time.deltaTime);
            _holdingScrap.ForEach(x => x.UpdatePositionCannon(_scrapHoldPos.position));
            if (_cannon.transform.position == _currentTarget)
            {
                _isMoving = false;
                MusicSoundManagement.Instance.StopSFXLoop(MusicSoundManagement.AUDIOTYPE.MAGNET_MOVE);
            }
        }
        if (_shootNextFrame && !_isMoving)
        {
            Shoot();
            _shootNextFrame = false;
            if (m_autoReload)
            {
                _isAttracting = !_isAttracting;
                MoveToScrap();
            }
        }
        else if (!_isMoving && _isAttracting && _currentLane == -1 && _holdingScrap.Count == 0)
        {
            PickupScrap(ScrapTable.Instance.TryGetScrap(m_maxScrapCapacity));
        }
    }
}
