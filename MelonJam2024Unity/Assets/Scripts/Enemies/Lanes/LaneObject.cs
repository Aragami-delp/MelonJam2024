using UnityEngine;

public abstract class LaneObject : MonoPoolItem
{
    [SerializeField, InspectorName("Tiles/Sec")] public float m_tilesSpeed = 1f;
    [HideInInspector] public float m_advancement = 0f;

    public abstract void Advance();
    public abstract void UpdatePositionAdvancement(Vector2 newPos);
}
