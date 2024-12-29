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
    [SerializeField] private SpriteRenderer _landMineIndicator;

    private List<Enemy> _activeEnemyList = new();
    private List<Bullet> _activeBulletList = new();
    [SerializeField, InspectorName("Tiles Distance")] private float _distance = 5f;

    private bool landMineExploded = false;

    private void Start()
    {
        //_distance = Vector2.Distance(_startPoint.position, _endPoint.position);
        _landMineIndicator.enabled = LaneSystem.Instance.m_landMinesActive;
    }

    public void SetLaneIndicator(bool active)
    {
        _activeIndicator.enabled = active;
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

    public void SpawnEnemy(Enemy enemyPrefab, float slowDebuffMultiplierReduction)
    {
        Enemy newEnemy = Instantiate(enemyPrefab);
        newEnemy.m_speedMultiplyer = 1 - slowDebuffMultiplierReduction;
        newEnemy.transform.position = m_startPoint.position;
        _activeEnemyList.Add(newEnemy);
    }

    private void Update()
    {
        _activeEnemyList.ToList().ForEach(enemy => MoveEnemy(enemy));
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
            if (!landMineExploded && LaneSystem.Instance.m_landMinesActive)
            {
                TriggerLandMine(enemy);
                return;
            }
            
            Debug.LogWarning("Loose Condition");
            //LaneSystem.Instance.m_onLooseCondition.Invoke();
            LaneSystem.Instance.LoadUpgradeScene();
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
            KillEnemy(enemy);
        }
    }

    private void KillEnemy(Enemy enemy)
    {
        _activeEnemyList.Remove(enemy);
        //LaneSystem.Instance.m_onEnemyDied.Invoke(enemy.m_lootValue);
        if (GameManager.Instance) GameManager.Instance.Coins += enemy.m_lootValue;
        Destroy(enemy.gameObject); // TODO: Pooling
    }

    private void TriggerLandMine(Enemy enemy)
    {
        landMineExploded = true;
        _landMineIndicator.enabled = false;
        KillEnemy(enemy);
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
