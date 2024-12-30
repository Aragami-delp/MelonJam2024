using UnityEngine;

public abstract class LaneObject : MonoPoolItem
{
    [SerializeField, InspectorName("Tiles/Sec")] public float m_tilesSpeed = 1f;
    [HideInInspector] public float m_advancement = 0f;
    private SpriteRenderer _renderer;

    public abstract void Advance();
    public abstract void UpdatePositionAdvancement(Vector2 newPos);

    public virtual void Awake()
    {
        _renderer = GetComponent<SpriteRenderer>();
    }

    public virtual void SetSpriteLayer(int layer)
    {
        _renderer.sortingOrder = layer;
    }
}
