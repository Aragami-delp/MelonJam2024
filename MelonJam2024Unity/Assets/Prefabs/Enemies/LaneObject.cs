using UnityEngine;

public abstract class LaneObject : MonoBehaviour
{
    [SerializeField, InspectorName("Tiles/Sec")] public float m_tilesSpeed = 1f;
    [HideInInspector] public float m_advancement = 0f;

    public abstract void Advance();
    public abstract void UpdatePosition(Vector2 newPos);
}
