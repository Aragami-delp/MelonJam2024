using UnityEngine;

public class Bullet : LaneObject
{
    [SerializeField] public int m_damage = 1;
    [SerializeField] private float _rotateSpeed = 100f;
    [SerializeField] private float _maxFlyHight = 1f;
    [SerializeField, Tooltip("Backwards")] private AnimationCurve _flyCurve;
    private float _startingAdvancement = 0f;

    public Bullet InitShoot(float startingAdvancement)
    {
        _startingAdvancement = startingAdvancement;
        m_advancement = _startingAdvancement;
        return this;
    }

    public override void Advance()
    {
        // advances backwards
        m_advancement -= m_tilesSpeed * Time.deltaTime;
    }

    public void UpdatePositionCannon(Vector2 newPos)
    {
        this.transform.position = newPos;
    }

    public override void UpdatePositionAdvancement(Vector2 newPos)
    {
        this.transform.position = new Vector2(newPos.x, newPos.y + _flyCurve.Evaluate(m_advancement/_startingAdvancement) * _maxFlyHight);
    }

    private void Update()
    {
        transform.Rotate(0f, 0f, _rotateSpeed * Time.deltaTime);
    }
}
