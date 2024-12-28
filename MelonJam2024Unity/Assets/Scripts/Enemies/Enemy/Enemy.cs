using UnityEngine;

public class Enemy : LaneObject
{
    [SerializeField] public int m_health = 2;

    public override void Advance()
    {
        m_advancement += m_tilesSpeed * Time.deltaTime;
    }

    public override void UpdatePositionAdvancement(Vector2 newPos)
    {
        this.transform.position = newPos;
    }
}
