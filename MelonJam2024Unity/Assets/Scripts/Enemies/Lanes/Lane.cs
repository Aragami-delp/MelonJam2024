using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using UnityEngine;
using static UnityEngine.EventSystems.EventTrigger;

public class Lane : MonoBehaviour
{
    [SerializeField] public Transform m_startPoint;
    [SerializeField] public Transform m_endPoint;
    [SerializeField] private SpriteRenderer _activeIndicator;

    private List<Enemy> _activeEnemyList = new();
    private List<Bullet> _activeBulletList = new();
    [SerializeField, InspectorName("Tiles Distance")] private float _distance = 5f;

    private void Start()
    {
        //_distance = Vector2.Distance(_startPoint.position, _endPoint.position);
    }

    public void SetLaneIndicator(Sprite newSprite)
    {
        _activeIndicator.sprite = newSprite;
    }

    public void Shoot(List<Bullet> bullets)
    {
        foreach (Bullet bullet in bullets)
        {
            bullet.InitShoot(_distance);
            bullet.transform.position = m_endPoint.position;
            _activeBulletList.Add(bullet);
        }
    }

    public void SpawnEnemy(Enemy enemyPrefab)
    {
        Enemy newEnemy = Instantiate(enemyPrefab);
        newEnemy.transform.position = m_startPoint.position;
        _activeEnemyList.Add(newEnemy);
    }

    private void Update()
    {
        _activeEnemyList.ForEach(enemy => MoveEnemy(enemy));
        _activeBulletList.ForEach(bullet => MoveBullet(bullet));
        BulletCollisionCheck();
        // Check for bullets out of bounds on lane without enemies
    }

    private void MoveEnemy(Enemy enemy)
    {
        enemy.Advance();
        enemy.UpdatePositionAdvancement(Vector2.Lerp(m_startPoint.position, m_endPoint.position, enemy.m_advancement/_distance));
        if (enemy.m_advancement >= _distance)
        {
            Debug.LogWarning("Loose Condition");
            LaneSystem.Instance.m_looseCondition.Invoke();
        }
    }

    private void MoveBullet(Bullet bullet)
    {
        bullet.Advance();
        bullet.UpdatePositionAdvancement(Vector2.Lerp(m_startPoint.position, m_endPoint.position, bullet.m_advancement/_distance));
    }

    private void BulletHitEnemy(Bullet bullet, Enemy enemy)
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
            Enemy furthestEnemy = GetFurthestLaneObject(_activeEnemyList);
            Bullet furthestBullet = GetClosestLaneObject(_activeBulletList);
            if (furthestBullet != null)
            {
                if (furthestEnemy != null && furthestEnemy.m_advancement > furthestBullet.m_advancement)
                {
                    BulletHitEnemy(furthestBullet, furthestEnemy);
                    continue;
                }
                else if (furthestBullet.m_advancement <= 0) // Bullet reaching right screen side
                {
                    _activeBulletList.Remove(furthestBullet);
                    Destroy(furthestBullet.gameObject);
                    continue;
                }
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
        if (m_startPoint is not null && m_endPoint is not null)
        {
            Gizmos.color = Color.blue;
            Gizmos.DrawLine(m_startPoint.position, m_endPoint.position);
        }
    }
    #endregion
}
