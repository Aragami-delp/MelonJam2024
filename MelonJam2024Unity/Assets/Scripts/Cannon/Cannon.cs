using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;

public class MoveCannon : MonoBehaviour
{
    [SerializeField] private float _moveSpeed = 1f;
    [SerializeField] private Transform _scrapPickupY;
    [SerializeField] private Transform _cannon;
    [SerializeField] private Transform _scrapHoldPos;
    [SerializeField] private Bullet _bulletPrefab;
    [SerializeField] private UnityEvent<bool> _switchPolarity;
    private List<float> _lanePositions = new();
    private bool _isMoving = false;
    private Vector3 _currentTarget = new();
    private List<Bullet> _holdingScrap = new();
    private bool _isAttracting = false;

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

    private Lane GetCurrentLane => EnemySpawner.Instance.m_lanes[_currentLane];

    public void SwitchPolarity()
    {
        _isAttracting = !_isAttracting;
        _switchPolarity?.Invoke(_isAttracting);
        if (_isAttracting)
        {
            PickupScrap();
        }
        else
        {
            Shoot();
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
        if (_currentLane != -1)
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
    }
}
