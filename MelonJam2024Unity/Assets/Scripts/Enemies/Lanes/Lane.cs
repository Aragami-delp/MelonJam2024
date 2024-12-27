using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using UnityEngine;
using static UnityEngine.EventSystems.EventTrigger;

public class Lane : MonoBehaviour
{
    [SerializeField] private Transform _startPoint;
    [SerializeField] private Transform _endPoint;

    private List<BaseEnemy> _activeEnemyList = new();
    private List<BaseBullet> _activeBulletList = new();
    [SerializeField, InspectorName("Tiles Distance")] private float _distance = 5f;

    [SerializeField] private List<BaseEnemy> _enemyPrefabList;
    [SerializeField] private BaseBullet _testBulletPrefab;

    private void Start()
    {
        //_distance = Vector2.Distance(_startPoint.position, _endPoint.position);
    }

    [ContextMenu("Shoot Test")]
    [Obsolete("Only for the inspector")]
    public void SpawnOneTestBullet()
    {
        if (Application.isPlaying)
        {
            Shoot(_testBulletPrefab);
        }
    }

    [ContextMenu("SpawnEnemy")]
    [Obsolete("Only for the inspector")]
    public void SpawnOneRandomEnemy()
    {
        if (Application.isPlaying)
        {
            SpawnEnemy(_enemyPrefabList[UnityEngine.Random.Range(0, _enemyPrefabList.Count)]);
        }
    }

    public void Shoot(BaseBullet bulletPrefab)
    {
        BaseBullet newBullet = Instantiate(bulletPrefab).Init(_distance);
        newBullet.transform.position = _endPoint.position;
        _activeBulletList.Add(newBullet);
    }

    public void SpawnEnemy(BaseEnemy enemyPrefab)
    {
        BaseEnemy newEnemy = Instantiate(enemyPrefab);
        newEnemy.transform.position = _startPoint.position;
        _activeEnemyList.Add(newEnemy);
    }

    private void Update()
    {
        _activeEnemyList.ForEach(enemy => MoveEnemy(enemy));
        _activeBulletList.ForEach(bullet => MoveBullet(bullet));
        BulletCollisionCheck();
        // Check for bullets out of bounds on lane without enemies
    }

    private void MoveEnemy(BaseEnemy enemy)
    {
        enemy.Advance();
        enemy.UpdatePosition(Vector2.Lerp(_startPoint.position, _endPoint.position, enemy.m_advancement/_distance));
    }

    private void MoveBullet(BaseBullet bullet)
    {
        bullet.Advance();
        bullet.UpdatePosition(Vector2.Lerp(_startPoint.position, _endPoint.position, bullet.m_advancement/_distance));
    }

    private void BulletHitEnemy(BaseBullet bullet, BaseEnemy enemy)
    {
        _activeBulletList.Remove(bullet);
        Destroy(bullet.gameObject);
        enemy.m_health -= bullet.m_damage;
        if (enemy.m_health <= 0)
        {
            _activeEnemyList.Remove(enemy);
            Destroy(enemy.gameObject); // TODO: Pooling
        }
    }

    private void BulletCollisionCheck()
    {
        do
        {
            BaseEnemy furthestEnemy = GetFurthestLaneObject(_activeEnemyList);
            BaseBullet furthestBullet = GetClosestLaneObject(_activeBulletList);
            if (furthestEnemy is not null && furthestBullet is not null &&
                furthestEnemy.m_advancement > furthestBullet.m_advancement)
            {
                BulletHitEnemy(furthestBullet, furthestEnemy);
                continue;
            }
            return;
        }
        while (true); // TODO: Bessere Ueberpruefung als true
    }

    /// <summary>
    /// Furthest from start away
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="laneObjects"></param>
    /// <returns></returns>
    private T GetFurthestLaneObject<T>(List<T> laneObjects) where T : LaneObject
    {
        if (laneObjects.Count == 0) return null;
        return laneObjects.Aggregate((maxObject, nextObject) => maxObject.m_advancement > nextObject.m_advancement ? maxObject : nextObject);
    }

    /// <summary>
    /// Closest to start
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="laneObjects"></param>
    /// <returns></returns>
    private T GetClosestLaneObject<T>(List<T> laneObjects) where T : LaneObject
    {
        if (laneObjects.Count == 0) return null;
        return laneObjects.Aggregate((maxObject, nextObject) => maxObject.m_advancement < nextObject.m_advancement ? maxObject : nextObject);
    }

    #region Debug
    public void OnDrawGizmos()
    {
        if (_startPoint is not null && _endPoint is not null)
        {
            Gizmos.color = Color.blue;
            Gizmos.DrawLine(_startPoint.position, _endPoint.position);
        }
    }
    #endregion
}
