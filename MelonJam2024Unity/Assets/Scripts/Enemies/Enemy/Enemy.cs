using UnityEngine;

public class Enemy : LaneObject
{
    public float m_speedMultiplyer = 1; 
    [SerializeField] public int m_health = 2;
    [SerializeField] public int m_lootValue = 1;

    public override void Advance()
    {
        m_advancement += m_tilesSpeed * Time.deltaTime * m_speedMultiplyer;
    }

    public override void UpdatePositionAdvancement(Vector2 newPos)
    {
        this.transform.position = newPos;
    }
}
