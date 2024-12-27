using UnityEngine;

public class Bullet : LaneObject
{
    [SerializeField] public int m_damage = 1;

    public Bullet Init(float startingAdvancement)
    {
        m_advancement = startingAdvancement;
        return this;
    }

    public override void Advance()
    {
        // advances backwards
        m_advancement -= m_tilesSpeed * Time.deltaTime;
    }

    public override void UpdatePosition(Vector2 newPos)
    {
        this.transform.position = newPos;
    }
}
