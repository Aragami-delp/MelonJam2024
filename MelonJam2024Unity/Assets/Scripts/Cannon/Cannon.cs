using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;

public class MoveCannon : MonoBehaviour
{
    [SerializeField] private float _moveSpeed = 4f;
    [SerializeField] private Transform _scrapPickupY;
    [SerializeField] private Transform _cannon;
    [SerializeField] private Transform _scrapHoldPos;
    [SerializeField] private Bullet _bulletPrefab;
    [SerializeField, Tooltip("True for magnet turns on")] private UnityEvent<bool> _onSwitchPolarity;
    private List<float> _lanePositions = new();
    private bool _isMoving = false;
    private Vector3 _currentTarget = new();
    private List<Bullet> _holdingScrap = new();
    private bool _isAttracting = false;
    private bool _shootNextFrame = false;

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

        _currentLane = Mathf.Clamp(lane, -1, EnemySpawner.Instance.m_lanes.Count - 1);
        _currentTarget = new Vector2(_cannon.transform.position.x, _currentLane == -1 ? _scrapPickupY.position.y : GetCurrentLane.m_endPoint.transform.position.y);

        _isMoving = true;
    }

    // Top to bottom. Highest lane is 0
    [ContextMenu("Move Lane Down")]
    public void MoveLaneDown()
    {
        MoveToLane(_currentLane + 1);
    }

    [ContextMenu("Move Lane Up")]
    public void MoveLaneUp()
    {
        MoveToLane(_currentLane - 1);
    }

    private Lane GetCurrentLane => _currentLane != -1 ? EnemySpawner.Instance.m_lanes[_currentLane] : null;

    public void SwitchPolarity()
    {
        _isAttracting = !_isAttracting;
        _onSwitchPolarity?.Invoke(_isAttracting);
        if (_isAttracting)
        {
            _shootNextFrame = false;
            //PickupScrap();
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
        //TODO: Pickup for real
        if (_currentLane != -1 || _isMoving) // TODO: Pickup once arrived and continue to pick up until left (or maybe just pickup on leave?)
        {
            return;
        }
        if (scrap is null || scrap.Count == 0)
        {
            _holdingScrap = new List<Bullet>
            {
                Instantiate(_bulletPrefab),
                Instantiate(_bulletPrefab),
                Instantiate(_bulletPrefab)
            };
            _holdingScrap.ForEach(x => x.transform.position = _scrapHoldPos.position);
        }
        else
        {
            _holdingScrap = scrap;
        }
        //Bullet newBullet = Instantiate(_bulletPrefab);
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
        if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            MoveLaneDown();
        }
        else if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            MoveLaneUp();
        }
        else if (Input.GetKeyDown(KeyCode.Space))
        {
            SwitchPolarity();
        }
        else if (Input.GetKeyDown(KeyCode.Return))
        {
            SpawnOneTestBullet();
        }

        if (_isMoving)
        {
            _cannon.transform.position = Vector3.MoveTowards(_cannon.transform.position, _currentTarget, _moveSpeed * Time.deltaTime);
            _holdingScrap.ForEach(x => x.UpdatePositionCannon(_scrapHoldPos.position));
            if (_cannon.transform.position == _currentTarget)
            {
                _isMoving = false;
            }
        }
        if (_shootNextFrame && !_isMoving)
        {
            Shoot();
            _shootNextFrame = false;
        }
        else if (!_isMoving && _isAttracting && _currentLane == -1 && _holdingScrap.Count == 0)
        {
            PickupScrap();
        }
    }
}
